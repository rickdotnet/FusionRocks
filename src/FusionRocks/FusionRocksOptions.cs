using Microsoft.Extensions.Options;
using RocksDbSharp;

namespace FusionRocks;

public record FusionRocksOptions : IOptions<FusionRocksOptions>
{
    public static FusionRocksOptions Default { get; } = new();
    public static Action<FusionRocksOptions> DefaultAction { get; } = _ => { };
    FusionRocksOptions IOptions<FusionRocksOptions>.Value => this;
    public DbOptions DbOptions { get; init; }
    public string CachePath { get; init; } = "FusionRocksCache.db";

    public FusionRocksOptions(): this(new DbOptions().SetCreateIfMissing()) { }
    public FusionRocksOptions(DbOptions options)
    {
        DbOptions = options;
    }
}