using Installer.Data.Models;
using Installer.Service;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Installer.Services.Test
{
    [TestClass]
    public class SoftwareTest
    {
        private Mock<ILogger<Configuration>> _loggerMock;

        private Software _software;

        [TestMethod]
        public void Software_GetSoftware_ReturnsSoftwareWhenInstalled()
        {
            // Arrange
            _loggerMock = new Mock<ILogger<Configuration>>();
            _software = new Software(_loggerMock.Object);

            // Act
            SoftwareInformation soft = _software.GetSoftware();

            // Assert
            Assert.IsTrue(soft != null);
        }
    }
}
