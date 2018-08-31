using Ditat.Logic.Models;

namespace Ditat.Logic
{
    public interface IBookAttributeProvider
    {
        BookInfo GetBookInfo(string code);
    }
}
