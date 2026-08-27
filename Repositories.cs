    using System.ComponentModel;

namespace BankingSystem;

public class BankRepository<T> where T : BankAccount
{
    private readonly List<T> _accounts = new();
    private int _count = 0;
    public IReadOnlyList<T> Accounts => _accounts;

    public void AddToBankRepo(T account)
    {
        account.AccountLookup = FindByAccountNumber;
        _accounts.Add(account);
        _count++;
    }

    public void CloseAccount(T account)
    {
        _accounts.Remove(account);
        _count--;
    }

    public T? FindByAccountNumber(int accountNumber) => _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
}


public class StockRepository 
{
    private Dictionary<string, double> StockCatalogue = new Dictionary<string, double>
    {
        {"NVDA", 225.16 },
        {"AAPL", 305.93 },
        {"GOOGL", 343.54},
        {"MSFT", 495.40},
        {"AMZN", 262.65},
        {"TSM", 426.35},
        {"AVGO", 392.99},
        {"META", 589.85},
        {"TSLA", 342.27},
        {"BRK.B", 504.03}
    };

    public double? FindBySymbol(string symbol)
    {
        if (StockCatalogue.TryGetValue(symbol, out double price))
            return price;
            
        return null;
    }
}