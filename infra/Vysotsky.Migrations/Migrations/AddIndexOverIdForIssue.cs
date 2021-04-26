using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Add index over id-column in issue", 2021, 04, 19, 19, 30)]
    public class AddIndexOverIdForIssue : Migration
    {
        public override void Up() => Create.Index("IX_issue_id").OnTable("issue").OnColumn("id").Ascending();

        public override void Down() => Delete.Index("IX_issue_id");
    }
}
