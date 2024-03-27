using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;

namespace FusionRocks;

public static class FusionCacheBuilderExtensions
{
    public static IFusionCacheBuilder WithFusionRocks(
        this IFusionCacheBuilder builder,
        IServiceCollection services,
        Action<FusionRocksOptions>? setupAction = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions();

        setupAction ??= FusionRocksOptions.DefaultAction;

        services.Configure(setupAction);
        services.AddTransient(x => x.GetRequiredService<IOptions<FusionRocksOptions>>().Value);

        // if no serializer, hope for the best?
        if (builder.Serializer == null)
            services.AddSingleton<IDistributedCache, FusionRocks>();
        else
            services.AddSingleton<IDistributedCache>(x =>
                new FusionRocks(x.GetRequiredService<FusionRocksOptions>(), builder.Serializer!));

        builder.WithRegisteredDistributedCache();
        return builder;
    }
}