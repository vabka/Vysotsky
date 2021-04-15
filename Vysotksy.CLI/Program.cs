using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Impl;

var hasher = new SecureHasher(new SecureHasherOptions
{
    Salt = Environment.GetEnvironmentVariable("SALT") ?? "ZWFzeVNhbHQ="
});
var connectionString = Environment.GetEnvironmentVariable("PG_CONNECTION_STRING") ??
                       throw new InvalidOperationException("PG_CONNECTION_STRING is not set");
var options = new LinqToDbConnectionOptionsBuilder()
    .UsePostgreSQL(connectionString)
    .WithTraceLevel(TraceLevel.Info)
    .Build<VysotskyDataConnection>();
await using var db = new VysotskyDataConnection(options);
await Seed(db);

async Task Seed(VysotskyDataConnection dataConnection)
{
    await dataConnection.Users.InsertAsync(() => new UserRecord
    {
        Username = "admin",
        PasswordHash = hasher.Hash("admin"),
        LastPasswordChange = DateTimeOffset.Now,
        Role = UserRole.SuperUser,
        FirstName = "Админ",
        LastName = "Админов",
        Patronymic = "Админович",
        Contacts = new[]
        {
            new UserContact
            {
                Name = "Телефон, чтобы ночью звонить", Value = "88005553535", Type = ContactType.Phone
            }
        }
    });
    var buildingId =
        await dataConnection.Buildings.InsertWithInt64IdentityAsync(() => new BuildingRecord { Name = "БЦ Высоцкий" });
    await CreateManyRooms(buildingId, 1, dataConnection);
    await CreateManyRooms(buildingId, 2, dataConnection);
    await CreateManyRooms(buildingId, 3, dataConnection);
    var floor4 = await dataConnection.Floors.InsertWithInt64IdentityAsync(() => new FloorRecord
    {
        Number = "4",
        BuildingId = buildingId
    });
    var floor5 = await dataConnection.Floors.InsertWithInt64IdentityAsync(() => new FloorRecord
    {
        Number = "5",
        BuildingId = buildingId
    });
    await CreateManyRooms(buildingId, 6, dataConnection);
    await CreateManyRooms(buildingId, 7, dataConnection);
    await CreateManyRooms(buildingId, 8, dataConnection);
    await CreateManyRooms(buildingId, 9, dataConnection);
    var orgId = await dataConnection.Organizations.InsertWithInt64IdentityAsync(() => new OrganizationRecord
    {
        Name = "Холдинг БП",
    });

    await dataConnection.Rooms.InsertAsync(() => new RoomRecord
    {
        Status = RoomStatus.Rented,
        Number = "500",
        Name = "Internal Server Error",
        FloorId = floor5,
        OwnerId = orgId
    });
    var room404 = await dataConnection.Rooms.InsertWithInt64IdentityAsync(() => new RoomRecord
    {
        Status = RoomStatus.Rented,
        Number = "404",
        Name = "Not Found",
        FloorId = floor4,
        OwnerId = orgId
    });
    await dataConnection.Rooms.InsertAsync(() => new RoomRecord
    {
        Status = RoomStatus.Free,
        Name = "Bad Request",
        Number = "400",
        FloorId = floor4
    });

    await dataConnection.Users.InsertAsync(() => new UserRecord
    {
        Username = "customer",
        PasswordHash = hasher.Hash("customer"),
        FirstName = "Иван",
        LastName = "Иванов",
        Role = UserRole.OrganizationOwner,
        OrganizationId = orgId,
        LastPasswordChange = DateTimeOffset.Now,
        Contacts = Array.Empty<UserContact>()
    });
    var representative = await dataConnection.Users.InsertWithInt64IdentityAsync(() => new UserRecord
    {
        Username = "representative",
        PasswordHash = hasher.Hash("representative"),
        FirstName = "Пётр",
        LastName = "Петров",
        Role = UserRole.OrganizationMember,
        OrganizationId = orgId,
        LastPasswordChange = DateTimeOffset.Now,
        Contacts = Array.Empty<UserContact>()
    });
    await dataConnection.Users.InsertWithInt64IdentityAsync(() => new UserRecord
    {
        Username = "supervisor",
        PasswordHash = hasher.Hash("supervisor"),
        FirstName = "Диспетчер",
        LastName = "Андреев",
        Role = UserRole.Supervisor,
        LastPasswordChange = DateTimeOffset.Now,
        Contacts = Array.Empty<UserContact>()
    });
    await dataConnection.Users.InsertWithInt64IdentityAsync(() => new UserRecord
    {
        Username = "worker",
        PasswordHash = hasher.Hash("worker"),
        FirstName = "Работяга",
        LastName = "СЗавода",
        Role = UserRole.Worker,
        LastPasswordChange = DateTimeOffset.Now,
        Contacts = Array.Empty<UserContact>()
    });
    var electrical = await dataConnection.Images.InsertWithInt64IdentityAsync(() => new ImageRecord
    {
        ExternalId = "icon://light_bulb"
    });
    var area = await dataConnection.Areas.InsertWithInt64IdentityAsync(() => new AreaRecord
    {
        Name = "Электрика",
        ImageId = electrical
    });
    await dataConnection.Categories.InsertWithInt64IdentityAsync(() => new CategoryRecord
    {
        Name = "Лампочку заменить",
        AreaId = area
    });
    await dataConnection.Issues.InsertWithInt64IdentityAsync(() => new IssueRecord
    {
        Status = IssueStatus.New,
        AreaId = area,
        Description = "Хз почему не светит",
        Note = "",
        RoomId = room404,
        AuthorId = representative,
        Title = "Лампочка не светит"
    });
}

async Task CreateManyRooms(long building, int floorNumber, VysotskyDataConnection dataConnection)
{
    var floor = await dataConnection.Floors.InsertWithInt64IdentityAsync(() => new FloorRecord
    {
        BuildingId = building,
        Number = floorNumber.ToString(CultureInfo.InvariantCulture),
    });
      await dataConnection.Rooms.BulkCopyAsync(new BulkCopyOptions(), Enumerable.Range(0, 100).Select(i =>
        new RoomRecord { FloorId = floor, Number = ((floorNumber * 100) + i).ToString(CultureInfo.InvariantCulture), Status = RoomStatus.Free }));
}
