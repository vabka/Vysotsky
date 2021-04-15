using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Dto.Issues;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API.Controllers.Issues
{
    [Route(Resources.Issues)]
    public class IssuesController : ApiController
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IIssueService _issueService;
        private readonly IRoomService _roomService;

        public IssuesController(ICurrentUserProvider currentUserProvider,
            IIssueService issueService,
            IRoomService roomService)
        {
            _currentUserProvider = currentUserProvider;
            _issueService = issueService;
            _roomService = roomService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> CreateIssue(IssueDto newIssue)
        {
            var currentUser = _currentUserProvider.CurrentUser;
            if (currentUser!.Role == UserRole.Worker)
                return NotAuthorized("Worker is not authorized to create issues", "issues.notAthorized");
            var areaTask = _issueService.GetAreaByIdOrNull(newIssue.AreaId);
            var roomTask = _roomService.GetRoomByIdOrNullAsync(newIssue.RoomId);
            var area = await areaTask;
            if (area == null)
                return BadRequest("Area not found", "issues.areaNotFound");
            var room = await roomTask;
            if (room == null)
                return BadRequest("Room not found", "rooms.roomNotFound");
            var createdIssue =
                await _issueService.CreateIssueAsync(newIssue.Title, newIssue.Description, area, room, currentUser);
            return Ok(createdIssue.ToDto());
        }


        [HttpPost("{issueId:long}/needInfo")]
        public async Task<ActionResult<ApiResponse>> MoveIssueToNeedInfo(long issueId, MoveIssueToNeedInfoDto data)
        {
            var issue = await _issueService.GetIssueByIdOrNullAsync(issueId);
            if (issue == null)
                return NotFound("Issue not found", "issue.notFound");
            if (!_currentUserProvider.CanMoveIssueToNeedInfo())
                return NotAuthorized("Only supervisor can move task to NeedInfo state", "issues.notAuthorized");
            var newState =
                await _issueService.MoveIssueToNeedInformation(issue, _currentUserProvider.CurrentUser!, data.Message);
            return Ok(newState.ToDto());
        }
    }
}