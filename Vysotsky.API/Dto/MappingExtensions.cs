using System;
using System.Linq;
using Vysotsky.API.Dto.Buildings;
using Vysotsky.API.Dto.Categories;
using Vysotsky.API.Dto.Images;
using Vysotsky.API.Dto.Issues;
using Vysotsky.API.Dto.Organizations;
using Vysotsky.API.Dto.Users;
using Vysotsky.Data.Entities;
using Vysotsky.Service.Interfaces;
using Vysotsky.Service.Types;

namespace Vysotsky.API.Dto
{
    public static class MappingExtensions
    {
        public static PersistedImageDto ToDto(this Image image) => new()
        {
            Id = image.Id
        };

        public static PersistedCategoryDto ToDto(this Category category) => new
            (category.Id, category.Name);

        public static PersistedAreaDto ToDto(this Area area) => new
            (area.Id, area.Name, area.Image.ToDto());

        public static PersistedIssueDto ToDto(this Issue issue) =>
            new()
            {
                Id = issue.Id
            };

        public static PersistedBuildingDto ToDto(this Building building) => new()
        {
            Id = building.Id,
            Name = building.Name
        };

        public static PersistedFloorDto ToDto(this Floor floor) => new()
        {
            Id = floor.Id,
            Number = floor.Number
        };

        public static PersistedRoomDto ToDto(this Room room) => new()
        {
            Id = room.Id,
            Name = room.Name,
            Number = room.Number,
            Status = room.Status switch
            {
                RoomStatus.Free => RoomStatusDto.Free,
                RoomStatus.Owned => RoomStatusDto.Owned,
                RoomStatus.Rented => RoomStatusDto.Rented,
                RoomStatus.Unavailable => RoomStatusDto.Unavalable,
                _ => throw new InvalidOperationException()
            }
        };

        public static OrganizationBuildingDto ToDto(this FullBuilding b) => new()
        {
            Id = b.Id,
            Name = b.Name,
            Floors = b.Floors.Select(f => new OrganizationFloorDto
            {
                Id = f.Id,
                Number = f.Number,
                Rooms = f.Rooms.Select(r => new OrganizationRoomDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Number = r.Number
                })
            })
        };

        public static PersistedUserDto ToDto(this User user) => new()
        {
            Id = user.Id,
            Name = new PersonName
            {
                FirstName = user.Firstname,
                LastName = user.LastName,
                Patronymic = user.Patronymic
            },
            Contacts = user.Contacts.Select(c => new UserContactDto
            {
                Name = c.Name,
                Type = c.Type.ToDto(),
                Value = c.Value
            }),
            OrganizationId = user.OrganizationId,
            Username = user.Username
        };

        public static UserContactTypeDto ToDto(this ContactType contactType) => contactType switch
        {
            ContactType.Phone => UserContactTypeDto.Phone,
            _ => throw new ArgumentOutOfRangeException(nameof(contactType), contactType, null)
        };

        public static PersistedOrganizationDto ToDto(this Organization organization) => new()
        {
            Id = organization.Id,
            Name = organization.Name
        };
    }
}