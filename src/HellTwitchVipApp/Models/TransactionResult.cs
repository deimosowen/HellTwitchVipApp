using HellTwitchVipApp.Models.Enum;

namespace HellTwitchVipApp.Models
{
    public class TransactionResult<T>
    {
        public TransactionResult(T result)
        {
            Result = result;
            Status = TransactionsStatus.Success;
        }

        protected T Result { get; set; }
        public TransactionsStatus Status { get; protected set; }
        public string Message { get; protected set; }

        public void Error(string message)
        {
            Status = TransactionsStatus.Error;
            Message = message;
        }
    }
}
