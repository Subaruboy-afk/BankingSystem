namespace BankingSystem;

public class BankingService
{
    private BankRepository<BankAccount> _accountRepository = new();
    private StockRepository _stockRepository = new();

    public BankRepository<BankAccount> AccountRepository => _accountRepository;

    public void CreateAccount(BankAccount account)
    {
        if (account is InvestmentAccount investmentAccount)
        {
            investmentAccount.PriceLookup = _stockRepository.FindBySymbol;
        }
        _accountRepository.AddToBankRepo(account);
        System.Console.WriteLine($"Account created for {account.Owner.Name.FirstName} {account.Owner.Name.LastName}");
    }

    public void CloseAccount(BankAccount account)
    {
        _accountRepository.CloseAccount(account);
        System.Console.WriteLine($"Account closed for {account.Owner.Name.FirstName} {account.Owner.Name.LastName}");
    }
}