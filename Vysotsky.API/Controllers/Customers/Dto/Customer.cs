using System;

namespace Vysotsky.API.Controllers.Customers.Dto
{
    /// <summary>
    /// Клиент
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Уникальный Идентификатор
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// ФИО клиента
        /// </summary>
        public PersonName? Person { get; init; }

        /// <summary>
        /// Организация клиента
        /// </summary>
        public OrganizationInfo? Organization { get; init; }

        // public Room[] Rooms { get; init; } = Array.Empty<Room>();

        /// <summary>
        /// Контакты клиента
        /// </summary>
        public UserContact[] Contacts { get; init; } = Array.Empty<UserContact>();
    }
}