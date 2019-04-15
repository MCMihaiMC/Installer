using Installer.Data.Models;
using Installer.Interface;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Installer.Service
{
    public class Software : ISoftware
    {
        private readonly ILogger<Configuration> _logger;

        private bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        private bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        private bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public Software(ILogger<Configuration> logger)
        {
            _logger = logger;
        }

        public SoftwareInformation GetSoftware()
        {
            _logger.LogWarning("The software installed reading started.");
            SoftwareInformation soft = new SoftwareInformation();
            soft.OS = GetOS();
            soft.IsOracle = IsOracleInstalled();

            _logger.LogWarning("The software installed reading ended.");
            return soft;
        }

        private string GetOS()
        {
            _logger.LogWarning("The OS installed checking started.");
            if (IsWindows())
                return "Windows";

            if (IsMacOS())
                return "MacOS";

            if (IsLinux())
                return "Linux";
            _logger.LogWarning("The OS installed checking ended.");

            return "Unknown OS";
        }

        private bool IsOracleInstalled()
        {
            _logger.LogWarning("The Oracle installation checking started.");
            bool isOracleClient = false;

            string result = string.Empty;
            System.Diagnostics.ProcessStartInfo proces = new System.Diagnostics.ProcessStartInfo("tnsping.exe");
            proces.RedirectStandardOutput = true;
            proces.CreateNoWindow = true;
            proces.UseShellExecute = false;
            System.Diagnostics.Process bufor;
            bufor = System.Diagnostics.Process.Start(proces);
            System.IO.StreamReader Output = bufor.StandardOutput;
            bufor.WaitForExit(2000);
            if (bufor.HasExited)
            {
                result = Output.ReadToEnd();
                result = result.ToLower();
                if (result.Contains("64-bit") || result.Contains("32-bit"))
                {
                    _logger.LogWarning("Oracle installation found.");
                    isOracleClient = true;
                }
                else
                {
                    _logger.LogWarning("Oracle installation not found.");
                }
            }

            _logger.LogWarning("The Oracle installation checking ended.");

            return isOracleClient;
        }
    }
}
