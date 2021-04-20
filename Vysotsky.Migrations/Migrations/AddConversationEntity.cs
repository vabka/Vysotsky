using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Add conversations", 2021, 04, 20, 14, 02)]
    public class AddConversationEntity : Migration
    {
        public override void Up() =>
            Create.SortableEntity("conversation")
                .WithColumn("has_unread_for_support").AsBoolean().Indexed()
                .WithColumn("has_unread_for_customer").AsBoolean().Indexed();

        public override void Down() =>
            Delete.Table("conversation");
    }
}
