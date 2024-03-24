using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;

namespace FusionRocks;


public static class FusionCacheBuilderExtensions
{
    public static IFusionCacheBuilder WithFusionRocks(this IFusionCacheBuilder builder, IServiceCollection services, Action<FusionRocksOptions>? setupAction = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions();

        if (setupAction == null)
            setupAction = FusionRocksOptions.DefaultAction;

        services.Configure(setupAction);
        services.AddTransient(x => x.GetRequiredService<IOptions<FusionRocksOptions>>().Value);
        services.AddSingleton<IDistributedCache, FusionRocks>();

        var options = new FusionRocksOptions();
        setupAction(options);

        builder.WithRegisteredDistributedCache();
        return builder;
    }
}