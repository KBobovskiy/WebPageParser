using DataBaseContext.Model;
using NLog;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseContext.DAO
{
    public class PriceHistoryDao : IPriceHistoryDao
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string _connectionString;

        public PriceHistoryDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Select all <see cref="OzonProduct"/> from DB.
        /// </summary>
        /// <returns></returns>
        public async Task<ReadOnlyCollection<OzonProduct>> GetAllOzonProductsAsync()
        {
            using (var client = new SqliteContext(_connectionString))
            {
                logger.Debug($"Load all {nameof(OzonProduct)}");

                var allOzonProducts = await Task.Run(() => { return client.OzonProducts.ToArray(); });

                return new ReadOnlyCollection<OzonProduct>(allOzonProducts);
            }
        }

        /// <summary>
        /// Save <see cref="OzonProductPriceHistory"/> data into DB
        /// </summary>
        /// <param name="productHistoryRecord"></param>
        /// <returns></returns>
        public async Task<int> SaveProductHistoryAsync(OzonProductPriceHistory productHistoryRecord)
        {
            if (productHistoryRecord == null)
            {
                logger.Debug($"{nameof(productHistoryRecord)} is null!");

                return 0;
            }

            using (var client = new SqliteContext(_connectionString))
            {
                if (productHistoryRecord.Id == 0)
                {
                    client.OzonProductPriceHistories.Add(productHistoryRecord);
                    await client.SaveChangesAsync();
                }
                else
                {
                    client.Update(productHistoryRecord);
                    await client.SaveChangesAsync();
                }

                return productHistoryRecord.Id;
            }
        }

        /// <summary>
        /// Select last <see cref="OzonProductPriceHistory"/> entry by product Id
        /// </summary>
        /// <param name="productId"><see cref="OzonProductPriceHistory.ProductId"/></param>
        /// <returns></returns>
        public async Task<OzonProductPriceHistory> SelectLastProductPriceAsync(int productId)
        {
            if (productId <= 0)
            {
                logger.Debug($"{nameof(productId)} <= 0!");

                return null;
            }

            using (var client = new SqliteContext(_connectionString))
            {
                var productPrice =
                    await Task.Run(() =>
                    {
                        return client.OzonProductPriceHistories
                            .Where(x => x.ProductId == productId)
                            .OrderByDescending(x => x.Date)
                            .FirstOrDefault();
                    });

                return productPrice;
            }
        }
    }
}