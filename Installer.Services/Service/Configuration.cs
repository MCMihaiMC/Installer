using Installer.Data.Models;
using Installer.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace Installer.Service
{
    public class Configuration:IConfiguration
    {
        private readonly ILogger<Configuration> _logger;

        private readonly AppSettings _settings;

        public Configuration(ILogger<Configuration> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public ConfigurationInformation GetConfiguration()
        {
            _logger.LogWarning("The configuration reading started.");

            ConfigurationInformation configuration = new ConfigurationInformation();
            configuration.Drives = GetDrives();

            _logger.LogWarning("The configuration reading ended.");
            return configuration;
        }

        public List<DriveInformation> GetDrives()
        {
            List<DriveInformation> drives = new List<DriveInformation>();
            DriveInformation driveInfo = new DriveInformation();

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (var drive in allDrives)
            {
                driveInfo = GetDriveInfo(drive);

                drives.Add(driveInfo);
            }

            return drives;
        }

        private DriveInformation GetDriveInfo(DriveInfo selectedDrive)
        {
            DriveInformation drive = new DriveInformation();
            drive.Name = selectedDrive.Name.ToString();
            drive.PartitionType = selectedDrive.DriveFormat.ToString();
            drive.TotalSpace = BytesToGb(selectedDrive.TotalSize).ToString("0.00");
            drive.RemainingSpace = BytesToGb(selectedDrive.TotalFreeSpace).ToString("0.00");
            drive.IsSystemDrive = IsSystemDrive(drive);

            return drive;
        }

        private static bool IsSystemDrive(DriveInformation drive)
        {
            string driveLetter = drive.Name.Split(':')[0];
            string systemLogicalDiskDeviceId = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 2);
            return systemLogicalDiskDeviceId.Contains(driveLetter) ? true : false;
        }

        private double BytesToGb(long bytes)
        {
            return  (long)bytes/1024d/1024d/1024d;
        }
    }
}
