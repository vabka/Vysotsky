namespace Vysotsky.API.Dto.Chats
{
    public class ConversationDto
    {
        public long Id { get; init; }
        public long Counterpart { get; init; }
        public bool HasUnreadMessages { get; init; }
    }
}
