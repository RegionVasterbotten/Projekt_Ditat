using Ditat.Logic.Interfaces;
using Ditat.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Ditat.Logic.Providers
{
    class ShopProvider : IShopProvider
    {
        public string PublishItem(Guid Id, string title, string description, int categoryId, int startPrice, int condition, int durationDays, List<string> imageList, bool Shipping, int? ShippingCost, int productNumber)
        {
           
            string appId = Properties.Settings.Default.ShopAppId;
            var iAppId = Convert.ToInt32(appId);

            string shopUser = Properties.Settings.Default.ShopUserId;
            int UserId = Convert.ToInt32(shopUser);

            string token = Properties.Settings.Default.ShopApplicationToken;

            string appServiceKey = Properties.Settings.Default.ShopApplicationKey;

            Tradera.RestrictedService.RestrictedService RestrictedService = new Tradera.RestrictedService.RestrictedService();
            Tradera.RestrictedService.ItemRequest ItemRequest = new Tradera.RestrictedService.ItemRequest();

            Tradera.RestrictedService.AuthenticationHeader AuthenticationHeader = new Tradera.RestrictedService.AuthenticationHeader();
            Tradera.RestrictedService.AuthorizationHeader AuthorizationHeader = new Tradera.RestrictedService.AuthorizationHeader();

            AuthenticationHeader.AppId = iAppId;
            AuthenticationHeader.AppKey = appServiceKey;

            AuthorizationHeader.UserId = UserId;
            AuthorizationHeader.Token = token;

            RestrictedService.UseDefaultCredentials = true;

            RestrictedService.AuthenticationHeaderValue = AuthenticationHeader;
            RestrictedService.AuthorizationHeaderValue = AuthorizationHeader;

            ItemRequest.Title = title;
            ItemRequest.Description = description;
            ItemRequest.CategoryId = categoryId;//344480 = Testauktioner
            ItemRequest.ItemType = 1;
            ItemRequest.AcceptedBidderId = 1;
            ItemRequest.Duration = durationDays;
            ItemRequest.StartPrice = startPrice;

            string[] ownref = new string[1];
            ownref[0] = productNumber.ToString();

            ItemRequest.OwnReferences = ownref;

            int[] paymentOptions = new int[1];
            //4096=Swish, 32=Kontant, 64=PayPal
            paymentOptions[0] = 64;
            
            ItemRequest.PaymentOptionIds = paymentOptions;

            //Todo: Condition

            int iShippingCost = 0;
            if (ShippingCost.HasValue) { iShippingCost = Convert.ToInt16(ShippingCost); }
            
            if (!Shipping && iShippingCost != 0)
            {
                //2 shipping options
                Tradera.RestrictedService.ItemShipping ItemShipping1 = new Tradera.RestrictedService.ItemShipping();
                Tradera.RestrictedService.ItemShipping ItemShipping2 = new Tradera.RestrictedService.ItemShipping();

                ItemShipping1.ShippingOptionId = 8;
                ItemShipping1.Cost = 0;

                ItemShipping2.ShippingOptionId = 1;
                ItemShipping2.Cost = iShippingCost;

                var ShippingOptions = new Tradera.RestrictedService.ItemShipping[2];
                ShippingOptions[0] = ItemShipping1;
                ShippingOptions[1] = ItemShipping2;

                ItemRequest.ShippingOptions = ShippingOptions;
            }
            else
            {
                // One shipping option
                Tradera.RestrictedService.ItemShipping ItemShipping = new Tradera.RestrictedService.ItemShipping();

                var ShippingOptions = new Tradera.RestrictedService.ItemShipping[1];

                ItemShipping.Cost = iShippingCost;
                if (!Shipping)
                {
                    ItemShipping.ShippingOptionId = 8;
                    ItemShipping.Cost = 0;
                }
                else
                {
                    ItemShipping.ShippingOptionId = 1;
                    ItemShipping.Cost = iShippingCost;
                }
                
                ShippingOptions[0] = ItemShipping;

                ItemRequest.ShippingOptions = ShippingOptions;
            }

            
            Tradera.RestrictedService.QueuedRequestResponse QueuedRequestResponse = new Tradera.RestrictedService.QueuedRequestResponse();
            
            QueuedRequestResponse = RestrictedService.AddItem(ItemRequest);
            var itemId = QueuedRequestResponse.ItemId;

            //Upload Images
            foreach (var imageFileName in imageList)
            {                
                byte[] ImageData = null;

                string imagePath = Properties.Settings.Default.ImagePath;

                string FileName = imagePath + Path.GetFileName(imageFileName); 

                ImageData = File.ReadAllBytes(FileName);

                RestrictedService.AddItemImage(QueuedRequestResponse.RequestId, ImageData, Tradera.RestrictedService.ImageFormat.Jpeg, true);
            }
            
                        
            RestrictedService.AddItemCommit(QueuedRequestResponse.RequestId);

            string itemLink = "";

            using (var publicService = new Tradera.PublicService.PublicService())
            {
                publicService.Url += string.Format("?appId={0}&appKey={1}", appId, appServiceKey);

                System.Threading.Thread.Sleep(5000);

                var item = publicService.GetItem(itemId);
                itemLink = item.ItemLink;
            }
            
            return itemLink;
        }
    }
}
