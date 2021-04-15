using FluentMigrator;

namespace Vysotsky.Migrations
{
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
}
