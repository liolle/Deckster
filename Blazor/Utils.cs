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

    public async Task WaitAsync(int randomId)
    {
        await _semaphore.WaitAsync();
        //Console.WriteLine($"Waiting: {randomId}: {_name}");
        lock (_owners)
        {
            _owners.Add(randomId);
        }
    }
     
    public void Release(int randomId)
    {
        //Console.WriteLine($"Releasing: {randomId}: {_name}");
        lock (_owners)
        {
            if (!_owners.Contains(randomId)) { return; }
            _semaphore.Release();
            _owners.Remove(randomId);
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}