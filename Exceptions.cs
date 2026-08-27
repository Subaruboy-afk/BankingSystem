namespace BankingSystem;

public class AppExceptions : Exception
{
    public string ErrorCode { get; }

    public AppExceptions(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public AppExceptions(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

public class InvalidAccountNumberException : AppExceptions
{
    public int AccountNumber { get; }

    public InvalidAccountNumberException(int accountNumber)
        : base($"Customer with {accountNumber} not found", "ACCOUNT_NOT_FOUND")
    {
        AccountNumber = accountNumber;
    }
}

public class InsufficientFundsException : AppExceptions
{
    public decimal Available { get; }
    public decimal Required { get; }

    public InsufficientFundsException(decimal available, decimal required)
        : base($"Insufficient funds. Available: {available:C}. Required: {required:C}", "INSUFFICIENT_FUNDS")
    {
        Available = available;
        Required = required;
    }
}

public class OverdraftLimitExceededException : AppExceptions
{
    public decimal Available { get; }
    public decimal Required { get; }

    public OverdraftLimitExceededException(decimal available, decimal required)
        : base($"Overdraft limit exceeded. Available: {available:C}. Requested: {required:C}", "OVERDRAFT_LIMIT_EXCEEDED")
    {
        Available = available;
        Required = required;
    }
}

public class InsufficientHoldingsException : AppExceptions
{
    public string Symbol { get; }
    public decimal QuantityHeld { get; }
    public decimal QuantityRequested { get; }

    public InsufficientHoldingsException(string symbol, decimal quantityHeld, decimal quantityRequested)
        : base($"Not enough holdings of {symbol} for this transaction. Your account holds {quantityHeld:C} worth of {symbol} - [{quantityRequested:F2}] units", "INSUFFICIENT_HOLDINGS")
    {
        Symbol = symbol;
        QuantityHeld = quantityHeld;
        QuantityRequested = quantityRequested;
    }
}