using Ditat.Logic.Models;

namespace Ditat.Logic
{
    public interface ICDAttributeProvider
    {
        CDInfo GetCDInfo(string path);
    }
}
