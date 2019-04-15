using System;
using System.Collections.Generic;
using System.Text;

namespace Installer.Data.Models
{
    public class DriveInformation
    {
        public string Name { get; set; }

        public string PartitionType { get; set; }

        public string TotalSpace { get; set; }

        public string RemainingSpace { get; set; }

        public bool IsSystemDrive { get; set; }
    }
}
