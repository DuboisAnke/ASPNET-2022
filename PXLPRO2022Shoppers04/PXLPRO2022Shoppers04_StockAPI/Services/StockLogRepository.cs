using PXLPRO2022Shoppers04_StockAPI.Data;
using PXLPRO2022Shoppers04_StockAPI.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04_StockAPI.Services
{
    public class StockLogRepository : IStockLogRepositoryInterface
    {
        DataContext _context;

        public StockLogRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<StockLog> GetAll()
        {
            Debug.WriteLine("Stocklog GetAll method executed.");
            return _context.StockLog;
        }
        public async Task Add(StockLog stocklog)
        {
            Debug.WriteLine("Stocklog Add method executed");
            _context.StockLog.Add(stocklog);
            await _context.SaveChangesAsync();
            var stocklogs = _context.StockLog;
        }

        public async Task Delete(StockLog stocklog)
        {
            Debug.WriteLine("Stocklog Delete method executed");
            _context.StockLog.Remove(stocklog);
            await _context.SaveChangesAsync();
        }

        public async Task Update(StockLog stocklog)
        {
            Debug.WriteLine("Stocklog Update method executed");
            _context.StockLog.Update(stocklog);
            await _context.SaveChangesAsync();
        }
    }
}
