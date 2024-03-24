using FusionRocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZiggyCreatures.Caching.Fusion;

var builder = Host.CreateApplicationBuilder(args);
var fusion = builder.Services
    .AddFusionCache()
    .WithFusionRocks(builder.Services)
    .WithDefaultEntryOptions(new FusionCacheEntryOptions(TimeSpan.FromSeconds(10)))
    .WithSystemTextJsonSerializer();

using var scope = builder.Build().Services.CreateScope();
var provider = scope.ServiceProvider;

var cache = provider.GetRequiredService<IFusionCache>();
var key = "my-key";
var value = await cache.GetOrSetAsync(key, "Initial Value");
Console.WriteLine($"Value: {value}");
await Task.Delay(5000);

value = await cache.GetOrSetAsync(key, "This won't set the value because it already exists");
Console.WriteLine($"Value: {value}");
await Task.Delay(5000);

value = await cache.GetOrSetAsync(key, "New value", new FusionCacheEntryOptions(TimeSpan.FromSeconds(10)));
Console.WriteLine($"Value: {value}");
await Task.Delay(2000);