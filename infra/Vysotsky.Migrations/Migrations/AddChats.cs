using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Add chats", 2021, 04, 19, 14, 00)]
    public class AddChats : Migration
    {
        public override void Up() =>
            Create.SortableEntity("support_chat_message")
                .WithColumn("client_user_id").References("user")
                .WithColumn("author").References("user")
                .WithColumn("content").AsString();

        public override void Down() =>
            Delete.Table("support_chat_message");
    }
}
