namespace OzonPriceChecker
{
    public class OzonProductDataState
    {
        public class OzonDataState
        {
            public OzonOffer[] offers { get; set; }
            public OzonCellTrackingInfo cellTrackingInfo { get; set; }
        }

        public class OzonCellTrackingInfo
        {
            public OzonProduct product { get; set; }
        }

        public class OzonProduct
        {
            public int id { get; set; }
            public string title { get; set; }
            public string link { get; set; }
            public double finalPrice { get; set; }
            public double price { get; set; }
            public double discount { get; set; }
        }

        public class OzonOffer
        {
            /// <summary>
            /// Offer is selected
            /// </summary>
            public bool isSelected { get; set; }

            /// <summary>
            /// The price with current discount
            /// </summary>
            public string price { get; set; }

            /// <summary>
            /// The base price
            /// </summary>
            public string originalPrice { get; set; }

            public PremiumTextRs[] premiumTextRs { get; set; }

            public string relativeLink { get; set; }

            /// <summary>
            /// Message. Examples: "Товар закончился"
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// Offer name, examples:
            /// "Регулярная доставка"
            /// "Обычная доставка"
            /// </summary>
            public string offerName { get; set; }

            public bool isAvailable { get; set; }
        }

        public class PremiumTextRs
        {
            public string type { get; set; }

            public string content { get; set; }
        }
    }
}