

namespace SGCP.Domain.Base
{
    public class OperationResult
    {

      

        public string? Message { get; set; }
        public bool Success { get; set; }

        public dynamic? Data { get; set; }

        public static OperationResult SuccessResult(string message, dynamic? data = null)
        {
            return new OperationResult
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static OperationResult FailureResult(string message, dynamic? data = null)
        {
            return new OperationResult
            {
                Success = false,
                Message = message,
                Data = data
            };
        }



    }
 
        }
