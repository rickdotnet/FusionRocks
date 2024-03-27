namespace FusionRocks;

public class CacheItem
{
    public byte[] Value { get; set; }
    public DateTimeOffset Expiration { get; set; }
}