namespace Vysotsky.API.Controllers.Auth
{
    /// <summary>
    /// Контейнер для токена
    /// </summary>
    public class AccessTokenContainer
    {
        /// <summary>
        /// Токен
        /// </summary>
        public string AccessToken { get; init; } = null!;
    }
}
