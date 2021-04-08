using LinqToDB;

namespace Vysotsky.Data
{
    public class Database : DataContext
    {
        public ITable<User> Users => this.GetTable<User>();
        public ITable<Token> Tokens => this.GetTable<Token>();
        public ITable<Organization> Organizations => this.GetTable<Organization>();

        public ITable<Building> Buildings => this.GetTable<Building>();
        public ITable<Floor> Floors => this.GetTable<Floor>();
        public ITable<Room> Rooms => this.GetTable<Room>();

        public ITable<Image> Images => this.GetTable<Image>();


        public ITable<Issue> Issues => this.GetTable<Issue>();
        public ITable<Category> Categories => this.GetTable<Category>();
        public ITable<Area> Areas => this.GetTable<Area>();
        public ITable<IssueHistoryRecord> IssueHistory => this.GetTable<IssueHistoryRecord>();
        

    }
}
