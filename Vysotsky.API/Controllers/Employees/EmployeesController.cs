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
        /// <summary>
        /// Получить сотрудника УК по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse> GetEmployee(int id)
        {
            throw new NotImplementedException();
        }
    }
}