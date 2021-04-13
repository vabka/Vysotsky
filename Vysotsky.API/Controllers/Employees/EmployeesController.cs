using System;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Common;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API.Controllers.Employees
{
    /// <summary>
    /// Контроллер сотрудников УК
    /// </summary>
    [Route(Resources.Employees)]
    public class EmployeesController : ApiController
    {
        [HttpGet]
        public ActionResult<ApiResponse<EmployeeDto[]>> GetAllEmployees()
        {
            throw new NotImplementedException();
        }
    }

    public class EmployeeDto
    {
    }
}
