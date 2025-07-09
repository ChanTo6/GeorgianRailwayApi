using Microsoft.AspNetCore.Http;

namespace GeorgianRailwayApi.Models
{
    public class ApiErrorResponse
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public int Status { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string[]> Errors { get; set; } = new();
        public string ErrorCode { get; set; }

        public static ApiErrorResponse Validation(string title, List<string> errors)
        {
            var response = new ApiErrorResponse
            {
                Title = title,
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Type = "https://httpstatuses.com/400"
            };
            foreach (var error in errors)
                response.Errors.Add(error, new[] { error });
            return response;
        }

        public static ApiErrorResponse Failure(string title, string detail, string errorCode, int status = StatusCodes.Status400BadRequest)
        {
            return new ApiErrorResponse
            {
                Title = title,
                Detail = detail,
                Status = status,
                Type = $"https://httpstatuses.com/{status}",
                ErrorCode = errorCode
            };
        }
    }
}