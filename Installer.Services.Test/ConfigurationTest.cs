using Installer.Data.Models;
using Installer.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace Installer.Services.Test
{
    [TestClass]
    public class ConfigurationTest
    {
        private Mock<ILogger<Configuration>> _loggerMock;

        private Mock<IOptions<AppSettings>> _appSettingsMock;

        private Configuration _configuration;

        [TestMethod]
        public void Configuration_GetConfiguration_ReturnsConfiguration()
        {
            // Arrange
            _loggerMock = new Mock<ILogger<Configuration>>();
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _appSettingsMock.Setup(v => v.Value).Returns(new AppSettings
            {
                SoftwareSpaceRequirement = 50,
                DatabaseSpaceRequirement = 30
            });
            _configuration = new Configuration(_loggerMock.Object, _appSettingsMock.Object);

            // Act
            ConfigurationInformation configuration = _configuration.GetConfiguration();

            // Assert
            Assert.IsTrue(configuration != null);
        }

        [TestMethod]
        public void Configuration_Getdrives_ReturnsDrives()
        {
            // Arrange
            _loggerMock = new Mock<ILogger<Configuration>>();
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _appSettingsMock.Setup(v => v.Value).Returns(new AppSettings
            {
                SoftwareSpaceRequirement = 50,
                DatabaseSpaceRequirement = 30
            });
            _configuration = new Configuration(_loggerMock.Object, _appSettingsMock.Object);

            // Act
            List<DriveInformation> drives = _configuration.GetDrives();

            // Assert
            Assert.IsTrue(drives != null);
        }
    }
}
