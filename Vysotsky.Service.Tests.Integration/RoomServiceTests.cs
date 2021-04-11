using System.Threading.Tasks;
using FluentAssertions;
using LinqToDB;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Impl;
using Xunit;

namespace Vysotsky.Service.Tests.Integration
{
    public class RoomServiceTests : DatabaseTests
    {
        private readonly RoomService _roomService;

        public RoomServiceTests()
        {
            _roomService = new RoomService(Database);
        }

        [Fact]
        public async Task CreateBuildingSuccessfully()
        {
            var building = await _roomService.CreateBuildingAsync("Высоцкий");
            var buildings = await Database.Buildings.ToArrayAsync();

            buildings.Length.Should().Be(1);
            var vysotsky = buildings[0];
            building.Name.Should().Be("Высоцкий");
            vysotsky.Name.Should().Be("Высоцкий");
            vysotsky.Id.Should().Be(building.Id);
        }

        [Fact]
        public async Task GetSingleBuildingSuccessfully()
        {
            var id = await CreateBuilding("A");
            var buildings = await _roomService.GetAllBuildingsAsync();
            buildings.Length.Should().Be(1);
            buildings[0].Id.Should().Be(id);
            buildings[0].Name.Should().Be("A");
        }

        [Fact]
        public async Task GetNoBuildingsSuccessfully()
        {
            var buildings = await _roomService.GetAllBuildingsAsync();
            buildings.Length.Should().Be(0);
        }

        [Fact]
        public async Task GetManyBuildingssuccessfully()
        {
            var id1 = await CreateBuilding("A");
            var id2 = await CreateBuilding("B");
            var buildings = await _roomService.GetAllBuildingsAsync();
            buildings.Length.Should().Be(2);

            buildings[0].Id.Should().Be(id1);
            buildings[0].Name.Should().Be("A");
            buildings[1].Id.Should().Be(id2);
            buildings[1].Name.Should().Be("B");
        }

        private async Task<long> CreateBuilding(string name)
        {
            var id = await Database.Buildings.InsertWithInt64IdentityAsync(() => new BuildingRecord
            {
                Name = name
            });
            return id;
        }
    }
}
