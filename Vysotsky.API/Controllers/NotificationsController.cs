using Microsoft.AspNetCore.Mvc;
using Vysotsky.API.Dto.Common;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API.Controllers
{
    [Route(Resources.Notifications)]
    public class NotificationsController : ApiController
    {
        [HttpPost("{platform}")]
        public ActionResult<ApiResponse> SubscribeOnPlatform([FromRoute] NotificationPlatformDto platform,
            [FromBody] NotificationKeyDto notificationKey) =>
            Ok();
    }
}
