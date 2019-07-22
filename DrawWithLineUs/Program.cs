using Microsoft.Extensions.DependencyInjection;

using DrawWithLineUs.Service;

namespace DrawWithLineUs.Con
{
    class Program
    {
        static void Main()
        {
            // Create service collection and configure our services
            var services = ConfigureServices();

            // Generate a service provider
            var serviceProvider = services.BuildServiceProvider();

            // Kick off our actual code
            serviceProvider.GetService<ConsoleApplication>().Run();
        }


        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddScoped<ICommunicationService, CommunicationService>();
            services.AddScoped<IGCodeService, GCodeService>();
            services.AddScoped<IGeometryService, GeometryService>();
            services.AddScoped<ISvgService, SvgService>();

            services.AddScoped<ConsoleApplication>();
            return services;
        }
    }
}
