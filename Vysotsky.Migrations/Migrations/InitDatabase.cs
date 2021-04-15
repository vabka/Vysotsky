using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Init", 2021, 04, 09, 14, 21)]
    public class InitDatabase : Migration
    {
        public override void Up()
        {
            this.Create.Entity("user");
            this.Create.Entity("organization")
                .WithColumn("name").AsString();
            this.Create.Entity("image")
                .WithColumn("external_id").AsString();

            this.Create.Entity("area")
                .WithColumn("name").AsString()
                .WithColumn("image_id").References("image");
            this.Create.Entity("category")
                .WithColumn("area_id").References("area")
                .WithColumn("name").AsString();

            this.Create.Entity("building")
                .WithColumn("name").AsString();
            this.Create.Entity("floor")
                .WithColumn("number").AsString()
                .WithColumn("building_id").References("building");

            this.Execute.CreateEnum("room_status", "Free", "Owned", "Rented", "Unavailable");
            this.Create.Entity("room")
                .WithColumn("floor_id").References("floor")
                .WithColumn("owner_id").References("organization").Nullable()
                .WithColumn("status").AsEnum("room_status")
                .WithColumn("name").AsString().Nullable()
                .WithColumn("number").AsString().Nullable();
            this.Create.Index()
                .OnTable("room")
                .OnColumn("owner_id");
            this.Execute.CreateEnum("user_role",
                "SuperUser",
                "Supervisor",
                "Worker",
                "OrganizationOwner",
                "OrganizationMember");
            this.Alter.Table("user").AddColumn("username").AsString().Unique()
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

            this.Create.Table("blocked_token")
                .WithColumn("jti").AsGuid().PrimaryKey()
                .WithColumn("expiration_time").AsDateTime();


            this.Execute.CreateEnum("issue_status",
                "New",
                "Cancelled",
                "NeedInfo",
                "Rejected",
                "InProgress",
                "Completed",
                "Accepted",
                "Closed");
            this.Create.VersionedEntity("issue")
                .WithColumn("status").AsEnum("issue_status")
                .WithColumn("title").AsString()
                .WithColumn("description").AsString()
                .WithColumn("note").AsString()
                .WithColumn("area_id").References("area")
                .WithColumn("category_id").References("category").Nullable()
                .WithColumn("author_id").References("user")
                .WithColumn("supervisor_id").References("user").Nullable()
                .WithColumn("worker_id").References("user").Nullable()
                .WithColumn("room_id").References("room");
            this.Create.Table("issue_image")
                .WithColumn("issue_id").References("issue").PrimaryKey()
                .WithColumn("image_id").References("image").PrimaryKey();

            this.Create.Entity("issue_comment")
                .WithColumn("issue_id").References("issue")
                .WithColumn("author_id").References("user")
                .WithColumn("text").AsString();
            this.Create.Table("issue_comment_image")
                .WithColumn("issue_comment_id").References("issue_comment")
                .WithColumn("image_id").References("image");
        }

        public override void Down()
        {
            this.Delete.Table("image");
            this.Delete.Table("area");
            this.Delete.Table("category");
            this.Delete.Table("building");
            this.Delete.Table("floor");
            this.Delete.Table("room");
            this.Execute.DropEnum("room_status");
            this.Delete.Table("user");
            this.Execute.DropEnum("user_role");
            this.Delete.Table("blocked_token");
            this.Delete.Table("organization");
            this.Delete.Table("issue");
            this.Execute.DropEnum("issue_status");
            this.Delete.Table("issue_image");
            this.Delete.Table("issue_comment");
            this.Delete.Table("issue_comment_image");
        }
    }
}
