using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("conversation")]
    public class ConversationRecord : SortableEntity
    {
        [Column("has_unread_for_customer")] public bool HasUnreadForCustomer { get; init; }
        [Column("has_unread_for_support")] public bool HasUnreadForSupport { get; init; }
    }
}
