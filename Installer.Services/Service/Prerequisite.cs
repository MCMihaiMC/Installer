using Installer.Data.Models;
using Installer.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Installer.Service
{
    public class Prerequisite : IPrerequisite
    {
        private readonly ILogger<Configuration> _logger;

        private readonly AppSettings _settings;

        public Prerequisite(ILogger<Configuration> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public PrerequisitesInformation GetPrerequisite()
        {
            _logger.LogWarning("The prerequisite reading started.");

            PrerequisitesInformation prerequisite = new PrerequisitesInformation();
            prerequisite.DatabaseSpaceRequirement = _settings.DatabaseSpaceRequirement;
            prerequisite.SoftwareSpaceRequirement = _settings.SoftwareSpaceRequirement;

            _logger.LogWarning("The prerequisite reading ended.");
            return prerequisite;
        }
    }
}
