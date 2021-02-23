namespace OzonPriceChecker
{
    public class Product
    {
        public int Id { get; set; }

        /// <summary>
        /// Product name/title
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Selected offer name
        /// </summary>
        public string OfferName { get; set; }

        /// <summary>
        /// Selected offer message
        /// </summary>
        public string OfferMessageText { get; set; }

        /// <summary>
        /// Product base price
        /// </summary>
        public double PriceBase { get; set; }

        /// <summary>
        /// Product price with discount
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Product discount
        /// </summary>
        public double Discount { get; set; }

        /// <summary>
        /// Selected offer premium price
        /// </summary>
        public double PricePremium { get; set; }

        /// <summary>
        /// Is product available to order
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}