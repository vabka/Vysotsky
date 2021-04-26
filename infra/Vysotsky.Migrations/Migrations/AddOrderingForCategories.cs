using FluentMigrator;

namespace Vysotsky.Migrations.Migrations
{
    [CustomMigration("Специальное поле для сортировки категорий", 2021, 04, 23, 12, 48)]
    public class AddOrderingForCategories : Migration
    {
        public override void Up() =>
            Alter.Table("category").AddColumn("order").AsInt32().Nullable();

        public override void Down() => Delete.Column("order").FromTable("category");
    }
}
