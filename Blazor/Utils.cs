using System.Security.Claims;

namespace Blazor.utils;

public static class Utils
{
    public static async Task<T?> ExBackoff<T>(Func<T?> operation, int maxRetries = 5, int initialDelay = 1)
    {
        await Task.Delay(initialDelay);

        for (int i = 0; i < maxRetries; i++)
        {
            int round = (int)Math.Ceiling(Math.Pow(2, i));
            await Task.Delay(round);
            T? u = operation();
            if (u is not null)
            {
                return u;
            }
        }
        return default;
    }

    public static int? ExtractIntFromClaim(IEnumerable<Claim> claims, string type)
    {
        string? idStr = claims.FirstOrDefault(val => val.Type == type)?.Value;
        if (idStr is null || !int.TryParse(idStr, out int id)) { return null; }
        return id;
    }

}

public class OwnedSemaphore : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private readonly HashSet<object> _owners;
    private readonly string _name;

    public OwnedSemaphore(int initialCount, int maxCount,string name)
    {
        _semaphore = new SemaphoreSlim(initialCount, maxCount);
        _owners = new HashSet<object>();
        _name = name;
    }

    public async Task WaitAsync()
    {
            await _semaphore.WaitAsync();
            int id = Environment.CurrentManagedThreadId;    
            lock (_owners)
            {
                _owners.Add(id);
            }

    }

    public void Release()
    {
        
        int id = Environment.CurrentManagedThreadId;    
        lock (_owners)
        {
            if (!_owners.Contains(id)) { return; }
            _semaphore.Release();
            _owners.Remove(id);
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}