namespace BankMore.Transfer.Domain.Exceptions
{
    public class InvalidDocumentException : Exception
    {
        public InvalidDocumentException(string message)
            : base(message) { }
    }
}
