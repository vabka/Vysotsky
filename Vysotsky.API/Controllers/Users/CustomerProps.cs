using System;
using Vysotsky.API.Controllers.Customers.Dto;

namespace Vysotsky.API.Controllers.Users
{
    /// <summary>
    /// Параметры для регистрации клиента
    /// </summary>
    public class CustomerProps
    {
        /// <summary>
        /// ФИО клиента
        /// </summary>
        public PersonName? Person { get; init; }

        /// <summary>
        /// Организация клиента
        /// </summary>
        public OrganizationInfo? Organization { get; init; }

        /// <summary>
        /// Идентификаторы помещений клиента
        /// </summary>
        public int[] Rooms { get; init; } = Array.Empty<int>();

        /// <summary>
        /// Контакты клиента
        /// </summary>
        public CustomerContact[] Contacts { get; init; } = Array.Empty<CustomerContact>();
    }
}
