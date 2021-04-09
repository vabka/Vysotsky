using System;
using Vysotsky.API.Controllers.Customers.Dto;

namespace Vysotsky.API.Controllers.Users
{
    public record Credentials(string Username, string Password);

    /// <summary>
    /// Параметры для регистрации пользователя
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Логин-пароль
        /// </summary>
        public Credentials Auth { get; init; } = null!;

        /// <summary>
        /// ФИО
        /// </summary>
        public PersonName Person { get; init; } = null!;

        public UserContact[] Contacts { get; init; } = Array.Empty<UserContact>();

        public UserRole Role { get; init; }

        /// <summary>
        /// Поля для регистрации пользователя с правами клиента
        /// </summary>
        public CustomerProps? Customer { get; init; }
    }

    public enum UserRole
    {
        Supervisor,
        Worker,
        Customer
    }
}