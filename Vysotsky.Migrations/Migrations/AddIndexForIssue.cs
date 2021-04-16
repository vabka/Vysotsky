using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Add index for issues", 2021, 04, 16, 17, 24)]
    public class AddIndexForIssue : Migration
    {
        public override void Up()
        {
            Create.Index("IX_issue_created_at_id_version")
                .OnTable("issue")
                .OnColumn("created_at").Descending()
                .OnColumn("id").Descending();

            Create.Index("IX_issue_id_version")
                .OnTable("issue")
                .OnColumn("id").Ascending()
                .OnColumn("version").Descending();
        }

        public override void Down()
        {
            Delete.Index("IX_issue_created_at_id_version");
            Delete.Index("IX_issue_id_version");
        }
    }
}
