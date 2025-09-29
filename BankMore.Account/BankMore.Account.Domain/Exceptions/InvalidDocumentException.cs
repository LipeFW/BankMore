namespace BankMore.Account.Domain.Exceptions
{
    public class InvalidDocumentException : Exception
    {
        public InvalidDocumentException(string message)
            : base(message) { }
    }
}
