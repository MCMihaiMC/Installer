using System.IO;
using Installer.Business;
using Installer.Data.Models;
using Installer.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Installer.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            
            var serviceProvider = 
                new ServiceCollection().AddLogging(cfg => cfg.AddConsole()).
                Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = 
                LogLevel.Debug).BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogDebug("Starting installer.");

            serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider.GetService<InstallerApp>().Run();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging();

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("app-configuration.json", false)
            .Build();
            serviceCollection.AddOptions();
            serviceCollection.Configure<AppSettings>(configuration.GetSection("Configuration"));

            serviceCollection.AddTransient<Interface.IConfiguration, Configuration>();
            serviceCollection.AddTransient<Interface.ISoftware, Software>();
            serviceCollection.AddTransient<Interface.IPrerequisite, Prerequisite>();

            serviceCollection.AddTransient<Interface.IPrerequisite, Prerequisite>();

            serviceCollection.AddTransient<InstallerApp>();
            serviceCollection.AddTransient<InstallationChecker>();
            serviceCollection.AddTransient<PrerequisitesInformation>();
        }
    }
}
