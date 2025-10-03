namespace BankMore.Account.Domain.Exceptions
{
    public class IdempotencyViolationException : Exception
    {
        public IdempotencyViolationException(string requestId)
                : base($"Já existe um movimento processado para o RequestId {requestId}.")
        {
        }
    }
}
