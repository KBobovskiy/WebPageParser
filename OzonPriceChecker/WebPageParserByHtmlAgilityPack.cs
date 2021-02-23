using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using static OzonPriceChecker.OzonProductDataState;

namespace OzonPriceChecker
{
    public class WebPageParserByHtmlAgilityPack : IWebPageParser
    {
        public Product GetOzonProductInfoByLink(string link)
        {
            if (TryGetDivWithPricesFromWebPage(link, out var divWithPrices))
            {
                if (TryGetPricesFromDivDataStateAttribute(divWithPrices, out Product productPrices))
                {
                    return productPrices;
                }
            }

            return null;
        }

        private static bool TryGetDivWithPricesFromWebPage(string url, out HtmlNode divWithPrices)
        {
            divWithPrices = null;
            HtmlDocument htmlDoc = null;
            const string SearchingString = "Обычная доставка";

            try
            {
                HtmlWeb web = new HtmlWeb();
                htmlDoc = web.Load(url);
            }
            catch (Exception)
            {
                return false;
            }

            if (htmlDoc == null)
            {
                return false;
            }

            var divs = htmlDoc?.DocumentNode.SelectNodes("//body/div");
            if (divs == null)
            {
                return false;
            }

            foreach (var div in divs)
            {
                if (div.OuterHtml.Contains(SearchingString, StringComparison.InvariantCultureIgnoreCase))
                {
                    divWithPrices = div;
                }
            }

            while (divWithPrices.OuterHtml.Contains(SearchingString, StringComparison.InvariantCultureIgnoreCase))
            {
                var childNodes = divWithPrices.ChildNodes;
                var updated = false;
                for (int i = 0; i < childNodes.Count; i++)
                {
                    var child = childNodes[i];
                    if (child.OuterHtml.Contains(SearchingString, StringComparison.InvariantCultureIgnoreCase))
                    {
                        divWithPrices = child;
                        updated = true;
                        break;
                    }
                }

                if (!updated)
                {
                    break;
                }
            }

            return divWithPrices != null;
        }

        private static bool TryGetPricesFromDivDataStateAttribute(HtmlNode divWithPrices, out Product productPrices)
        {
            productPrices = new Product();

            if (divWithPrices == null)
            {
                return false;
            }

            if (!divWithPrices.Attributes.Contains("data-state"))
            {
                return false;
            }

            var dataState = divWithPrices.Attributes["data-state"];
            if (dataState == null)
            {
                return false;
            }

            var ozonDataState = JsonSerializer.Deserialize<OzonDataState>(dataState.Value);
            if (ozonDataState != null)
            {
                if (ozonDataState.cellTrackingInfo?.product != null)
                {
                    var ozonProduct = ozonDataState.cellTrackingInfo.product;
                    productPrices.Id = ozonProduct.id;
                    productPrices.PriceBase = ozonProduct.price;
                    productPrices.Price = ozonProduct.finalPrice;
                    productPrices.PricePremium = ozonProduct.finalPrice;
                    productPrices.Name = ozonProduct.title;
                    productPrices.Discount = ozonProduct.discount;

                    var currentOffer = ozonDataState.offers.FirstOrDefault(x => x.isSelected);
                    if (currentOffer != null)
                    {
                        productPrices.PricePremium = GetPremiumPriceFromOffer(currentOffer);
                        productPrices.OfferName = currentOffer.offerName;
                        productPrices.OfferMessageText = currentOffer.message;
                        productPrices.IsAvailable = currentOffer.isAvailable;
                    }
                }
                else
                {
                    var currentOffer = ozonDataState.offers.FirstOrDefault(x => x.isSelected);
                    if (currentOffer != null)
                    {
                        productPrices.PriceBase = GetPriceFromString(currentOffer.originalPrice);
                        productPrices.Price = GetPriceFromString(currentOffer.price);
                        productPrices.PricePremium = GetPremiumPriceFromOffer(currentOffer);
                        productPrices.OfferName = currentOffer.offerName;
                        productPrices.OfferMessageText = currentOffer.message;
                        productPrices.IsAvailable = currentOffer.isAvailable;
                    }
                }
            }

            return true;
        }

        private static double GetPremiumPriceFromOffer(OzonOffer currentOffer)
        {
            if (currentOffer == null || currentOffer.premiumTextRs == null)
            {
                return 0;
            }

            var premiumPriceString = currentOffer.premiumTextRs.FirstOrDefault(x => x.type == "text")?.content ?? string.Empty;
            if (string.IsNullOrEmpty(premiumPriceString))
            {
                return 0;
            }

            return GetPriceFromString(premiumPriceString);
        }

        private static double GetPriceFromString(string stringWithPrice)
        {
            // Cents are not needed
            var regex = new Regex(@"[\d ]+ ");
            var priceAsString = regex.Matches(stringWithPrice).FirstOrDefault().Value;
            priceAsString = priceAsString.Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(priceAsString))
            {
                return 0;
            }

            if (double.TryParse(priceAsString, out var resultPrice))
            {
                return resultPrice;
            }

            return 0;
        }
    }
}