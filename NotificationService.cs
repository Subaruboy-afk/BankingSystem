namespace BankingSystem;

public class TransactionCompletedEventArgs : EventArgs
{
    // this is the data carrier
    public AccountHolder Owner { get; init; }
    public decimal Amount { get; init; }
    public decimal Balance { get; init; }
    public TransactionType TransactionType { get; init; }
    public string AccountType { get; init; }

    public TransactionCompletedEventArgs(AccountHolder owner, decimal amount, decimal balance, TransactionType transactionType, string accountType)
    {
        Owner = owner;
        Amount = amount;
        Balance = balance;
        TransactionType = transactionType;
        AccountType = accountType;
    }
}



// Subscriber method
public class AlertService
{
    public void OnTransactionComplete(object? sender, TransactionCompletedEventArgs args)
    {
        System.Console.WriteLine(
            $"[ALERT] Account owner: {args.Owner.Name.LastName} | " +
            $"Transaction type: {args.TransactionType} | " +
            $"Amount: {args.Amount:C} | " +
            $"New Balance: {args.Balance:C} | " +
            $"Account type: [{args.AccountType}]"
        );
    }
}
