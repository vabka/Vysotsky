namespace Vysotsky.API.Models
{
    /// <summary>
    /// Данные об ошибке
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message { get; init; } = "";

        /// <summary>
        /// Код ошибки
        /// </summary>
        public string Code { get; init; } = "";
    }
}