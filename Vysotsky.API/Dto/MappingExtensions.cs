using System;
using System.Linq;
using Vysotsky.API.Dto.Auth;
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
        public static AccessTokenDto ToDto(this TokenContainer token) => new()
        {
            Token = token.Token, ExpiresAt = token.ExpiresAt
        };

        public static UserContact ToModel(this UserContactDto userContactDto) => new()
        {
            Name = userContactDto.Name, Type = userContactDto.Type.ToModel(), Value = userContactDto.Value
        };

        public static UserContactType ToModel(this UserContactTypeDto type) => type switch
        {
            UserContactTypeDto.Phone => UserContactType.Phone,
            _                        => throw new ArgumentOutOfRangeException(nameof(type))
        };

        public static PersistedImageDto ToDto(this Image image) => new() {Id = image.Id};

        public static ShortPersistedIssueDto ToDto(this ShortIssue issue) => new()
        {
            Id = issue.Id, Title = issue.Title, CreatedAt = issue.CreatedAt, Status = issue.Status.ToDto()
        };

        public static IssueStatusDto ToDto(this IssueStatus status) => status switch
        {
            IssueStatus.New                 => IssueStatusDto.New,
            IssueStatus.CancelledByCustomer => IssueStatusDto.Cancelled,
            IssueStatus.NeedInfo            => IssueStatusDto.NeedInfo,
            IssueStatus.Rejected            => IssueStatusDto.Rejected,
            IssueStatus.InProgress          => IssueStatusDto.InProgress,
            IssueStatus.Completed           => IssueStatusDto.InProgress,
            IssueStatus.Accepted            => IssueStatusDto.InProgress,
            IssueStatus.Closed              => IssueStatusDto.Done,
            _                               => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };

        public static PersistedCategoryDto ToDto(this Category category) => new()
        {
            Id = category.Id, Name = category.Name
        };

        public static PersistedAreaDto ToDto(this Area area) => new()
        {
            Id = area.Id, Image = area.Image.ToDto(), Name = area.Name
        };

        public static PersistedIssueDto ToDto(this FullIssue issue) =>
            new()
            {
                Id = issue.Id,
                Version = issue.Version,
                Description = issue.Description,
                Status = issue.Status.ToDto(),
                Title = issue.Title,
                Area = issue.Area.ToDto(),
                Room = issue.Room.ToDto(),
                Category = issue.Category?.ToDto()
            };

        public static PersistedBuildingDto ToDto(this Building building) => new()
        {
            Id = building.Id, Name = building.Name
        };

        public static PersistedFloorDto ToDto(this Floor floor) => new() {Id = floor.Id, Number = floor.Number};

        public static PersistedRoomDto ToDto(this Room room) => new()
        {
            Id = room.Id,
            Name = room.Name,
            Number = room.Number,
            Status = room.Status switch
            {
                RoomStatus.Free        => RoomStatusDto.Free,
                RoomStatus.Owned       => RoomStatusDto.Owned,
                RoomStatus.Rented      => RoomStatusDto.Rented,
                RoomStatus.Unavailable => RoomStatusDto.Unavalable,
                _                      => throw new InvalidOperationException()
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
                    Id = r.Id, Name = r.Name, Number = r.Number
                })
            })
        };

        public static PersistedUserDto ToDto(this User user) => new()
        {
            Id = user.Id,
            FirstName = user.Firstname,
            LastName = user.LastName,
            Patronymic = user.Patronymic,
            Contacts = user.Contacts.Select(c => new UserContactDto
            {
                Name = c.Name, Type = c.Type.ToDto(), Value = c.Value
            }),
            OrganizationId = user.OrganizationId,
            Username = user.Username
        };

        public static UserContactTypeDto ToDto(this UserContactType userContactType) => userContactType switch
        {
            UserContactType.Phone => UserContactTypeDto.Phone,
            _ => throw new ArgumentOutOfRangeException(nameof(userContactType), userContactType, null)
        };

        public static PersistedOrganizationDto ToDto(this Organization organization) => new()
        {
            Id = organization.Id, Name = organization.Name
        };
    }
}
