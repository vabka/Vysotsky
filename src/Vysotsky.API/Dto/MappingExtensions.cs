using System;
using System.Collections.Generic;
using System.Linq;
using Vysotsky.API.Dto.Auth;
using Vysotsky.API.Dto.Buildings;
using Vysotsky.API.Dto.Categories;
using Vysotsky.API.Dto.Chats;
using Vysotsky.API.Dto.Common;
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
        public static WrappedListDto<T> ToDto<T>(this IEnumerable<T> collection) =>
            new() {Data = collection};

        public static ConversationDto ToDto(this Conversation conversation) => new()
        {
            Id = conversation.AttachedUserId,
            Counterpart = conversation.AttachedUserId,
            HasUnreadMessages = conversation.HasUnreadMessages
        };

        public static MessageContent ToModel(this MessageContentDto content) => new() {Text = content.Text!};

        public static ChatMessageDto ToDto(this ChatMessage message) => new()
        {
            Id = message.Id,
            CreatedAt = message.CreatedAt,
            From = message.From,
            Content = message.Content.ToDto(),
            Status = message.Status.ToDto()
        };

        public static ChatMessageStatusDto ToDto(this ChatMessageStatus status) => status switch
        {
            ChatMessageStatus.Sent => ChatMessageStatusDto.Sent,
            ChatMessageStatus.Read => ChatMessageStatusDto.Read,
            _                      => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };

        public static MessageContentDto ToDto(this MessageContent content) => new() {Text = content.Text};

        public static AccessTokenDto ToDto(this TokenContainer token) => new()
        {
            Token = token.Token, ExpiresAt = token.ExpiresAt
        };

        public static ShortPersistedUserDto ToShortDto(this User user) =>
            new()
            {
                Id = user.Id,
                Patronymic = user.Patronymic,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                OrganizationId = user.OrganizationId
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
            Id = issue.Id,
            Title = issue.Title,
            CreatedAt = issue.CreatedAt,
            Status = issue.Status.ToDto(),
            Room = issue.Room.ToDto(),
            HasUnread = issue.HasUnreadComments,
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

        public static PersistedIssueCommentDto ToDto(this IssueComment comment) =>
            new();

        public static PersistedIssueDto ToDto(this Issue issue) =>
            new()
            {
                Id = issue.Id,
                CreatedAt = issue.CreatedAt,
                Version = issue.Version,
                Description = issue.Description,
                Status = issue.Status.ToDto(),
                Title = issue.Title,
                CategoryId = issue.CategoryId,
                Room = issue.Room.ToDto(),
                AuthorId = issue.AuthorId,
                HasUnreadComments = issue.HasUnreadComments,
            };

        public static PersistedBuildingDto ToDto(this Building building) => new()
        {
            Id = building.Id, Name = building.Name
        };

        public static PersistedFloorDto ToDto(this Floor floor) => new() {Id = floor.Id, Number = floor.Number};

        public static PersistedRoomDto ToDto(this Room room) => new()
        {
            Id = room.Id,
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
                Rooms = f.Rooms.Select(r => new OrganizationRoomDto {Id = r.Id, Number = r.Number})
            })
        };

        public static PersistedUserDto ToDto(this User user) => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
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

    public class PersistedIssueCommentDto
    {
    }
}
