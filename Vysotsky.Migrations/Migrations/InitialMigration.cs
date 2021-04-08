using System.Linq;
using FluentMigrator;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Execute;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Init", 2021, 04, 09, 14, 21)]
    public class InitialMigration : Migration
    {
        public override void Up()
        {
            Create.Entity("image")
                .WithColumn("external_id").AsString();

            Create.Entity("area")
                .WithColumn("name").AsString()
                .WithReference("image_id", "image");
            Create.Entity("category")
                .WithReference("area_id", "area");

            Create.Entity("building")
                .WithColumn("name").AsString();
            Create.Entity("floor")
                .WithColumn("name").AsString().Nullable()
                .WithReference("building_id", "building");

            Execute.CreateEnum("room_status", "Free", "Owned", "Rented", "Unavailable");
            Create.Entity("room")
                .WithReference("floor_id", "floor")
                .WithReference("owner_id", "organization").Nullable()
                .WithColumn("status").AsEnum("room_status");


            Execute.CreateEnum("user_role",
                "SuperUser",
                "Supervisor",
                "Worker",
                "OrganizationOwner",
                "OrganizationMember");
            Create.Entity("user")
                .WithColumn("username").AsString()
                .WithColumn("password_hash").AsBinary(512 / 8)
                .WithReference("image_id", "image").Nullable()
                .WithColumn("firstname").AsString()
                .WithColumn("lastname").AsString()
                .WithColumn("patronymic").AsString().Nullable()
                .WithColumn("contacts").AsJsonb()
                .WithColumn("role").AsEnum("user_role")
                .WithReference("organization_id", "organization").Nullable()
                .WithColumnDescription("Only for members of organization");
            Create.Entity("token")
                .WithReference("user_id", "user")
                .WithColumn("value").AsString();

            Create.Entity("organization")
                .WithColumn("name").AsString()
                .WithReference("owner_id", "user");

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
                .WithReference("area_id", "area")
                .WithReference("category_id", "category").Nullable()
                .WithReference("author_id", "user")
                .WithReference("supervisor_id", "user")
                .WithReference("worker_id", "user")
                .WithReference("room_id", "room");
            Create.Table("issue_image")
                .WithReference("issue_id", "issue").PrimaryKey()
                .WithReference("image_id", "image").PrimaryKey();

            Create.Entity("issue_comment")
                .WithReference("issue_id", "issue")
                .WithReference("author_id", "user")
                .WithColumn("text").AsString();
            Create.Table("issue_comment_image")
                .WithReference("issue_comment_id", "issue_comment")
                .WithReference("image_id", "image");

            Execute.CreateEnum("issue_event",
                "StatusChanged",
                "SupervisorChanged",
                "WorkerChanged",
                "DescriptionChanged",
                "CommentAdded");
            Create.Entity("issue_history")
                .WithReference("issue_id", "issue")
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
            Delete.Table("token");
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

    public class CustomMigrationAttribute : MigrationAttribute
    {
        public CustomMigrationAttribute(string description, int year, int month, int day, int hour, int minute) : base(
            CalculateValue(year, month, day, hour, minute), TransactionBehavior.Default,
            description)
        {
        }

        private static long CalculateValue(int year, int month, int day, int hour, int minute) =>
            year * 100000000L + month * 1000000L + day * 10000L + hour * 100L +
            minute;
    }

    public static class MigrationExtensions
    {
        public static ICreateTableWithColumnSyntax Entity(this ICreateExpressionRoot create, string name) =>
            create.Table(name)
                .WithColumn("id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                .WithColumn("created_at")
                .AsDateTime();

        public static void CreateEnum(this IExecuteExpressionRoot execute, string name, params string[] values) =>
            execute.Sql($"CREATE TYPE {name} as ENUM (${string.Join(", ", values.Select(v => $"'{v}'"))});");

        public static void DropEnum(this IExecuteExpressionRoot execute, string name) =>
            execute.Sql($"DROP TYPE {name};");

        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithReference(
            this ICreateTableWithColumnSyntax table, string name,
            string primaryTable) =>
            table.WithColumn(name)
                .AsInt64()
                .ForeignKey(primaryTable, "id");

        public static ICreateTableColumnOptionOrWithColumnSyntax AsJsonb(this ICreateTableColumnAsTypeSyntax column) =>
            column.AsCustom("jsonb");

        public static ICreateTableColumnOptionOrWithColumnSyntax AsEnum(this ICreateTableColumnAsTypeSyntax column,
            string enumName) =>
            column.AsCustom(enumName);
    }
}
