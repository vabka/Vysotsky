using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("support_chat_message")]
    public class SupportChatMessageRecord : SortableEntity
    {
        [Column("client_user_id")] public long UserId { get; init; }
        [Column("author")] public long AuthorId { get; init; }

        [Column("content")] public string Content { get; init; } = "";
    }
}
