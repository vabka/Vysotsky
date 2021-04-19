using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Dto.Issues;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.API.Controllers
{
    [Route(Resources.Issues)]
    public class IssuesController : ApiController
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IIssueService _issueService;
        private readonly IRoomService _roomService;
        private readonly IWorkerService _workerService;
        private readonly ICategoriesService _categoriesService;

        public IssuesController(ICurrentUserProvider currentUserProvider,
            IIssueService issueService,
            IRoomService roomService,
            IWorkerService workerService,
            ICategoriesService categoriesService)
        {
            _currentUserProvider = currentUserProvider;
            _issueService = issueService;
            _roomService = roomService;
            _workerService = workerService;
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedData<ShortPersistedIssueDto>>>> GetAllIssues(
            [FromQuery] PaginationParameters paginationParameters)
        {
            var (total, data) = await _issueService.GetIssuesToShowUser(_currentUserProvider.CurrentUser,
                paginationParameters.Until, paginationParameters.ToTake(), paginationParameters.ToSkip());
            return Ok(PaginatedData.Create(paginationParameters,
                total,
                data.Select(x => x.ToDto()).ToArray(),
                Resources.Issues));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> CreateIssue([FromBody] IssueDto newIssue)
        {
            var currentUser = _currentUserProvider.CurrentUser;
            if (currentUser!.Role == UserRole.Worker)
            {
                return NotAuthorized("Worker is not authorized to create issues", "issues.notAthorized");
            }

            var areaTask = _issueService.GetAreaByIdOrNull(newIssue.AreaId);
            var roomTask = _roomService.GetRoomByIdOrNullAsync(newIssue.RoomId);
            var area = await areaTask;
            if (area == null)
            {
                return BadRequest("Area not found", "issues.areaNotFound");
            }

            var room = await roomTask;
            if (room == null)
            {
                return BadRequest("Room not found", "rooms.roomNotFound");
            }

            var createdIssue =
                await _issueService.CreateIssueAsync(newIssue.Title, newIssue.Description, area, room, currentUser);
            return Ok(createdIssue.ToDto());
        }


        [HttpPost("{issueId:long}/needInfo")]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> MoveIssueToNeedInfo([FromRoute] long issueId,
            [FromBody] MoveIssueToNeedInfoDto data)
        {
            var issue = await _issueService.GetIssueByIdOrNullAsync(issueId);
            if (issue == null)
            {
                return IssueNotFound();
            }

            if (!_currentUserProvider.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can move task to NeedInfo state", "issues.notAuthorized");
            }

            var newState =
                await _issueService.MoveIssueToNeedInformationAsync(issue, _currentUserProvider.CurrentUser!,
                    data.Message);
            return Ok(newState.ToDto());
        }

        [HttpGet("{issueId:long}/history")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Issue>>>> GetHistory([FromRoute] long issueId) =>
            await _issueService.GetIssueByIdOrNullAsync(issueId) switch
            {
                null      => IssueNotFound(),
                var issue => Ok((await _issueService.GetIssueHistoryAsync(issue)).Select(i => i.ToDto()))
            };

        private ActionResult IssueNotFound() => NotFound("Issue not found", "issue.notFound");

        [HttpPost("{issueId:long}/inProgress")]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> MoveIssueToInProgress([FromRoute] long issueId,
            [FromBody] MoveIssueToInPgoressDto data)
        {
            if (!_currentUserProvider.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can move task to InProgress state", "issues.notAuthorized");
            }

            var issue = await _issueService.GetIssueByIdOrNullAsync(issueId);
            if (issue == null)
            {
                return IssueNotFound();
            }

            var worker = await _workerService.GetWorkerByIdOrNullAsync(data.WorkerId);
            if (worker == null)
            {
                return WorkerNotFound();
            }

            var category = await _categoriesService.GetCategoryByIdOrNullAsync(data.CategoryId);
            if (category == null)
            {
                return CategoryNotFound();
            }

            var newState =
                await _issueService.TakeToWorkAsync(issue, _currentUserProvider.CurrentUser, worker, category);
            return Ok(newState.ToDto());
        }

        private ActionResult<ApiResponse<PersistedIssueDto>> CategoryNotFound() =>
            BadRequest("Category or area not found", "categories.notFound");

        private ActionResult<ApiResponse<PersistedIssueDto>> WorkerNotFound() =>
            BadRequest("Worker not found", "workers.notFound");
    }
}
