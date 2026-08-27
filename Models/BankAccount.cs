using System.Runtime.Serialization;
using System.Transactions;

namespace BankingSystem;

public abstract class BankAccount
{
    public AccountHolder Owner { get; set; }
    public int AccountNumber { get; protected set; }
    public decimal Balance { get; protected set; } = 0.00m;
    public string CreationDate { get; private set; } = string.Empty;
    private static int _registeredAccounts = 0;
    public static int RegisteredAccounts => _registeredAccounts;
    private List<Transaction> _transactions = new();
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();
    internal Func<int, BankAccount?>? AccountLookup { get; set; }
    public abstract string AccountType { get; }
    public event EventHandler<TransactionCompletedEventArgs>? TransactionComplete;

    protected BankAccount(AccountHolder owner)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        AccountNumber = Random.Shared.Next(100000000, 999999999);
        CreationDate = DateTime.Now.ToString("yyyyMMdd");
        _registeredAccounts++;
    }

    public void ShowBalance()
    {
        System.Console.WriteLine($"Name: [{Owner.Name.FirstName}] - Account Balance: [{Balance:C}]");
    }

    internal void RecieveTransfer(decimal amount, int fromAccountNumber)
    {
        Balance += amount;
        RecordTransaction(TransactionType.Credit, amount, $"Received [{amount:C}] from: [{fromAccountNumber}]");
    }

    protected void RecordTransaction(TransactionType type, decimal amount, string desc)
    {
        _transactions.Add(new Transaction(Guid.NewGuid(), type, amount, desc, DateTime.Now, Balance));

        OnTransactionComplete(new TransactionCompletedEventArgs(Owner, amount, Balance, type, AccountType));
    }

    protected virtual void OnTransactionComplete(TransactionCompletedEventArgs args)
    {
        TransactionComplete?.Invoke(this, args);
    }
    
    public async IAsyncEnumerable<Transaction> StreamTransactionsAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        foreach (var tx in _transactions)
        {
            await Task.Delay(50, ct);
            yield return tx;
        }
    }

    public abstract void GetAccountInfo();
}


