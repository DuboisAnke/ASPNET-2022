using PXLPRO2022Shoppers04_StockAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04_StockAPI.Services
{
    public interface IStockLogRepositoryInterface
    {
        IEnumerable<StockLog> GetAll();
        Task Update(StockLog stocklog);
        Task Delete(StockLog stocklog);
        Task Add(StockLog stocklog);
    }
}
