using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessagingTest
{
    public class Startup
    {
        /// <summary>
        /// Creates the dependency injection service objects
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IStorageProvider, InMemoryStorage>();
            services.AddSingleton<ICentalHub, CentralHub>();
            services.AddTransient<IDependentNode>(sp =>
            {
                var name = $"Node{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                return new DependentNode(name,
                    sp.GetRequiredService<IStorageProvider>(),
                    sp.GetRequiredService<ILogger<DependentNode>>());
            });

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
        }
    }
}
