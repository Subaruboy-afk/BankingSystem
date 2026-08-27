namespace BankingSystem;

public class BankSessionManager : IDisposable
{
    private string _name;
    private bool _disposed = false;

    public BankSessionManager(string name)
    {
        _name = name;
        System.Console.WriteLine("[Managing Resources]");
    }

    public void LogSession()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(_name);
        }
        System.Console.WriteLine($"[{_name}] Doing work");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // this is usually where I'd dispose of other IDisposable objects this class owns - someThing.Dispose()
            }
            System.Console.WriteLine($"[{_name}] Session Ended");
            _disposed = true;
        }
    }

    ~BankSessionManager()
    {
        Dispose(false);
        System.Console.WriteLine("Finalizer ran!.");
    }
}