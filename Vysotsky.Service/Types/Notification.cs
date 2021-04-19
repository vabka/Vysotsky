using System.Collections.Generic;

namespace Vysotsky.Service.Types
{
    public class Notification
    {
        public NotificationCategory Category { get; init; }
        public string Title { get; init; } = null!;
        public string Body { get; init; } = null!;
        public string Image { get; init; } = null!;
        public Dictionary<string, string> Payload { get; } = new();
    }
}