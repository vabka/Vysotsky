namespace Vysotsky.API.Models
{
    public class ApiResponse
    {
        public ResponseStatus Status { get; init; }
        public ApiError? Error { get; init; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Result { get; init; }
    }
}
