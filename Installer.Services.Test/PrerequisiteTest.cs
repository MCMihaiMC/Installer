using Installer.Data.Models;
using Installer.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Installer.Services.Test
{
    [TestClass]
    public class PrerequisiteTest
    {
        private Mock<ILogger<Configuration>> _loggerMock;

        private  Mock<IOptions<AppSettings>> _appSettingsMock;
        
        private Prerequisite _prerequisite;

        [TestMethod]
        public void Prerequisite_GetPrerequisite_ReturnsPrerequisitesWhenOk()
        {
            // Arrange
            _loggerMock = new Mock<ILogger<Configuration>>();
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _appSettingsMock.Setup(v => v.Value).Returns(new AppSettings
            {
                 SoftwareSpaceRequirement = 70,
                 DatabaseSpaceRequirement = 40 
            });
            _prerequisite = new Prerequisite(_loggerMock.Object, _appSettingsMock.Object);

            // Act
            PrerequisitesInformation prerequisites = _prerequisite.GetPrerequisite();

            // Assert
            Assert.IsTrue(prerequisites != null);
            Assert.IsTrue(prerequisites.DatabaseSpaceRequirement.Equals(40));
            Assert.IsTrue(prerequisites.SoftwareSpaceRequirement.Equals(70));
        }

        [TestMethod]
        public void Prerequisite_GetPrerequisite_ReturnsZeroWhenNotSet()
        {
            // Arrange
            _loggerMock = new Mock<ILogger<Configuration>>();
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            int notSet = 0;
            _appSettingsMock.Setup(v => v.Value).Returns(new AppSettings
            {
            });

            _prerequisite = new Prerequisite(_loggerMock.Object, _appSettingsMock.Object);

            // Act
            PrerequisitesInformation prerequisites = _prerequisite.GetPrerequisite();

            // Assert
            Assert.IsTrue(prerequisites != null);
            Assert.IsTrue(prerequisites.DatabaseSpaceRequirement.Equals(notSet));
            Assert.IsTrue(prerequisites.SoftwareSpaceRequirement.Equals(notSet));
        }
    }
}
