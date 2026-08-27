namespace BankingSystem;

public static class BankExtensions
{
    public static bool IsOverDrawn(this BankAccount account) => account.Balance < 0;

    public static string FormattedSummary(this BankAccount account)
    {
        return account.AccountType switch
        {
            "Savings Account" => $"SAV-[{account.AccountNumber} | {account.Owner.Name.LastName} {account.Owner.Name.FirstName} {account.Owner.Name.MiddleName} | [{account.Balance}]]",
            "Current Account" => $"CUR-[{account.AccountNumber} | {account.Owner.Name.LastName} {account.Owner.Name.FirstName} {account.Owner.Name.MiddleName} | [{account.Balance}]]",
            "Investment Account" => $"INV-[{account.AccountNumber}] | {account.Owner.Name.LastName} {account.Owner.Name.FirstName} {account.Owner.Name.MiddleName} | [{account.Balance}]",
            _ => "...[Unknown Account Type]..."


        };
    }

    public static decimal TotalBalance(this IEnumerable<BankAccount> accounts) => accounts.Sum(a => a.Balance);

    public static int DaysSinceCreation(this BankAccount account)
    {
        DateTime created = DateTime.ParseExact(account.CreationDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        return (DateTime.Now - created).Days;
    }
}