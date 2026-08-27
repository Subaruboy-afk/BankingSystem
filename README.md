BankingSystem

A console-based banking platform built in C# as a capstone project covering the full range of C# language concepts — from core OOP through advanced language features. Every account type, service, and pattern in this repo was designed and implemented from scratch.

What This Project Demonstrates

OOP fundamentals

Abstract base class (BankAccount) with three concrete account types
Inheritance — SavingsAccount, CurrentAccount, InvestmentAccount
Polymorphism — AccountType, GetAccountInfo() overridden per account type
Encapsulation — private/protected state (Balance, _holdings, _transactions) exposed only through controlled properties and methods
Interfaces — ITransactable
Records — Transaction, AccountHolder, CustomerName
sealed, IDisposable — BankSessionManager

Intermediate C#

Generics with constraints — BankRepository<T> where T : BankAccount
Collections — List<T>, Dictionary<TKey, TValue>, IReadOnlyList<T>, IReadOnlyDictionary<TKey, TValue>
Delegates — AccountLookup (Func<int, BankAccount?>), PriceLookup (Func<string, double?>)
Events — TransactionComplete (EventHandler<TransactionCompletedEventArgs>) on BankAccount
Lambdas — throughout LINQ queries and delegate assignments
Extension methods — IsOverdrawn(), FormattedSummary(), TotalBalance(), DaysSinceCreation()
Nullable reference/value types — string?, Func<...>?, double?
Pattern matching — switch expressions (FormattedSummary), is pattern matching (is InvestmentAccount investmentAccount)

Advanced C#

LINQ — Where, OrderByDescending, GroupBy, Sum, Average,, Take
async/await with CancellationToken — async variants of Deposit, Withdraw, Transfer, BuyStock, SellStock 
IAsyncEnumerable<T> — StreamTransactionsAsync() using yield return
Custom exception hierarchy — AppExceptions base, with InsufficientFundsException, InvalidAccountNumberException, OverdraftLimitExceededException, InsufficientHoldingsException
Dependency injection via delegates — accounts are handed capability-shaped functions (AccountLookup, PriceLookup) at registration time rather than holding direct references to repositories

Architecture (May not be exhaustive)

BankingSystem/
├── Exceptions/     Custom exception hierarchy
├── Models/         BankAccount (abstract), SavingsAccount, CurrentAccount,
│                   InvestmentAccount, AssetHolding
├── Interfaces/     ITransactable
├── Repositories/   BankRepository<T>, StockRepository
├── Records/        CustomerName,Customer, TransactionType, Transaction
├── Extensions/     BankExtensions
├── Services/       BankingService
├── Analytics
├── Session/        BankSessionManager (IDisposable)
└── Program.cs      Wires everything together

Design principle: capability injection over hard coupling

Model classes (BankAccount and its subclasses) never hold direct references to repository or service classes. Instead, they expose delegate-typed "holes" — AccountLookup, PriceLookup — that get filled in exactly once, at the moment an account is registered via BankingService.CreateAccount. This keeps the model layer free of infrastructure dependencies: an account can be unit-tested by handing it a fake lookup function, with no real repository ever constructed.

csharp
// BankAccount declares a capability it needs, not a dependency
internal Func<int, BankAccount?>? AccountLookup { get; set; }

// BankRepository<T> is the one thing that actually has the capability
public T? FindByAccountNumber(int accountNumber)
    => _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

// BankingService wires them together, once, at registration
account.AccountLookup = FindByAccountNumber;
Account Types
Type	Key Feature
SavingsAccount	Fixed interest rate
CurrentAccount	Configurable per-account overdraft limit
InvestmentAccount	Multi-asset holdings, buy/sell against a live price catalog, computed portfolio value

This project is a work in progress and these are tracked, not hidden:

InvestmentAccount.SellStock has real bugs, not yet fixed:
price from PriceLookup is used in a decimal cast without a null check, unlike BuyStock's equivalent guard — a missing symbol throws an unhandled cast exception instead of a clean domain exception.
_holdings[symbol] is accessed directly before confirming the symbol exists in holdings, which throws an uncaught KeyNotFoundException if the customer never bought that asset.
The fallback branch that's meant to throw InsufficientHoldingsException references the failed TryGetValue out-variable, which is unset at that point — this throws a NullReferenceException instead of the intended exception.
InvestmentAccount.FetchHoldings() prints AssetHolding objects directly ({pair.Value}) instead of a specific property. AssetHolding has no ToString() override, so this prints the type name (BankingSystem.AssetHolding) rather than the quantity held.
A large placeholder test deposit (used to sanity-check Analytics/TotalBalance) is still present in Program.cs sample data and should be removed before this is treated as a clean demo.
IAsyncEnumerable<T> streaming (StreamTransactionsAsync) must be consumed with await foreach, not printed directly — an earlier attempt tried to WriteLine the sequence object itself, which prints a compiler-generated state machine name instead of the transactions.

Running It
bash
dotnet run

Requires .NET (developed and tested on Linux/Ubuntu with the .NET SDK).

What's Next
Span<T> usage
Push toward a real ASP.NET Core Web API version of this system, backed by this same domain model

Built as a self-directed learning project, working toward cloud-native development in C#.
