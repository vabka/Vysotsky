using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Init", 2021, 04, 09, 14, 21)]
    public class InitDatabase : Migration
    {
        public override void Up()
        {
            Create.Entity("user");
            Create.Entity("organization")
                .WithColumn("name").AsString()
                .WithColumn("owner_id").References("user");
            Create.Entity("image")
                .WithColumn("external_id").AsString();

            Create.Entity("area")
                .WithColumn("name").AsString()
                .WithColumn("image_id").References("image");
            Create.Entity("category")
                .WithColumn("area_id").References("area");

            Create.Entity("building")
                .WithColumn("name").AsString();
            Create.Entity("floor")
                .WithColumn("number").AsString()
                .WithColumn("building_id").References("building");

            Execute.CreateEnum("room_status", "Free", "Owned", "Rented", "Unavailable");
            Create.Entity("room")
                .WithColumn("floor_id").References("floor")
                .WithColumn("owner_id").References("organization").Nullable()
                .WithColumn("status").AsEnum("room_status")
                .WithColumn("name").AsString().Nullable()
                .WithColumn("number").AsString().Nullable();

            Execute.CreateEnum("user_role",
                "SuperUser",
                "Supervisor",
                "Worker",
                "OrganizationOwner",
                "OrganizationMember");
            Alter.Table("user").AddColumn("username").AsString().Unique()
                .AddColumn("password_hash")
                .AsBinary(512 / 8)
                .AddColumn("image_id").References("image").Nullable()
                .AddColumn("firstname").AsString()
                .AddColumn("lastname").AsString()
                .AddColumn("patronymic").AsString().Nullable()
                .AddColumn("contacts").AsJsonb()
                .AddColumn("role").AsEnum("user_role")
                .AddColumn("organization_id").References("organization").Nullable()
                .WithColumnDescription("Only for members of organization");

            Create.Entity("blocked_token")
                .WithColumn("expiration_time").AsDateTime()
                .WithColumn("user_id").References("user")
                .WithColumn("value").AsString().Unique();

            Execute.CreateEnum("issue_status",
                "New",
                "Cancelled",
                "NeedInfo",
                "Rejected",
                "InProgress",
                "Completed",
                "Accepted",
                "Closed");
            Create.Entity("issue")
                .WithColumn("status").AsEnum("issue_status")
                .WithColumn("title").AsString()
                .WithColumn("description").AsString()
                .WithColumn("note").AsString()
                .WithColumn("area_id").References("area")
                .WithColumn("category_id").References("category").Nullable()
                .WithColumn("author_id").References("user")
                .WithColumn("supervisor_id").References("user")
                .WithColumn("worker_id").References("user")
                .WithColumn("room_id").References("room");
            Create.Table("issue_image")
                .WithColumn("issue_id").References("issue").PrimaryKey()
                .WithColumn("image_id").References("image").PrimaryKey();

            Create.Entity("issue_comment")
                .WithColumn("issue_id").References("issue")
                .WithColumn("author_id").References("user")
                .WithColumn("text").AsString();
            Create.Table("issue_comment_image")
                .WithColumn("issue_comment_id").References("issue_comment")
                .WithColumn("image_id").References("image");

            Execute.CreateEnum("issue_event",
                "StatusChanged",
                "SupervisorChanged",
                "WorkerChanged",
                "DescriptionChanged",
                "CommentAdded");
            Create.Entity("issue_history")
                .WithColumn("issue_id").References("issue")
                .WithColumn("event").AsEnum("issue_event")
                .WithColumn("extension").AsJsonb();
        }

        public override void Down()
        {
            Delete.Table("image");
            Delete.Table("area");
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
            Delete.Table("issue_history");
            Execute.DropEnum("issue_event");
        }
    }
}
