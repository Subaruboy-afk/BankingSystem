namespace BankingSystem;

public record CustomerName(string LastName, string? MiddleName, string FirstName);

public record AccountHolder(CustomerName Name, string NationalId, string Email, string? Phone);

public enum TransactionType
{
    Credit,
    Deposit,
    Withdrawal,
    Transfer,
    Purchase,
    Sale
}

public record Transaction(Guid TId, TransactionType Type, decimal Amount, string Desc, DateTime TimeStamp, decimal BalanceAfter)
{
    public override string ToString() => $"Transaction info: TID: {TId}, [{Type}], (${Amount:F2}), ''{Desc}'', Time: [{TimeStamp:yyyy-MM-dd HH-mm}], Balance: [{BalanceAfter}]";
}