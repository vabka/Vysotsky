using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Add read status", 2021, 04, 19, 20, 08)]
    public class AddReadStatusForChatMessages : Migration
    {
        public override void Up()
        {
            Execute.CreateEnum("chat_message_status", "Sent", "Read");
            Alter.Table("support_chat_message")
                .AddColumn("status").AsEnum("chat_message_status");
        }

        public override void Down()
        {
            Delete.Column("status").FromTable("support_chat_message");
            Execute.DropEnum("chat_message_status");
        }
    }
}
