using System.Text.Json.Serialization;

namespace Vysotsky.API.Models
{
    public class ApiResponse
    {
        public ResponseStatus Status { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ApiError? Error { get; init; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Result { get; init; }
    }
}
