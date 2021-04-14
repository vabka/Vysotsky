using System.Text.Json.Serialization;

namespace Vysotsky.API.Dto.Common
{
    /// <summary>
    /// Стандартный ответ
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Состояние
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResponseStatus Status { get; init; }

        /// <summary>
        /// Данные об ошибке, если произошла ошибка
        /// </summary>

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ApiError? Error { get; init; }
    }

    /// <summary>
    /// Стандартный ответ с данными
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T> : ApiResponse
    {
        /// <summary>
        /// Данные
        /// </summary>
        public T? Result { get; init; }
    }
}