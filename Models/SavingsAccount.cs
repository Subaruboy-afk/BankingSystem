using System.Buffers;

namespace BankingSystem;

public class SavingsAccount : BankAccount, ITransactable
{
    public decimal InterestRate { get; } = 0.03m;
    private bool _isClosed = false;
    public override string AccountType => "Savings Account";

    public SavingsAccount(AccountHolder owner)
        : base(owner) { }

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
        await Task.Delay(100);
        Transfer(amount, targetAccountNumber);
    }

    public void Deposit(decimal amount)
    {
        if (_isClosed) throw new InvalidOperationException($"Account for {Owner.Name.FirstName} is closed");
        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.00");
        Balance += amount;
        RecordTransaction(TransactionType.Deposit, amount, $"Deposit of {amount:C}");
        // System.Console.WriteLine($"Deposited {amount:C}. New Balance: {Balance:C}");
    }

    public void Withdraw(decimal amount)
    {
        if (_isClosed) throw new InvalidOperationException($"Account for {Owner.Name.FirstName} is closed");
        if (amount <= 0) throw new ArgumentException($"Withdrawal amount must be more than 0.00");
        if (amount > Balance) throw new InsufficientFundsException(Balance, amount);

        Balance -= amount;
        RecordTransaction(TransactionType.Withdrawal, amount, $"Withdrawal of: {amount:C}");
        // System.Console.WriteLine($"Withdrew: [{amount:C}]. New Balance: [{Balance:C}]");
    }

    public void Transfer(decimal amount, int targetAccountNumber)
    {
        if (_isClosed) throw new InvalidOperationException($"Account for {Owner.Name.FirstName} is closed");
        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.00:C");
        if (amount > Balance) throw new InsufficientFundsException(Balance, amount);
        if (AccountLookup is null) throw new InvalidOperationException("Account is not registered with a repository");

        BankAccount target = AccountLookup(targetAccountNumber) ?? throw new InvalidAccountNumberException(targetAccountNumber);
 
        if (target.AccountNumber == AccountNumber) throw new InvalidOperationException("Cannot transfer to the same account");

        Balance -= amount;
        RecordTransaction(TransactionType.Transfer, amount, $"Transfered: [{amount:c}] to [{targetAccountNumber}]");
        target.RecieveTransfer(amount, AccountNumber);

        // System.Console.WriteLine($"Transfered: [{amount:C}] to [{targetAccountNumber}]. New Balance: [{Balance:C}]");
    }

    public override void GetAccountInfo()
    {
        decimal interestEarned = Balance * InterestRate;
        System.Console.WriteLine($"Account holder [{Owner.Name.LastName} {Owner.Name.MiddleName} {Owner.Name.FirstName}] " +
                                $"\nAccount balance: {Balance:C} " +
                                $"\nInterest earned: {interestEarned:C} " +
                                $"\nTotal balance: {Balance + interestEarned:C} " +
                                $"\nCreation date: {CreationDate} " +
                                $"\nAccount number: {AccountNumber}" +
                                $"\nAccount type: Savings account");
    }

    public void Close() => _isClosed = true;
}