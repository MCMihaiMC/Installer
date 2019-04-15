using Installer.Business;
using Installer.Data.Models;
using Installer.Service;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Installer.Data.Test
{
    [TestClass]
    public class InstallationCheckerTest
    {
        private ILogger<Configuration> _logger;

        private InstallationChecker _installationChecker;

        [TestMethod]
        public void InstallationChecker_GetInstallationConfiguration_OnOneDrive_ReturnsDone()
        {
            // Arrange
            ConfigurationInformation config = GetOneDriveConfigurationOk();
            PrerequisitesInformation prerequisite = GetPrerequisite();
            double fullSpaceNeeded = prerequisite.DatabaseSpaceRequirement + prerequisite.SoftwareSpaceRequirement + 10;
            double expected = Double.Parse(config.Drives[0].RemainingSpace) - fullSpaceNeeded;

            _installationChecker = new InstallationChecker(_logger); 

            // Act
            List<Dictionary<string, string>> actual = _installationChecker.GetInstallationConfiguration(config, prerequisite);

            // Assert
            Dictionary<string, string> element = actual[0];
            Assert.AreEqual(expected, Double.Parse(element.FirstOrDefault().Value));
        }

        [TestMethod]
        public void InstallationChecker_GetInstallationConfiguration_OnOneDrive_ReturnsCannotInstall()
        {
            // Arrange
            ConfigurationInformation config = GetOneDriveConfigurationTooSmall();
            PrerequisitesInformation prerequisite = GetPrerequisite();
            double fullSpaceNeeded = prerequisite.DatabaseSpaceRequirement + prerequisite.SoftwareSpaceRequirement + 10;
            string expected = "0";

            _installationChecker = new InstallationChecker(_logger);

            // Act
            List<Dictionary<string, string>> actual = _installationChecker.GetInstallationConfiguration(config, prerequisite);

            // Assert
            Dictionary<string, string> element = actual[0];
            Assert.AreEqual(expected, element.FirstOrDefault().Value);
        }

        [TestMethod]
        public void InstallationChecker_GetInstallationConfiguration_OnTwoDrives_ReturnsOkOnStorageDrive()
        {
            // Arrange
            ConfigurationInformation config = GetTwoDrivesConfigurationOkStorageDrive();
            PrerequisitesInformation prerequisite = GetPrerequisite();
            double fullSpaceNeeded = prerequisite.DatabaseSpaceRequirement + prerequisite.SoftwareSpaceRequirement + 10;
            double expected = Double.Parse(config.Drives[1].RemainingSpace) - fullSpaceNeeded;

            _installationChecker = new InstallationChecker(_logger);

            // Act
            List<Dictionary<string, string>> actual = _installationChecker.GetInstallationConfiguration(config, prerequisite);

            // Assert
            Dictionary<string, string> element = actual[0];
            Assert.AreEqual(expected, Double.Parse(element.FirstOrDefault().Value));
        }

        [TestMethod]
        public void InstallationChecker_GetInstallationConfiguration_OnTwoDrives_ReturnsCannotInstall()
        {
            // Arrange
            ConfigurationInformation config = GetTwoDrivesConfigurationNotOkOnBothDrives();
            PrerequisitesInformation prerequisite = GetPrerequisite();
            double fullSpaceNeeded = prerequisite.DatabaseSpaceRequirement + prerequisite.SoftwareSpaceRequirement + 10;
            string expected = "0";

            _installationChecker = new InstallationChecker(_logger);

            // Act
            List<Dictionary<string, string>> actual = _installationChecker.GetInstallationConfiguration(config, prerequisite);

            // Assert
            Dictionary<string, string> element1 = actual[0];
            Dictionary<string, string> element2 = actual[1];
            Assert.AreEqual(expected, element1.FirstOrDefault().Value);
            Assert.AreEqual(expected, element2.FirstOrDefault().Value);
        }

        [TestMethod]
        public void InstallationChecker_GetInstallationConfiguration_OnTwoDrives_ReturnsSplittedInstall()
        {
            // Arrange
            ConfigurationInformation config = GetTwoDrivesConfigurationOkSplittedInstall();
            PrerequisitesInformation prerequisite = GetPrerequisite();
            double expectedOnSystem = Double.Parse(config.Drives[0].RemainingSpace) - prerequisite.SoftwareSpaceRequirement - 5;
            double expectedOnStorage = Double.Parse(config.Drives[1].RemainingSpace) - prerequisite.DatabaseSpaceRequirement - 5;

            _installationChecker = new InstallationChecker(_logger);

            // Act
            List<Dictionary<string, string>> actual = _installationChecker.GetInstallationConfiguration(config, prerequisite);

            // Assert
            Dictionary<string, string> element1 = actual[0];
            Dictionary<string, string> element2 = actual[1];
            Assert.AreEqual(expectedOnSystem.ToString(), element1.FirstOrDefault().Value);
            Assert.AreEqual(expectedOnStorage.ToString(), element2.FirstOrDefault().Value);
        }

        private ConfigurationInformation GetOneDriveConfigurationOk()
        {
            ConfigurationInformation configuration = new ConfigurationInformation();
            DriveInformation drive = new DriveInformation();
            configuration.Drives = new List<DriveInformation>();

            drive.Name = "Test1";
            drive.PartitionType = "NTFS";
            drive.TotalSpace = "250";
            drive.RemainingSpace = "120";
            drive.IsSystemDrive = true;

            configuration.Drives.Add(drive);

            return configuration;
        }

        private ConfigurationInformation GetOneDriveConfigurationTooSmall()
        {
            ConfigurationInformation configuration = new ConfigurationInformation();
            DriveInformation drive = new DriveInformation();
            configuration.Drives = new List<DriveInformation>();

            drive.Name = "Test1";
            drive.PartitionType = "NTFS";
            drive.TotalSpace = "250";
            drive.RemainingSpace = "60";
            drive.IsSystemDrive = true;

            configuration.Drives.Add(drive);

            return configuration;
        }

        private ConfigurationInformation GetTwoDrivesConfigurationOkStorageDrive()
        {
            ConfigurationInformation configuration = new ConfigurationInformation();
            DriveInformation drive1 = new DriveInformation();
            DriveInformation drive2 = new DriveInformation();
            configuration.Drives = new List<DriveInformation>();

            drive1.Name = "Test1";
            drive1.PartitionType = "NTFS";
            drive1.TotalSpace = "250";
            drive1.RemainingSpace = "120";
            drive1.IsSystemDrive = true;

            configuration.Drives.Add(drive1);

            drive2.Name = "Test2";
            drive2.PartitionType = "NTFS";
            drive2.TotalSpace = "500";
            drive2.RemainingSpace = "140";
            drive2.IsSystemDrive = false;

            configuration.Drives.Add(drive2);

            return configuration;
        }

        private ConfigurationInformation GetTwoDrivesConfigurationNotOkOnBothDrives()
        {
            ConfigurationInformation configuration = new ConfigurationInformation();
            DriveInformation drive1 = new DriveInformation();
            DriveInformation drive2 = new DriveInformation();
            configuration.Drives = new List<DriveInformation>();

            drive1.Name = "Test1";
            drive1.PartitionType = "NTFS";
            drive1.TotalSpace = "250";
            drive1.RemainingSpace = "20";
            drive1.IsSystemDrive = true;

            configuration.Drives.Add(drive1);

            drive2.Name = "Test2";
            drive2.PartitionType = "NTFS";
            drive2.TotalSpace = "500";
            drive2.RemainingSpace = "20";
            drive2.IsSystemDrive = false;

            configuration.Drives.Add(drive2);

            return configuration;
        }

        private ConfigurationInformation GetTwoDrivesConfigurationOkSplittedInstall()
        {
            ConfigurationInformation configuration = new ConfigurationInformation();
            DriveInformation drive1 = new DriveInformation();
            DriveInformation drive2 = new DriveInformation();
            configuration.Drives = new List<DriveInformation>();

            drive1.Name = "Test1";
            drive1.PartitionType = "NTFS";
            drive1.TotalSpace = "250";
            drive1.RemainingSpace = "60";
            drive1.IsSystemDrive = true;

            configuration.Drives.Add(drive1);

            drive2.Name = "Test2";
            drive2.PartitionType = "NTFS";
            drive2.TotalSpace = "500";
            drive2.RemainingSpace = "40";
            drive2.IsSystemDrive = false;

            configuration.Drives.Add(drive2);

            return configuration;
        }

        private PrerequisitesInformation GetPrerequisite()
        {
            PrerequisitesInformation prerequisite = new PrerequisitesInformation();
            prerequisite.SoftwareSpaceRequirement = 50;
            prerequisite.DatabaseSpaceRequirement = 30;

            return prerequisite;
        }
        
    }
}
