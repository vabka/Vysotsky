using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Dto.Issues;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API.Controllers.Issues
{
    [Route(Resources.Issues)]
    public class IssuesController : ApiController
    {
        [HttpPost]
        public Task<ActionResult<ApiResponse<PersistedIssue>>> CreateIssue()
        {
            throw new NotImplementedException();
        }
    }
}