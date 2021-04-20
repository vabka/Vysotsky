namespace Vysotsky.Service.Types
{
    public class Conversation
    {
        public long AttachedUserId { get; init; }
        public bool HasUnreadMessages { get; init; }
    }
}
