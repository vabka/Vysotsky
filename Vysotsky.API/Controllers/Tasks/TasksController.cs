using System;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API.Controllers.Tasks
{
    /// <summary>
    /// Контроллер заявок
    /// </summary>
    [Route(Resources.Tasks)]
    public class TasksController : ApiController
    {
        /// <summary>
        /// Получить данные по заявке
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpGet("{taskId}")]
        public ActionResult GetTask(int taskId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить комментарии по заявке
        /// </summary>
        /// <param name="taskId">Идентификатор заявки</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("{taskId}/comments")]
        public ActionResult GetComments(int taskId)
        {
            throw new NotImplementedException();
        }
    }
}