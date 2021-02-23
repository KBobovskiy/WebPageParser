namespace DataBaseContext.Model
{
    public class OzonProduct : IEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Product name/title
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Product id from Ozon web site
        /// </summary>
        public int OzonProductId { get; set; }

        /// <summary>
        /// Product link
        /// </summary>
        public string ProductLink { get; set; }
    }
}