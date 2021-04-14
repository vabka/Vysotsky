using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API.Controllers.Issues
{
    [Microsoft.AspNetCore.Components.Route(Resources.Issues)]
    public class IssuesController : ApiController
    {
        [HttpPost]
        public Task<ActionResult<ApiResponse<PersistedIssue>>> CreateIssue()
        {
            throw new NotImplementedException();
        }
    }

    public class PersistedIssue
    {
    }
}