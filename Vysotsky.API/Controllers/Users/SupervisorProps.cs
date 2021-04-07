using Vysotsky.API.Controllers.Customers;

namespace Vysotsky.API.Controllers.Users
{
    /// <summary>
    /// Параметры для регистрации диспетчера
    /// </summary>
    public class SupervisorProps
    {
        /// <summary>
        /// ФИО диспетчера
        /// </summary>
        public PersonName PersonName { get; init; } = null!;
    }
}