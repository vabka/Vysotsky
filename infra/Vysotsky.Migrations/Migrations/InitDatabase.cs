using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Init", 2021, 04, 09, 14, 21)]
    public class InitDatabase : Migration
    {
        public override void Up()
        {
            Create.SortableEntity("user");
            Create.SortableEntity("organization")
                .WithColumn("name").AsString();

            Create.EntityWithId("image")
                .WithColumn("external_id").AsString();

            Create.SortableEntity("category")
                .WithColumn("image_id").References("image")
                .WithColumn("name").AsString();

            Create.SortableEntity("building")
                .WithColumn("name").AsString();

            Create.SortableEntity("floor")
                .WithColumn("number").AsString()
                .WithColumn("building_id").References("building");

            Execute.CreateEnum("room_status", "Free", "Owned", "Rented", "Unavailable");

            Create.SortableEntity("room")
                .WithColumn("floor_id").References("floor")
                .WithColumn("owner_id").References("organization").Nullable()
                .WithColumn("status").AsEnum("room_status")
                .WithColumn("number").AsString().Nullable();

            Create.Index()
                .OnTable("room")
                .OnColumn("owner_id");

            Execute.CreateEnum("user_role",
                "SuperUser",
                "Supervisor",
                "Worker",
                "OrganizationOwner",
                "OrganizationMember");

            Alter.Table("user")
                .AddColumn("username").AsString().Unique()
                .AddColumn("password_hash")
                .AsBinary(512 / 8)
                .AddColumn("image_id").References("image").Nullable()
                .AddColumn("firstname").AsString()
                .AddColumn("lastname").AsString()
                .AddColumn("patronymic").AsString().Nullable()
                .AddColumn("contacts").AsJsonb()
                .AddColumn("role").AsEnum("user_role")
                .AddColumn("last_password_change").AsDateTimeOffset()
                .AddColumn("organization_id").References("organization").Nullable();

            Create.Table("blocked_token")
                .WithColumn("jti").AsGuid().PrimaryKey()
                .WithColumn("expiration_time").AsDateTime();


            Execute.CreateEnum("issue_status",
                "New",
                "Cancelled",
                "NeedInfo",
                "Rejected",
                "InProgress",
                "Completed",
                "Accepted",
                "Closed");

            Create.VersionedEntity("issue")
                .WithColumn("status").AsEnum("issue_status")
                .WithColumn("title").AsString()
                .WithColumn("description").AsString()
                .WithColumn("note").AsString()
                .WithColumn("category_id").References("category")
                .WithColumn("author_id").References("user")
                .WithColumn("supervisor_id").References("user").Nullable()
                .WithColumn("worker_id").References("user").Nullable()
                .WithColumn("room_id").References("room")
                .WithColumn("author_has_unread_comments").AsBoolean();

            Create.Table("issue_image")
                .WithColumn("issue_id").AsInt64().PrimaryKey() // References(issue)
                .WithColumn("image_id").References("image").PrimaryKey();

            Create.SortableEntity("issue_comment")
                .WithColumn("issue_id").AsInt64() // References(issue)
                .WithColumn("author_id").References("user")
                .WithColumn("text").AsString();

            Create.Table("issue_comment_image")
                .WithColumn("issue_comment_id").References("issue_comment")
                .WithColumn("image_id").References("image");
        }

        public override void Down()
        {
            Delete.Table("image");
            Delete.Table("category");
            Delete.Table("building");
            Delete.Table("floor");
            Delete.Table("room");
            Execute.DropEnum("room_status");
            Delete.Table("user");
            Execute.DropEnum("user_role");
            Delete.Table("blocked_token");
            Delete.Table("organization");
            Delete.Table("issue");
            Execute.DropEnum("issue_status");
            Delete.Table("issue_image");
            Delete.Table("issue_comment");
            Delete.Table("issue_comment_image");
        }
    }
}
