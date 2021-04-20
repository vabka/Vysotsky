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
            ICategoriesService categoriesService, IUserService userService)
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
                paginationParameters.Until, paginationParameters.ToTake(), paginationParameters.ToSkip());
            return Ok(PaginatedData.Create(paginationParameters,
                total,
                data.Select(x => x.ToDto()).ToArray(),
                Resources.Issues));
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
            if (currentUserProvider.IsCustomer() &&
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
            if (currentUser!.Role == UserRole.Worker)
            {
                return NotAuthorized("Worker is not authorized to create issues", "issues.notAuthorized");
            }

            var categoryTask = categoriesService.GetByIdOrNullAsync(newIssue.CategoryId);
            var roomTask = roomService.GetRoomByIdOrNullAsync(newIssue.RoomId);
            var category = await categoryTask;
            if (category == null)
            {
                return BadRequest("Area not found", "issues.areaNotFound");
            }

            var room = await roomTask;
            if (room == null)
            {
                return BadRequest("Room not found", "rooms.roomNotFound");
            }

            var createdIssue =
                await issueService.CreateIssueAsync(newIssue.Title, newIssue.Description, category, room, currentUser);
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

        [HttpGet("{issueId:long}/history")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Issue>>>> GetHistory([FromRoute] long issueId) =>
            await issueService.GetIssueByIdOrNullAsync(issueId) switch
            {
                null      => IssueNotFound(),
                var issue => Ok((await issueService.GetIssueHistoryAsync(issue)).Select(i => i.ToDto()))
            };

        private ActionResult IssueNotFound() => NotFound("Issue not found", "issue.notFound");

        [HttpPost("{issueId:long}/inProgress")]
        public async Task<ActionResult<ApiResponse<PersistedIssueDto>>> MoveIssueToInProgress([FromRoute] long issueId,
            [FromBody] MoveIssueToInPgoressDto data)
        {
            if (!currentUserProvider.IsSupervisor())
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

            var category = await categoriesService.GetByIdOrNullAsync(data.CategoryId);
            if (category == null)
            {
                return CategoryNotFound();
            }

            var newState =
                await issueService.TakeToWorkAsync(issue, currentUserProvider.CurrentUser, worker, category);
            return Ok(newState.ToDto());
        }

        private static ActionResult<ApiResponse<PersistedIssueDto>> CategoryNotFound() =>
            BadRequest("Category or area not found", "categories.notFound");

        private static ActionResult<ApiResponse<PersistedIssueDto>> WorkerNotFound() =>
            BadRequest("Worker not found", "workers.notFound");
    }
}
