using System.Linq.Expressions;
using System.Transactions;
using BankingSystem;

#region Instances
    var bankingService = new BankingService();
    var alertService = new AlertService();
#endregion

#region NewCustomers
    var customerName1 = new CustomerName("Osawe", "Osas", "Akin");
    var accountHolder1 = new AccountHolder(customerName1, "ABC12345", "akin@email.com", "12345678901");
    
    var customerName2 = new CustomerName("James", "", "Gunn");
    var accountHolder2 = new AccountHolder(customerName2, "DEF67890", "osas@email.com", "09876543212");
    
    var customerName4 = new CustomerName("Sportacus", "Da", "Speedster");
    var accountHolder4 = new AccountHolder(customerName4, "HIJKLMN0", "sporta@cus.com", "00293873655");

    var customerName5 = new CustomerName("Dave", "Chapelle", "Nicolas");
    var accountHolder5 = new AccountHolder(customerName5, "IRVUIYRFH", "dave@chapelle.com", "029037256723");

#endregion

System.Console.WriteLine("\n");

#region NewAccountsRegistration
    var account1 = new SavingsAccount(accountHolder1);
    bankingService.CreateAccount(account1);
    
    var account2 = new SavingsAccount(accountHolder2);
    bankingService.CreateAccount(account2);
    
    var account3 = new CurrentAccount(accountHolder1, 500);
    bankingService.CreateAccount(account3);
    
    var account4 = new InvestmentAccount(accountHolder4);
    bankingService.CreateAccount(account4);

    var account5 = new InvestmentAccount(accountHolder5);
    bankingService.CreateAccount(account5);

    System.Console.WriteLine("\n");
#endregion

#region AlertSubscription
account1.TransactionComplete += alertService.OnTransactionComplete;
    account2.TransactionComplete += alertService.OnTransactionComplete;
    account3.TransactionComplete += alertService.OnTransactionComplete;
    account4.TransactionComplete += alertService.OnTransactionComplete;
    account5.TransactionComplete += alertService.OnTransactionComplete;
#endregion

using var session = new BankSessionManager("Bank Session");
session.LogSession();

try
{
    await account1.DepositAsync(1100);
    await account1.TransferAsync(100, account2.AccountNumber);
    await account1.WithdrawAsync(1000);

    System.Console.WriteLine("\n");
    await account5.DepositAsync(1000000000);

    System.Console.WriteLine("\n");
    await account3.DepositAsync(1000);
    await account3.WithdrawAsync(1000);
    await account3.TransferAsync(500, account4.AccountNumber);
    // await account3.TransferAsync(500, account1.AccountNumber);
    // System.Console.WriteLine(account1.Balance); Deprecated in favor of ShowBalance method in BankAccount.cs
    account1.ShowBalance();
    foreach (var tx in account3.Transactions)
    {
        System.Console.WriteLine(tx);
    }

    System.Console.WriteLine("\n");


    await account4.DepositAsync(10000);
    await account4.BuyStockAsync("NVDA", 2000);
    await account4.BuyStockAsync("AAPL", 2000);
    // System.Console.WriteLine($"\nAccount Balance: [{account4.Balance:C}]"); Deprecated in favor of ShowBalance method in BankAccount.cs
    account4.ShowBalance();
    // both foreach loops have been deprecated and commented out in favor of the FetchHoldings method in InvestmentAccount.cs
    // foreach (var kvp in account4.Holdings)
    // {
    //     double? price = account4.PriceLookup.Invoke(kvp.Key);
    //     decimal value = price is not null ? kvp.Value.Quantity * (decimal)price : 0;
    //     System.Console.WriteLine($"{kvp.Key} - {kvp.Value.Quantity:F2} units held. Value - [{value:F2}]");
    // }
    account4.FetchHoldings();
    await account4.SellStockAsync("NVDA", 1000);
    await account4.SellStockAsync("AAPL", 500);
    // both foreach loops have been deprecated and commented out in favor of the FetchHoldings method in InvestmentAccount.cs
    // foreach (var kvp in account4.Holdings)
    // {
    //     double? price = account4.PriceLookup.Invoke(kvp.Key);
    //     decimal value = price is not null ? kvp.Value.Quantity * (decimal)price : 0;
    //     System.Console.WriteLine($"{kvp.Key} - {kvp.Value.Quantity:F2} units held. Value - [{value:F2}]");
    // }

    account4.FetchHoldings();

    System.Console.WriteLine("\n");
    await foreach(var tx in account1.StreamTransactionsAsync())
    {
        System.Console.WriteLine(tx);
    }

    Analytics.Run(bankingService.AccountRepository);

    System.Console.WriteLine("\n");

    System.Console.WriteLine(account1.IsOverDrawn());

    System.Console.WriteLine("\n");

    System.Console.WriteLine(account4.FormattedSummary());

    System.Console.WriteLine("\n");

    System.Console.WriteLine(bankingService.AccountRepository.Accounts.TotalBalance());

    System.Console.WriteLine("\n");

    System.Console.WriteLine(account5.DaysSinceCreation());



}
catch (InvalidAccountNumberException ex)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"[ERROR] {ex.Message}");
    Console.ResetColor();
}
catch (InsufficientFundsException ex)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[ERROR] {ex.Message}");
    Console.ResetColor();
}
catch (OverdraftLimitExceededException ex)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    System.Console.WriteLine($"[ERROR] {ex.Message}");
    Console.ResetColor();
}
catch (InsufficientHoldingsException ex)
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    System.Console.WriteLine($"[ERROR] {ex.Message}");
    Console.ResetColor();
}
catch (InvalidOperationException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    System.Console.WriteLine($"[ERROR] {ex.Message}");
    Console.ResetColor();
}
catch (ArgumentException ex)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    System.Console.WriteLine($"[ERROR] {ex.Message}");
    Console.ResetColor();
}
catch (Exception ex)
{
    System.Console.WriteLine($"[ERROR] {ex.Message}");
}


// Value - [{kvp.Value.Quantity*(decimal)sstockRepository.FindBySymbol(kvp.Key):F2}] KeyValuePair<string, AssetHolding>c
