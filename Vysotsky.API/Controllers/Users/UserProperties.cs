namespace Vysotsky.API.Controllers.Users
{
    /// <summary>
    /// Параметры для регистрации пользователя
    /// </summary>
    public class UserProperties
    {
        /// <summary>
        /// Логин-пароль
        /// </summary>
        public AuthProps Auth { get; init; } = null!;

        /// <summary>
        /// Поля для регистрации пользователя с правами клиента
        /// </summary>
        public CustomerProps? Customer { get; init; }

        /// <summary>
        /// Поля для регистрации пользователя с правами сотрудника
        /// </summary>
        public EmployeeProprs? Employee { get; init; }

        /// <summary>
        /// Поля для регистрации пользователя с правами диспетчера
        /// </summary>
        public SupervisorProps? Supervisor { get; init; }
    }
}