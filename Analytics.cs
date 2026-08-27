using System.Linq;
namespace BankingSystem;

public static class Analytics
{

    public static void Run(BankRepository<BankAccount> accountRepo)
    {
        System.Console.WriteLine("\n=== Average Balance of accounts ===");
        System.Console.WriteLine($"Average balance: {accountRepo.Accounts.Average(a => a.Balance)}");

        System.Console.WriteLine("===\n Ranking accounts by balance ===");
        var byPrice = accountRepo.Accounts
            .OrderByDescending(b => b.Balance)
            .Take(5)
            .Select(s => $"{s.Owner.Name.FirstName} - {s.AccountType} - {s.Balance}")
            .ToList();
        // System.Console.WriteLine($"Highest account balance: {accountRepo.Accounts.OrderByDescending(b => b.Balance).FirstOrDefault()}");
        byPrice.ForEach(Console.WriteLine);

        System.Console.WriteLine("\n=== Account with highest balance ===");

        var topAccount = accountRepo.Accounts.OrderByDescending(b => b.Balance).FirstOrDefault();
        if (topAccount is not null)
        {
            System.Console.WriteLine($"{topAccount.Owner.Name.FirstName} - {topAccount.AccountType} - [{topAccount.Balance}]");
        }

        System.Console.WriteLine("\n=== Numbe of accounts in each account category ===");

        var byCategory = accountRepo.Accounts
            .GroupBy(a => a.AccountType)
            .Select(g => new
            {
                Type = g.Key,
                Count = g.Count(),
            });

        foreach (var g in byCategory)
        {
            System.Console.WriteLine($"\n{g.Type}");
            System.Console.WriteLine($"Count: {g.Count}");
        }

        System.Console.WriteLine("\n=== Accounts with negative balance ===");

        var negBalance = accountRepo.Accounts
            .Where(b => b.Balance < 0).ToList();

        // negBalance.ForEach(Console.WriteLine());

        foreach (var b in negBalance)
        {
            System.Console.WriteLine($"{b.Owner.Name.FirstName} - {b.AccountType} - [{b.Balance}]");
        }
    }
}