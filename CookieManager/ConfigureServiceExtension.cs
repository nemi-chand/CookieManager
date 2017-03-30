using CookieManager;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Extension methods for setting up Cookie manager services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ConfigureServiceExtension
    {
        /// <summary>
        /// Adds Cookie manager services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddCookieManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(ServiceDescriptor.Transient<ICookie, HttpCookie>());
            services.TryAdd(ServiceDescriptor.Transient<ICookieManager, DefaultCookieManager>());

            return services;
        }
    }
}
