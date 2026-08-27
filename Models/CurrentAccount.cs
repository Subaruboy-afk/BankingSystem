using System.Buffers;

namespace BankingSystem;

public class CurrentAccount : BankAccount, ITransactable
{
    public decimal OverdraftLimit { get; private set; }
    private bool _isClosed = false;
    public override string AccountType => "Current Account";

    public CurrentAccount(AccountHolder owner, decimal overdraftLimit)
        : base(owner)
    {
        OverdraftLimit = overdraftLimit;
    }

    // Async transactions
    public async Task DepositAsync(decimal amount, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        Deposit(amount);
    }

    public async Task WithdrawAsync(decimal amount, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        Withdraw(amount);
    }

    public async Task TransferAsync(decimal amount, int targetAccountNumber, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        Transfer(amount, targetAccountNumber);
    }

    public void Deposit(decimal amount)
    {
        if (_isClosed) throw new InvalidOperationException($"Account for {Owner.Name.FirstName}");
        if (amount <= 0) throw new ArgumentException($"Amount must be greater than 0.00");
        Balance += amount;
        RecordTransaction(TransactionType.Deposit, amount, $"Deposit of {amount:C}");
        // System.Console.WriteLine($"Deposited [{amount:C}]. New Balance: [{Balance:C}]");
    }

    public void Withdraw(decimal amount)
    {
        if (_isClosed) throw new InvalidOperationException($"Account for {Owner.Name.FirstName} is closed.");
        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.00");
        if (amount > (Balance + OverdraftLimit)) throw new OverdraftLimitExceededException((Balance + OverdraftLimit), amount);

        Balance -= amount;
        RecordTransaction(TransactionType.Withdrawal, amount, $"Withdrawal of {amount:C}");
        // System.Console.WriteLine($"Withdrew: [{amount:C}]. New Balance: [{Balance:C}]");
    }

    public void Transfer(decimal amount, int targetAccountNumber)
    {
        if (_isClosed) throw new InvalidOperationException($"Account for {Owner.Name.FirstName} is closed");
        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.00:C");
        if (amount > (Balance + OverdraftLimit)) throw new OverdraftLimitExceededException((Balance + OverdraftLimit), amount);
        if (AccountLookup is null) throw new InvalidOperationException("Account is not registered in a repository");

        BankAccount target = AccountLookup(targetAccountNumber) ?? throw new InvalidAccountNumberException(targetAccountNumber);

        if (target.AccountNumber == AccountNumber) throw new InvalidOperationException("Cannot transfer to the same account");

        Balance -= amount;
        RecordTransaction(TransactionType.Transfer, amount, $"Transfered: [{amount:C}] to [{targetAccountNumber}]");
        target.RecieveTransfer(amount, AccountNumber);

        // System.Console.WriteLine($"Transfered: [{amount:C}] to [{targetAccountNumber}]. New Balance: [{Balance:C}]");

    }

    public override void GetAccountInfo()
    {
        System.Console.WriteLine($"Account holder [{Owner.Name.LastName} {Owner.Name.MiddleName} {Owner.Name.FirstName}] " +
                                $"\nAccount balance: {Balance:C} " +
                                $"\nAccount overdraft: {OverdraftLimit:C} " +
                                $"\nTotal balance: {(Balance + OverdraftLimit):C} " +
                                $"\nCreation date: {CreationDate} " +
                                $"\nAccount number: {AccountNumber}" +
                                $"\nAccount type: Current Account");
    }

    public void Close() => _isClosed = true;
}