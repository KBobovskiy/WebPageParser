using DataBaseContext.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataBaseContext.DAO
{
    public interface IPriceHistoryDao
    {
        Task<ReadOnlyCollection<OzonProduct>> GetAllOzonProductsAsync();

        Task<int> SaveProductHistoryAsync(OzonProductPriceHistory productHistoryRecord);

        Task<OzonProductPriceHistory> SelectLastProductPriceAsync(int productId);
    }
}