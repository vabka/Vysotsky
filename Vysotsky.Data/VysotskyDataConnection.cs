using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using Npgsql;
using Npgsql.NameTranslation;
using Npgsql.TypeHandlers;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using Vysotsky.Data.Entities;

namespace Vysotsky.Data
{
    public class VysotskyDataConnection : DataConnection
    {
        static VysotskyDataConnection()
        {
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                Converters = {new JsonStringEnumConverter()}
            };
            NpgsqlConnection.GlobalTypeMapper
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "jsonb",
                    NpgsqlDbType = NpgsqlDbType.Jsonb,
                    ClrTypes = null,
                    TypeHandlerFactory = new JsonbHandlerFactory(jsonSerializerOptions)
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "json",
                    NpgsqlDbType = NpgsqlDbType.Json,
                    ClrTypes = null,
                    TypeHandlerFactory = new JsonHandlerFactory(jsonSerializerOptions)
                }.Build())
                .MapEnum<IssueEvent>("issue_event", new NpgsqlNullNameTranslator())
                .MapEnum<UserRole>("user_role", new NpgsqlNullNameTranslator())
                .MapEnum<IssueStatus>("issue_status", new NpgsqlNullNameTranslator())
                .MapEnum<RoomStatus>("room_status", new NpgsqlNullNameTranslator());
        }

        public VysotskyDataConnection(LinqToDbConnectionOptions<VysotskyDataConnection> options) : base(options)
        {
        }

        public ITable<User> Users => GetTable<User>();
        public ITable<BlockedToken> BlockedTokens => GetTable<BlockedToken>();
        public ITable<Organization> Organizations => GetTable<Organization>();
        public ITable<Building> Buildings => GetTable<Building>();
        public ITable<Floor> Floors => GetTable<Floor>();
        public ITable<Room> Rooms => GetTable<Room>();
        public ITable<Image> Images => GetTable<Image>();
        public ITable<Issue> Issues => GetTable<Issue>();
        public ITable<IssueImageRelation> IssueImages => GetTable<IssueImageRelation>();
        public ITable<IssueComment> IssueComments => GetTable<IssueComment>();
        public ITable<IssueCommentImageRelation> IssueCommentImages => GetTable<IssueCommentImageRelation>();
        public ITable<Category> Categories => GetTable<Category>();
        public ITable<Area> Areas => GetTable<Area>();
        public ITable<IssueHistoryRecord> IssueHistory => GetTable<IssueHistoryRecord>();
    }
}
