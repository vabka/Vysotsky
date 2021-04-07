using System;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Controllers.Users;
using Vysotsky.API.Infrastructure;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers.Employees
{
    /// <summary>
    /// Контроллер сотрудников УК
    /// </summary>
    [Route(Resources.Employees)]
    public class EmployeeController : ApiController
    {
        /// <summary>
        /// Получить сотрудника УК по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<Employee>> GetEmployee(int id)
        {
            throw new NotImplementedException();
        }
    }
}