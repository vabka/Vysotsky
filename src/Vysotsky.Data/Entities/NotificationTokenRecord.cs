using LinqToDB.Mapping;
using Vysotsky.Data.Entities.Abstraction;

namespace Vysotsky.Data.Entities
{
    [Table("notification_token")]
    public class NotificationTokenRecord : Entity
    {
        [Column("user_id")]
        public long UserId { get; init; }
        [Column("token")]
        public string Token { get; init; } = null!;
        [Column("platform", DbType = "notification_platforms")]
        public NotificationPlatform Platform { get; init; }
    }
}
