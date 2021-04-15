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
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IIssueService issueService;
        private readonly IRoomService roomService;
        private readonly IWorkerService workerService;
        private readonly ICategoriesService categoriesService;

        public IssuesController(ICurrentUserProvider currentUserProvider,
            IIssueService issueService,
            IRoomService roomService,
            IWorkerService workerService,
            ICategoriesService categoriesService)
        {
            this.currentUserProvider = currentUserProvider;
            this.issueService = issueService;
            this.roomService = roomService;
            this.workerService = workerService;
            this.categoriesService = categoriesService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> CreateIssue([FromBody] IssueDto newIssue)
        {
            var currentUser = currentUserProvider.CurrentUser;
            if (currentUser!.Role == UserRole.Worker)
            {
                return NotAuthorized("Worker is not authorized to create issues", "issues.notAthorized");
            }

            var areaTask = issueService.GetAreaByIdOrNull(newIssue.AreaId);
            var roomTask = roomService.GetRoomByIdOrNullAsync(newIssue.RoomId);
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
                await issueService.CreateIssueAsync(newIssue.Title, newIssue.Description, area, room, currentUser);
            return Ok(createdIssue.ToDto());
        }


        [HttpPost("{issueId:long}/needInfo")]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> MoveIssueToNeedInfo([FromRoute] long issueId,
            [FromBody] MoveIssueToNeedInfoDto data)
        {
            var issue = await issueService.GetIssueByIdOrNullAsync(issueId);
            if (issue == null)
            {
                return IssueNotFound();
            }

            if (!currentUserProvider.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can move task to NeedInfo state", "issues.notAuthorized");
            }

            var newState =
                await issueService.MoveIssueToNeedInformationAsync(issue, currentUserProvider.CurrentUser!,
                    data.Message);
            return Ok(newState.ToDto());
        }

        private ActionResult IssueNotFound() => NotFound("Issue not found", "issue.notFound");

        [HttpPost("{issueId:long}/inProgress")]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> MoveIssueToInProgress([FromRoute] long issueId,
            [FromBody] MoveIssueToInPgoressDto data)
        {
            var issue = await issueService.GetIssueByIdOrNullAsync(issueId);
            if (!currentUserProvider.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can move task to InProgress state", "issues.notAuthorized");
            }

            if (issue == null)
            {
                return IssueNotFound();
            }

            var currentUser = currentUserProvider.CurrentUser!;
            var worker = await workerService.GetWorkerByIdOrNullAsync(data.WorkerId);
            if (worker == null)
            {
                return WorkerNotFound();
            }

            var category = await categoriesService.GetCategoryByIdOrNullAsync(data.CategoryId);
            if (category == null)
            {
                return CategoryNotFound();
            }

            var newState =
                await issueService.TakeToWorkAsync(issue, currentUser, worker, category);
            return Ok(newState.ToDto());
        }

        private ActionResult<ApiResponse<PersistedIssueDto>> CategoryNotFound() => throw new System.NotImplementedException();

        private ActionResult<ApiResponse<PersistedIssueDto>> WorkerNotFound() => NotFound("Worker not found", "workers.notFound");
    }
}
