using Microsoft.Extensions.Caching.Distributed;

namespace FusionRocks.Tests;

public class FusionRocksIntegrationTests : IDisposable
{
    private readonly FusionRocks fusionRocks;
    private readonly string testDbPath;

    public FusionRocksIntegrationTests()
    {
        // Create a unique path for each test instance
        testDbPath = $"TestRocksDb_{Guid.NewGuid()}";
        var options = new FusionRocksOptions
        {
            CachePath = testDbPath
        };
        fusionRocks = new FusionRocks(options);
    }

    [Fact]
    public void Get_ReturnsNull_WhenKeyDoesNotExist()
    {
        // Arrange
        var key = "non-existing-key";

        // Act
        var value = fusionRocks.Get(key);

        // Assert
        Assert.Null(value);
    }

    [Fact]
    public void SetAndGet_RoundTrip_WorksCorrectly()
    {
        // Arrange
        var key = "key";
        var expectedValue = "value"u8.ToArray();

        // Act
        fusionRocks.Set(key, expectedValue, new DistributedCacheEntryOptions());
        var actualValue = fusionRocks.Get(key);

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void RemoveAsync_RemovesKey_WhenKeyExists()
    {
        // Arrange
        var key = "key-to-remove";
        var value = "value"u8.ToArray();

        fusionRocks.Set(key, value, new DistributedCacheEntryOptions());

        // Act
        fusionRocks.Remove(key);
        var valueAfterRemove = fusionRocks.Get(key);

        // Assert
        Assert.Null(valueAfterRemove);
    }

    public void Dispose()
    {
        // Clean up the RocksDB instance
        fusionRocks.Dispose();

        // Optionally delete the database files if you don't need them for debugging
        Directory.Delete(testDbPath, recursive: true);
    }
}