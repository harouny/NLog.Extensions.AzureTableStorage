using System;
using Microsoft.Extensions.DependencyInjection;

namespace NLog.Extensions.AzureTableStorage
{
    public static class AtsCoreServiceCollectionExtensions
    {
        public static void ConfigureAts(this IServiceCollection services,
                                        [NotNull] Action<AtsOptions> setupAction)
        {
            var options = AtsOptions.Instance;
            setupAction(options);
        }
    }
}
