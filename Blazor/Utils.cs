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
        string? id_str = claims.FirstOrDefault(val => val.Type == type)?.Value;
        if (id_str is null || !int.TryParse(id_str, out int id)) { return null; }
        return id;
    }

}

public class OwnedSemaphore
{
    private readonly SemaphoreSlim _semaphore;
    private readonly HashSet<object> _owners;

    public OwnedSemaphore(int initialCount, int maxCount)
    {
        _semaphore = new SemaphoreSlim(initialCount, maxCount);
        _owners = new HashSet<object>();
    }

    public async Task WaitAsync()
    {
        int? CurrentId = Task.CurrentId;
        if (CurrentId is null) { return; }

        await _semaphore.WaitAsync();
        lock (_owners)
        {
            _owners.Add(CurrentId);
        }
    }

    public void Release()
    {
        int? CurrentId = Task.CurrentId;
        if (CurrentId is null) { return; }
        lock (_owners)
        {
            if (!_owners.Contains(CurrentId)) { return; }

            _semaphore.Release();
            _owners.Remove(CurrentId);
        }
    }
}