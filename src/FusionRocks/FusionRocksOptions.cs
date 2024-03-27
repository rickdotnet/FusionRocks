using System.Text;
using Microsoft.Extensions.Options;
using RocksDbSharp;

namespace FusionRocks;

public record FusionRocksOptions : IOptions<FusionRocksOptions>
{
    public static FusionRocksOptions Default { get; } = new();
    public static Action<FusionRocksOptions> DefaultAction { get; } = _ => { };
    FusionRocksOptions IOptions<FusionRocksOptions>.Value => this;
    public DbOptions DbOptions { get; init; }
    public string CachePath { get; init; } = "FusionRocks";
    public Encoding KeyEncoding { get; init; } = Encoding.UTF8;
    
    // I don't know that this would ever be used; fusion cache handles expiration
    public TimeSpan DefaultExpiration { get; init; } = TimeSpan.FromMinutes(20); 

    public FusionRocksOptions() : this(new DbOptions().SetCreateIfMissing()){ }
    public FusionRocksOptions(DbOptions options)
    {
        DbOptions = options;
    }
}