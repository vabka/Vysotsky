using System;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Infrastructure;
using Vysotsky.API.Models;

namespace Vysotsky.API.Controllers.Customers
{
    /// <summary>
    /// Контроллер клиентов
    /// </summary>
    [Route(Resources.Customers)]
    public class CustomersController : ApiController
    {
        /// <summary>
        /// Получить клиента по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<Customer>> GetCustomer(int id)
        {
            throw new NotImplementedException();
        }
    }
}