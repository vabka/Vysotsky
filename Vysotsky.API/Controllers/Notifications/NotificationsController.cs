using System;
using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API.Controllers.Notifications
{
    /// <summary>
    /// Контроллер центра уведомлений
    /// </summary>
    [Route(Resources.Notifications)]
    public class NotificationsController : ApiController
    {
        /// <summary>
        /// Получить уведомления для текущего пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetNotifications()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Создать глобальное уведомление
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BroadcastNotification()
        {
            throw new NotImplementedException();
        }
    }
}