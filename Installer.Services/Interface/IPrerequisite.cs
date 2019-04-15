using Installer.Data.Models;

namespace Installer.Interface
{
    public interface IPrerequisite
    {
        PrerequisitesInformation GetPrerequisite();
    }
}
