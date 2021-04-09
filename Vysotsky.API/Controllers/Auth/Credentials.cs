namespace Vysotsky.API.Controllers.Auth
{
    /// <summary>
    /// Параметры для аутентификации пользователя
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// Логин
        /// </summary>
        public string Username { get; init; } = null!;

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; init; } = null!;

        /// <summary>
        /// Время жизни токена. Если передан true, то токен будет жить 180 дней, иначе 1 день.
        /// </summary>
        public bool LongLiving { get; init; }
    }
}