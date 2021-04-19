using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Add notifications", 2021, 04, 19, 13, 00)]
    public class AddNotifications : Migration
    {
        public override void Up()
        {
            Execute.CreateEnum("notification_platform", "Firebase");
            Create.Table("notification_token")
                .WithColumn("user_id").References("user").PrimaryKey()
                .WithColumn("token").AsString().PrimaryKey()
                .WithColumn("platform").AsEnum("notification_platform").PrimaryKey();
        }

        public override void Down()
        {
            Delete.Table("notification_token");
            Execute.DropEnum("notification_platform");
        }
    }
}
