using System.IO;
using Ditat.Logic.Models;

namespace Ditat.Logic.Interfaces
{
    public interface ICVProvider
    {
        ImageDimensionResponse ExtractDimensions(FileInfo imageFileInfo);
    }
}
