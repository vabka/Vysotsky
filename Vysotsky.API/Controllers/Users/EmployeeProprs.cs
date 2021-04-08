using Vysotsky.API.Controllers.Customers.Dto;

namespace Vysotsky.API.Controllers.Users
{
    /// <summary>
    /// Параметры для создания сотрудника УК
    /// </summary>
    public class EmployeeProprs
    {
        /// <summary>
        /// ФИО Сотрудника
        /// </summary>
        public PersonName Person { get; init; } = null!;
    }
}
