namespace BankMore.Transfer.Domain.DTOs.Responses
{
    public class ErrorResponse
    {
        public ErrorResponse(string message, string errorType)
        {
            ErrorType = errorType;
            Message = message;
        }

        public string ErrorType { get; set; }
        public string Message { get; set; }
    }
}
