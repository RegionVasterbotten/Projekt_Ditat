using System.IO;
using Ditat.Logic.Models;

namespace Ditat.Logic.Interfaces
{
    public interface IClassifierProvider
    {
        ClassifierResponse ClassifyImage(FileInfo imageFileInfo);
        
    }
}
