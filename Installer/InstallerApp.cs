using Installer.Business;
using Installer.Data.Models;
using Installer.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Installer
{
    public class InstallerApp
    {
        private readonly IConfiguration _configuration;

        private readonly ISoftware _software;

        private readonly IPrerequisite _prerequisite;

        private readonly ILogger<InstallerApp> _logger;

        private readonly AppSettings _settings;

        private readonly InstallationChecker _installationChecker;

        public InstallerApp(IConfiguration configuration, ISoftware software, IPrerequisite prerequisite,
            IOptions<AppSettings> settings,
            ILogger<InstallerApp> logger,
            InstallationChecker installationChecker)
        {
            _configuration = configuration;
            _software = software;
            _prerequisite = prerequisite;

            _logger = logger;
            _settings = settings.Value;
            _installationChecker = installationChecker;
        }

        public void Run()
        {
            _logger.LogInformation("The installer started.");
            Console.WriteLine($"Installation started at: {DateTime.Now.ToLongDateString()}  {DateTime.Now.ToShortTimeString()}");

            try
            {
                ConfigurationInformation configInfo = _configuration.GetConfiguration();
                DisplayConfiguration(configInfo);

                if (configInfo.Drives != null)
                {
                    SoftwareInformation softwareInfo = _software.GetSoftware();
                    DisplaySoftware(softwareInfo);

                    PrerequisitesInformation prerequisiteInfo = _prerequisite.GetPrerequisite();
                    DisplayPrerequisites(prerequisiteInfo);


                    List<Dictionary<string, string>> installationSchema  =_installationChecker.GetInstallationConfiguration(configInfo, prerequisiteInfo);
                    DisplayOptimumInstallationMode(installationSchema);
                }
                else
                {
                    string message = "The installation has been aborted. No drives found.";
                    _logger.LogInformation(message);
                    Console.WriteLine(message);
                }

                Console.WriteLine($"Press any key to continue");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                string message = "The installation has been aborted. Errors found. Please check your system.";
                _logger.LogInformation(message);
                throw new Exception(message);
            }

            Console.WriteLine($"Installation ended at: {DateTime.Now.ToLongDateString()}  {DateTime.Now.ToShortTimeString()}");
            Console.ReadLine();
        }
        
        private void DisplayConfiguration(ConfigurationInformation config)
        {
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("Drives information");
            Console.WriteLine("===================");

            foreach (DriveInformation drive in config.Drives)
            {
                Console.WriteLine(drive.Name);
                Console.WriteLine(drive.PartitionType);
                Console.WriteLine(drive.IsSystemDrive ? "OS partition" : "Non-OS partition");
                Console.WriteLine($"Total space: { drive.TotalSpace} | Free space: {drive.RemainingSpace}");
                Console.WriteLine("------------------------------------------------------");
            }
        }
        
        private void DisplaySoftware(SoftwareInformation softwareInfo)
        {
            Console.WriteLine("");
            Console.WriteLine("System information");
            Console.WriteLine("===================");
            Console.WriteLine($"Operating system: {softwareInfo.OS}");
            string oracleInstalled = softwareInfo.IsOracle ? "Yes" : "No";
            Console.WriteLine($"Is Oracle installed : {oracleInstalled}");
            Console.WriteLine("------------------------------------------------------");
        }

        private void DisplayPrerequisites(PrerequisitesInformation prerequisiteInfo)
        {
            Console.WriteLine("");
            Console.WriteLine("Installation prerequisites");
            Console.WriteLine("===================");
            Console.WriteLine($"Space needed for database: {prerequisiteInfo.DatabaseSpaceRequirement}");
            Console.WriteLine($"Space needed for software: {prerequisiteInfo.SoftwareSpaceRequirement}");
            Console.WriteLine("------------------------------------------------------");

        }

        private void DisplayOptimumInstallationMode(List<Dictionary<string, string>> installationSchema)
        {
            Console.WriteLine("");
            Console.WriteLine("Installation schema");
            Console.WriteLine("===================");
            if (installationSchema.Count > 1)
            {
                foreach(var drive in installationSchema)
                {
                    Console.WriteLine($"{drive.FirstOrDefault().ToString()}. Space remaining after installation: {drive.FirstOrDefault().Value} GB.");
                }
            }
            else
            {
                Dictionary<string, string> element = installationSchema[0];
                var key = element.FirstOrDefault().ToString().Split('[')[1].Split(',')[0];
                var value = element.FirstOrDefault().Value;
                Console.WriteLine($"Software and database will be installed on the same drive: {key}. Space remaining after installation: {value} GB.");
            }
            Console.WriteLine("------------------------------------------------------");
        }
        
    }
}
