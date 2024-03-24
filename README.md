# FusionRocks: RocksDB-Persisted Cache for FusionCache

FusionRocks is a persistent caching solution for .NET applications that integrates with the powerful FusionCache library. It leverages RocksDB, a high-performance embedded database for key-value data, to store cache data on disk, providing a robust and efficient caching mechanism. FusionRocks implements the `IDistributedCache` interface for compatibility and ease of use, making it an ideal choice for scenarios requiring data durability and high throughput.

## Features

- **Persistence**: Utilizes RocksDB for durable cache storage, ensuring that cached data is preserved across application restarts and crashes.
- **FusionCache Integration**: Designed to work seamlessly with [ZiggyCreatures' FusionCache](https://github.com/ZiggyCreatures/FusionCache), taking advantage of its sophisticated caching strategies and policies.
- **High Performance**: RocksDB's optimized storage engine provides fast read and write operations, suitable for high-load environments.
- **Configurable**: Offers customizable options to tailor the RocksDB instance, including database path, compression, and caching behaviors.
- **Asynchronous Support**: Provides async methods that follow the async/await pattern in .NET for non-blocking cache operations.
- **Ease of Setup**: Can be easily configured with a fluent API, making it simple to integrate into your .NET projects.

## Installation

To start using FusionRocks in your application, install the package via your preferred NuGet package manager:

```sh
Install-Package RickDotNet.FusionRocks
```

## Configuration

In your application's startup configuration, register FusionRocks with the service collection as follows:

```csharp
// In your ConfigureServices method or wherever you register DI services
var fusion = builder.Services  
    .AddFusionCache()  
    .WithFusionRocks(builder.Services, options =>
    {
        options.CachePath = "fusionrocks.db";
    })
    .WithDefaultEntryOptions(new FusionCacheEntryOptions(TimeSpan.FromMinutes(2)));
```

## Usage

After configuring FusionRocks, you can interact with FusionCache as usual:

```csharp
var userId = 12345;

var user = cache.GetOrSet<User>(
    $"user:{userId}",
    _ => GetUserFromDatabase(userId),
    TimeSpan.FromSeconds(30)
);
```

## License

FusionRocks is released under the [MIT License](https://opensource.org/licenses/MIT), allowing you to freely use, modify, and distribute the software within the confines of the license.