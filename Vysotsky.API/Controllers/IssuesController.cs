using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Dto.Issues;
using Vysotsky.API.Infrastructure;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.API.Controllers
{
    [Route(Resources.Issues)]
    public class IssuesController : ApiController
    {
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IIssueService issueService;
        private readonly IRoomService roomService;
        private readonly IWorkerService workerService;
        private readonly ICategoriesService categoriesService;
        private readonly IUserService userService;

        public IssuesController(ICurrentUserProvider currentUserProvider,
            IIssueService issueService,
            IRoomService roomService,
            IWorkerService workerService,
            ICategoriesService categoriesService,
            IUserService userService)
        {
            this.currentUserProvider = currentUserProvider;
            this.issueService = issueService;
            this.roomService = roomService;
            this.workerService = workerService;
            this.categoriesService = categoriesService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedData<ShortPersistedIssueDto>>>> GetAllIssues(
            [FromQuery] PaginationParameters paginationParameters)
        {
            var (total, data) = await issueService.GetIssuesToShowUser(currentUserProvider.CurrentUser,
                paginationParameters.Until, paginationParameters.Take, paginationParameters.Skip);
            return Ok(PaginatedData.Create(paginationParameters,
                total,
                data.Select(x => x.ToDto()).ToArray()));
        }

        [HttpGet("{issueId:long}")]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> GetIssueById([FromRoute] long issueId)
        {
            var issue = await issueService.GetIssueByIdOrNullAsync(issueId);
            if (issue == null)
            {
                return IssueNotFound();
            }

            var issueCustomer = await userService.GetUserByIdOrNullAsync(issue.AuthorId);
            if (currentUserProvider.CurrentUser.IsCustomer() &&
                currentUserProvider.CurrentUser.OrganizationId != issueCustomer?.OrganizationId)
            {
                return NotAuthorized("Customer can read only authored issues", "issues.notAuthorized");
            }

            return Ok(issue.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> CreateIssue([FromBody] IssueDto newIssue)
        {
            var currentUser = currentUserProvider.CurrentUser;
            if (!currentUser.CanCreateIssue())
            {
                return NotAuthorized("User is not authorized to create issues", "issues.notAuthorized");
            }

            var category = await categoriesService.GetByIdOrNullAsync(newIssue.CategoryId);
            if (category == null)
            {
                return BadRequest("Category not found", "categories.categoryNotFound");
            }

            var room = await roomService.GetRoomByIdOrNullAsync(newIssue.RoomId);

            if (room == null)
            {
                return BadRequest("Room not found", "rooms.roomNotFound");
            }

            var createdIssue =
                await issueService.CreateIssueAsync(newIssue.Title, newIssue.Description, category, room, currentUser);
            return Ok(createdIssue.ToDto());
        }


        [HttpGet("{issueId:long}/history")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Issue>>>> GetHistory([FromRoute] long issueId) =>
            await issueService.GetIssueByIdOrNullAsync(issueId) switch
            {
                null      => IssueNotFound(),
                var issue => Ok((await issueService.GetIssueHistoryAsync(issue)).Select(i => i.ToDto()))
            };

        [HttpPost("{issueId:long}/state/needInfo")]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> MoveIssueToNeedInfo([FromRoute] long issueId,
            [FromBody] MoveIssueToNeedInfoDto data)
        {
            var issue = await issueService.GetIssueByIdOrNullAsync(issueId);
            if (issue == null)
            {
                return IssueNotFound();
            }

            if (!currentUserProvider.CurrentUser.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can move task to NeedInfo state", "issues.notAuthorized");
            }

            var newState =
                await issueService.MoveIssueToNeedInformationAsync(issue, currentUserProvider.CurrentUser!,
                    data.Message);
            return Ok(newState.ToDto());
        }

        private ActionResult IssueNotFound() => NotFound("Issue not found", "issue.notFound");

        private static ActionResult<ApiResponse<PersistedIssueDto>> WorkerNotFound() =>
            BadRequest("Worker not found", "workers.notFound");

        [HttpPost("{issueId:long}/state/completed")]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> MoveIssueToCompleted([FromRoute] long issueId,
            [FromBody] MoveIssueToCompletedDto data)
        {
            var issue = await issueService.GetIssueByIdOrNullAsync(issueId);
            if (issue == null)
            {
                return IssueNotFound();
            }


            if (issue.WorkerId == null)
            {
                return BadRequest("Issue in invalid state", "issue.invalidState");
            }

            if (!currentUserProvider.CurrentUser.CanCompleteIssue(issue))
            {
                return NotAuthorized("Only worker can complete issue", "issues.notAuthorized");
            }

            var worker = currentUserProvider.CurrentUser;
            var message = data.Message;
            var newState = await issueService.CompleteIssueAsync(issue, worker, message);
            return Ok(newState.ToDto());
        }

        [HttpPost("{issueId:long}/state/inProgress")]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> MoveIssueToInProgress([FromRoute] long issueId,
            [FromBody] MoveIssueToInPgoressDto data)
        {
            if (!currentUserProvider.CurrentUser.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can move task to InProgress state", "issues.notAuthorized");
            }

            var issue = await issueService.GetIssueByIdOrNullAsync(issueId);
            if (issue == null)
            {
                return IssueNotFound();
            }

            var worker = await workerService.GetWorkerByIdOrNullAsync(data.WorkerId);
            if (worker == null)
            {
                return WorkerNotFound();
            }

            var newState =
                await issueService.TakeToWorkAsync(issue, currentUserProvider.CurrentUser, worker, data.Message);
            return Ok(newState.ToDto());
        }

        [HttpPost("{issueId:long}/state/rejected")]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> RejectIssue([FromRoute] long issueId,
            [FromBody] MoveIssueToRejectedDto data)
        {
            var issue = await issueService.GetIssueByIdOrNullAsync(issueId);
            if (issue == null)
            {
                return IssueNotFound();
            }

            if (!currentUserProvider.CurrentUser.IsSupervisor())
            {
                return NotAuthorized("Only supervisor can reject task", "issues.notAuthorized");
            }

            return Ok((await issueService.RejectIssueAsync(issue, data.Message)).ToDto());
        }
    }

    public class MoveIssueToRejectedDto
    {
        public string Message { get; init; } = "";
    }

    public class MoveIssueToCompletedDto
    {
        public string Message { get; init; } = "";
    }
}
