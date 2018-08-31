using Ditat.Logic.Models;

namespace Ditat.Logic.Interfaces
{
    public interface IEANAttributeProvider
    {
        EANInfo GetEANInfo(string path);
    }
}
