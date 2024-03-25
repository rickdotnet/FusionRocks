using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RocksDbSharp;

namespace FusionRocks;

public sealed class FusionRocks : IDistributedCache, IDisposable
{
    private readonly ILogger logger; // TODO: use me
    private readonly RocksDb rocksDb;

    public FusionRocks(FusionRocksOptions options, ILogger<FusionRocks>? logger = null)
    {
        this.logger = logger ?? NullLogger<FusionRocks>.Instance;
        rocksDb = RocksDb.Open(options.DbOptions, options.CachePath);
    }

    public byte[]? Get(string key) 
        => rocksDb.Get(key.ToBytes());

    public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        => Task.Run(() => Get(key), token);

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options) 
        => rocksDb.Put(key.ToBytes(), value);

    /// <summary>
    /// This is a FusionRocks specific method that allows you to set multiple keys at once.
    /// </summary>
    public void SetBatch(IDictionary<string, byte[]> values)
    {
        using var batch = new WriteBatch();

        foreach (var (key, value) in values)
            batch.Put(key.ToBytes(), value);

        rocksDb.Write(batch);
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        => Task.Run(() => Set(key, value, options), token);

    public void Refresh(string key)
        => UnusedMethodException.Throw();

    public Task RefreshAsync(string key, CancellationToken token = default)
        => throw new UnusedMethodException();

    public void Remove(string key) 
        => rocksDb.Remove(key.ToBytes());

    public Task RemoveAsync(string key, CancellationToken token = default)
        => Task.Run(() => Remove(key), token);

    public void Dispose() 
        => rocksDb.Dispose();
}

internal static class StringExtensions
{
    public static byte[] ToBytes(this string value)
        => Encoding.UTF8.GetBytes(value);
}