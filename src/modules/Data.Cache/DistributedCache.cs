using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Ws.Core.Extensions.Data.Cache;

public class DistributedCache: ICache
{
    private const string _keyCollection = "___all_keys";
    private readonly IDistributedCache _client;
    private readonly IExpirationTier _expirationTier;
    public DistributedCache(IDistributedCache client, IExpirationTier expirationTier)
    {
        _client = client;
        _expirationTier = expirationTier;
    }

    public IEnumerable<string> Keys => Get<HashSet<string>>(_keyCollection) ?? new HashSet<string>();
    public IExpirationTier ExpirationTier => _expirationTier;
    public byte[] Get(string key) => _client.Get(key);

    public async Task<byte[]> GetAsync(string key, CancellationToken token = default) => await _client.GetAsync(key, token);
    public T? Get<T>(string key) => _get<T>(_client.Get(key));

    public async Task<T?> GetAsync<T>(string key)
    {
        var source = await _client.GetAsync(key);
        return await Task.FromResult(_get<T>(source));
    }

    private static T? _get<T>(byte[]? source)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(source ?? Array.Empty<byte>())) ?? default;
        }
        catch
        {
            return default;
        }
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options) => SetAsync(key, value, options).RunSynchronously();

    public async Task SetObjectAsync(string key, object value, DistributedCacheEntryOptions options, CancellationToken token = default)
    => await SetAsync(key, Data.Cache.Util.ObjToByte(value), options, token);

    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        await _client.SetAsync(key, value, options, token);
        if (key != _keyCollection && !Keys.Contains(key))
            await SyncKeysAsync(Keys.Append(key).ToHashSet<string>());
    }

    public void Refresh(string key) => _client.Refresh(key);

    public async Task RefreshAsync(string key, CancellationToken token = default) => await _client.RefreshAsync(key, token);

    public void Remove(string key) => RemoveAsync(key).RunSynchronously();

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        await _client.RemoveAsync(key, token);

        if (Keys.Contains(key))
            await SyncKeysAsync(Keys.Where(_ => _ != key).ToHashSet<string>());
    }

    public void Clear() => ClearAsync().RunSynchronously();

    public async Task ClearAsync(CancellationToken token = default)
    {
        foreach (var key in Keys)
            await _client.RemoveAsync(key, token);

        await SyncKeysAsync(new HashSet<string>());
    }

    private async Task SyncKeysAsync(HashSet<string> keys)
    => await SetObjectAsync(_keyCollection, keys, ExpirationTier.Never);

}
