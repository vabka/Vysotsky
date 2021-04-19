using LinqToDB.Mapping;

namespace Vysotsky.Data.Entities
{
    public enum NotificationPlatform
    {
        [MapValue("SignalR")] SignalR,
        [MapValue("Firebase")] Firebase
    }
}