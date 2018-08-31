using System.IO;
using Ditat.Logic.Models;
using System;
using System.Collections.Generic;

namespace Ditat.Logic.Interfaces
{
    public interface IShopProvider
    {
        string PublishItem(Guid Id, string title, string description, int categoryId, int startPrice, int condition, int durationDays, List<string> imageList, bool Shipping, int? ShippingCost, int productNumber);
    }
}
