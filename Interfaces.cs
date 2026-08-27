namespace BankingSystem;

public interface ITransactable
{
    public void Deposit(decimal amount);

    public void Withdraw(decimal amount);

    public void Transfer(decimal amount, int accountNumber);
}