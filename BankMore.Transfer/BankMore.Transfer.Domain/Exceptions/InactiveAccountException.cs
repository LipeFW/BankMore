namespace BankMore.Transfer.Domain.Exceptions
{
    public class InactiveAccountException : Exception
    {
        public InactiveAccountException(string message) : base(message) { }
    }
}
