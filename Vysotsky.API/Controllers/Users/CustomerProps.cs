using System;
using Vysotsky.API.Controllers.Customers.Dto;

namespace Vysotsky.API.Controllers.Users
{
    /// <summary>
    /// Параметры для регистрации владельца организации
    /// </summary>
    public class CustomerProps
    {
        /// <summary>
        /// Организация клиента
        /// </summary>
        public OrganizationInfo? Organization { get; init; }

        /// <summary>
        /// Идентификаторы помещений клиента
        /// </summary>
        public int[] Rooms { get; init; } = Array.Empty<int>();
    }
}