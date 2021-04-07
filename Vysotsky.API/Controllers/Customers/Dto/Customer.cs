using System;
using Vysotsky.API.Controllers.Rooms;

namespace Vysotsky.API.Controllers.Customers
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

        /// <summary>
        /// Помещения клиента
        /// </summary>
        public Room[] Rooms { get; init; } = Array.Empty<Room>();

        /// <summary>
        /// Контакты клиента
        /// </summary>
        public CustomerContact[] Contacts { get; init; } = Array.Empty<CustomerContact>();
    }
}