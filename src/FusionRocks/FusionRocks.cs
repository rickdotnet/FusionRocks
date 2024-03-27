using Microsoft.Extensions.Caching.Distributed;
using RocksDbSharp;
using ZiggyCreatures.Caching.Fusion.Serialization;

namespace FusionRocks;

public sealed class FusionRocks : IDistributedCache, IDisposable
{
    private readonly FusionRocksOptions options;
    private readonly IFusionCacheSerializer serializer;
    private readonly RocksDb rocksDb;

    public FusionRocks(FusionRocksOptions options, IFusionCacheSerializer serializer)
    {
        this.options = options;
        this.serializer = serializer;
        rocksDb = RocksDb.Open(options.DbOptions, options.CachePath);
    }

    public byte[]? Get(string key)
    {
        var keyBytes = options.KeyEncoding.GetBytes(key);
        var value = rocksDb.Get(keyBytes);

        if (value is null)
            return null;

        var cacheItem = serializer.Deserialize<CacheItem>(value);
        if (DateTimeOffset.UtcNow <= cacheItem?.Expiration)
            return cacheItem.Value;

        Remove(keyBytes);
        return null;
    }

    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        var keyBytes = options.KeyEncoding.GetBytes(key);
        var value = rocksDb.Get(keyBytes);

        if (value is null)
            return null;

        var cacheItem = await serializer.DeserializeAsync<CacheItem>(value);
        if (DateTimeOffset.UtcNow <= cacheItem?.Expiration)
            return cacheItem.Value;

        Remove(keyBytes);
        return null;
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions cacheOptions)
    {
        var keyBytes = options.KeyEncoding.GetBytes(key);
        var cacheItem = CreateCacheItem(value, cacheOptions);

        rocksDb.Put(keyBytes, serializer.Serialize(cacheItem));
    }
    
    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions cacheOptions,
        CancellationToken token = default)
    {
        var keyBytes = options.KeyEncoding.GetBytes(key);
        var cacheItem = CreateCacheItem(value, cacheOptions);

        rocksDb.Put(keyBytes, await serializer.SerializeAsync(cacheItem));
    }
    private CacheItem CreateCacheItem(byte[] value, DistributedCacheEntryOptions cacheOptions)
    {
        var cacheItem = new CacheItem
        {
            Value = value,
            Expiration = DateTimeOffset.UtcNow.Add(
                cacheOptions.AbsoluteExpirationRelativeToNow
                ?? cacheOptions.SlidingExpiration
                ?? options.DefaultExpiration
            )
        };

        return cacheItem;
    }
    public void Refresh(string key)
        => throw new UnusedMethodException();

    public Task RefreshAsync(string key, CancellationToken token = default)
        => throw new UnusedMethodException();

    public void Remove(string key)
        => Remove(options.KeyEncoding.GetBytes(key));

    private void Remove(byte[] key)
        => rocksDb.Remove(key);

    public Task RemoveAsync(string key, CancellationToken token = default)
        => Task.Run(() => Remove(key), token);

    public void Dispose()
        => rocksDb.Dispose();
}