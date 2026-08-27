using System.Buffers;

namespace BankingSystem;

public class InvestmentAccount : BankAccount, ITransactable
{
    internal Func<string, double?>? PriceLookup { get; set; }
    private Dictionary<string, AssetHolding> _holdings = new();
    public IReadOnlyDictionary<string, AssetHolding> Holdings => _holdings;
    public decimal TotalPortfolioValue
    {
        get
        {
            decimal total = Balance;

            foreach (var holding in _holdings.Values)
            {
                double? price = PriceLookup?.Invoke(holding.Symbol);
                if (price is not null)
                    total += holding.Quantity * (decimal)price;
            }
            return total;
        }
    }
    private bool _isClosed = false;
    public override string AccountType => "Investment Account";

    public InvestmentAccount(AccountHolder owner)
        : base(owner)
    {

    }

    // Async transactions
    public async Task DepositAsync(decimal amount, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        Deposit(amount);
    }

    public async Task TransferAsync(decimal amount, int targetAccountNumber, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        Transfer(amount, targetAccountNumber);
    }

    public async Task WithdrawAsync(decimal amount, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        Withdraw(amount);
    }

    public async Task BuyStockAsync(string symbol, decimal amount, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        BuyStock(symbol, amount);
    }

    public async Task SellStockAsync(string symbol, decimal amount, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        SellStock(symbol, amount);
    }

    public void Deposit(decimal amount)
    {
        if (_isClosed) throw new InvalidOperationException($"Account for {Owner.Name.FirstName}");
        if (amount <= 0) throw new ArgumentException($"Amount must be greater than 0.00");
        Balance += amount;
        RecordTransaction(TransactionType.Deposit, amount, $"Deposit of {amount:C}");
        // System.Console.WriteLine($"Deposited [{amount:C}]. New Balance: [{Balance:C}]");
    }

    public void BuyStock(string symbol, decimal amount)
    {
        if (PriceLookup is null) throw new InvalidOperationException("Price lookup for this stock came back null");
        double? price = PriceLookup(symbol);
        if (price is null) throw new InvalidOperationException($"Price for {symbol} not found");
        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.00");
        if (amount > Balance) throw new InsufficientFundsException(Balance, amount);
        decimal totalPurchaseQuantity = amount / (decimal)price; // units to be bought
        Balance -= amount;
        if (_holdings.TryGetValue(symbol, out var holding))
            holding.IncreaseQuantity(totalPurchaseQuantity);
        else
            _holdings[symbol] = new AssetHolding(symbol, totalPurchaseQuantity);

        RecordTransaction(TransactionType.Purchase, amount, $"Bought {amount:C} worth of {symbol}");

        // System.Console.WriteLine($"Aped {amount:C} worth of {symbol}. New balance is {Balance:C}");
    }
    
    public void SellStock(string symbol, decimal amount)
    {
        if (PriceLookup is null) throw new InvalidOperationException("Price lookup for this stock came back null");
        double? price = PriceLookup(symbol);
        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.00");
        decimal quantity = _holdings[symbol].Quantity; // units held for [symbol]
        decimal totalHeldAmount = quantity * (decimal)price; // value held for [symbol]
        decimal totalSaleQuantity = amount / (decimal)price; // units to sell
        if (amount > totalHeldAmount) throw new InsufficientHoldingsException(symbol, totalHeldAmount, totalSaleQuantity);
        Balance += amount;
        if (_holdings.TryGetValue(symbol, out var holding))
            holding.DecreaseQuantity(totalSaleQuantity);
        else throw new InsufficientHoldingsException(symbol, holding.Quantity, totalSaleQuantity);

        RecordTransaction(TransactionType.Sale, amount, $"Sold {amount:C} worth of {symbol:F2}");

        // System.Console.WriteLine($"Liquidated {amount:C} worth of {symbol}. New balance is {Balance:C}");
    }

    public void Transfer(decimal amount, int targetAccountNumber)
    {
        if (_isClosed) throw new InvalidOperationException($"Account for {Owner.Name.FirstName} is closed");
        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.00");
        if (amount > Balance) throw new InsufficientFundsException(Balance, amount);
        if (AccountLookup is null) throw new InvalidOperationException("Account not registered");

        BankAccount target = AccountLookup(targetAccountNumber) ?? throw new InvalidAccountNumberException(targetAccountNumber);

        if (target.AccountNumber == AccountNumber) throw new InvalidOperationException("Cannot transfer to the same account number");

        Balance -= amount;
        RecordTransaction(TransactionType.Transfer, amount, $"Transfered: [{amount:c}] to [{targetAccountNumber}]");
        target.RecieveTransfer(amount, AccountNumber);

        // System.Console.WriteLine($"Transfered: [{amount:C}] to [{targetAccountNumber}]. New Balance: [{Balance:C}]");
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

    public override void GetAccountInfo()
    {
        System.Console.WriteLine($"Account holder [{Owner.Name.LastName} {Owner.Name.MiddleName} {Owner.Name.FirstName}]" +
                                 $"\nAccount balance: {Balance:C}" +
                                 $"\n");
        FetchHoldings();

    }
    
    public void FetchHoldings()
    {
        System.Console.WriteLine("Account Holdings:");
        foreach(var pair in Holdings)
        {
            System.Console.WriteLine($"[{pair.Key}] - {pair.Value.Quantity:F2} units held. Worth: [{pair.Value.Quantity * (decimal)PriceLookup!(pair.Key):C}]");
        }
    }

    public void Close() => _isClosed = true;
}

public class AssetHolding
{
    public string Symbol { get; }
    public decimal Quantity { get; private set; }

    public AssetHolding(string symbol, decimal quantity)
    {
        Symbol = symbol;
        Quantity = quantity;
    }

    public void IncreaseQuantity(decimal amount) => Quantity += amount;

    public void DecreaseQuantity(decimal amount) => Quantity -= amount;
}


