using Installer.Data.Models;
using Installer.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Installer.Business
{
    public class InstallationChecker
    {
        private readonly ILogger<Configuration> _logger;

        private double _requiredTotalSpace;

        private double _requiredSoftwareSpace;

        private double _requiredDatabaseSpace;

        public InstallationChecker(ILogger<Configuration> logger)
        {
            _logger = logger;
        }

        public List<Dictionary<string, string>> GetInstallationConfiguration(ConfigurationInformation config, PrerequisitesInformation prerequisiteInfo)
        {
            Dictionary<string, string> installation = new Dictionary<string, string>();
            List<Dictionary<string, string>> configuration = new List<Dictionary<string, string>>();

            _requiredSoftwareSpace = prerequisiteInfo.SoftwareSpaceRequirement + 5; // 5 = hdd space buffer
            _requiredDatabaseSpace = prerequisiteInfo.DatabaseSpaceRequirement + 5;
            _requiredTotalSpace = _requiredSoftwareSpace + _requiredDatabaseSpace;

            if (config.Drives.Count == 1)
            {
                installation = CheckInstallationConfigurationOnOneDrive(Double.Parse(config.Drives[0].RemainingSpace), config.Drives[0]);
                configuration.Add(installation);
            }
            else if (config.Drives.Count == 2)
            {
                configuration = CheckInstallationConfigurationOnTwoDrives(config, prerequisiteInfo);
            }

            return configuration;
        }

        private Dictionary<string, string> CheckInstallationConfigurationOnOneDrive(double availableSpace, DriveInformation drive)
        {
            Dictionary<string, string> installation = new Dictionary<string, string>();

            if (availableSpace - _requiredTotalSpace > 0)
            {
                installation.Add(drive.Name, (availableSpace - _requiredTotalSpace).ToString());
            }
            else
            {
                installation.Add("0", "0");
            }

            return installation;
        }

        private List<Dictionary<string, string>> CheckInstallationConfigurationOnTwoDrives(ConfigurationInformation config, PrerequisitesInformation prerequisiteInfo)
        {
            Dictionary<string, string> installation = new Dictionary<string, string>();
            List<Dictionary<string, string>> configuration = new List<Dictionary<string, string>>();
            double spaceOnSystemDrive = 0d;
            double spaceOnStorageDrive = 0d;

            foreach (var drive in config.Drives)
            {
                if (drive.IsSystemDrive)
                    spaceOnSystemDrive = Double.Parse(drive.RemainingSpace);
                else
                    spaceOnStorageDrive = Double.Parse(drive.RemainingSpace);
            }

            installation = CheckInstallationConfigurationOnOneDrive(spaceOnStorageDrive, config.Drives[1]);


            if (installation.ContainsKey("0"))
            {
                configuration = CheckDividedInstallation(spaceOnStorageDrive, spaceOnSystemDrive);
                if (configuration[0].ContainsKey("0"))
                {
                    installation = CheckInstallationConfigurationOnOneDrive(spaceOnSystemDrive, config.Drives[0]);
                    configuration.Add(installation);
                }
            }
            else
            {
                configuration.Add(installation);
            }

            return configuration;
        }

        private List<Dictionary<string, string>> CheckDividedInstallation(double spaceOnStorageDrive, double spaceOnSystemDrive)
        {
            Dictionary<string, string> installation = new Dictionary<string, string>();
            List<Dictionary<string, string>> configuration = new List<Dictionary<string, string>>();

            if (spaceOnStorageDrive - _requiredSoftwareSpace > 0 && spaceOnSystemDrive - _requiredDatabaseSpace > 0)
            {
                installation.Add("Software will be installed on storage drive", (spaceOnStorageDrive - _requiredSoftwareSpace).ToString());
                configuration.Add(installation);
                installation.Add("Database will be installed on system drive", (spaceOnSystemDrive - _requiredDatabaseSpace).ToString());
                configuration.Add(installation);
            }
            else if (spaceOnStorageDrive - _requiredDatabaseSpace > 0 && spaceOnSystemDrive - _requiredSoftwareSpace > 0)
            {
                installation.Add("Software will be installed on system drive", (spaceOnStorageDrive - _requiredDatabaseSpace).ToString());
                configuration.Add(installation);
                installation.Add("Database will be installed on database drive", (spaceOnSystemDrive - _requiredSoftwareSpace).ToString());
                configuration.Add(installation);
            }
            else
            {
                installation.Add("0", "0");
                configuration.Add(installation);
            }
            return configuration;
        }

    }
}
