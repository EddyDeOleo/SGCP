namespace SGCP.Application.Base
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; } 
        public dynamic? Data { get; set; }


        public ServiceResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public ServiceResult(bool success, string message, object? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public ServiceResult()
        {
        }
    }
}
