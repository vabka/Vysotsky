namespace Vysotsky.API.Models
{
    public class ApiResponse
    {
        public ResponseStatus Status { get; init; }
        public ApiError? Error { get; init; }
    }
}
