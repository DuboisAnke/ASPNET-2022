using PXLPRO2022Shoppers04_StockAPI.Data;
using PXLPRO2022Shoppers04_StockAPI.Models;
using PXLPRO2022Shoppers04_StockAPI.ViewModel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04_StockAPI.Services
{
    public class StockRepository : IStockRepositoryInterface
    {
        DataContext _context;
        public StockRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ErrorMessageViewModel> Add(Stock stock)
        {
            Debug.WriteLine("Stock Add method executed.");
            var result = _context.Stock.Find(stock.SSN);
            if (result == null)
            {
                _context.Stock.Add(stock);
                await _context.SaveChangesAsync();
            }
            else
            {
                return new ErrorMessageViewModel
                {
                    ErrorCode = 409,
                    ErrorMessage = "This SSN already exists."
                };
            }

            return null;
        }

        public async Task Delete(Guid ssn)
        {
            Debug.WriteLine("Stock Delete method executed.");
            _context.Stock.Remove(_context.Stock.Find(ssn));
            await _context.SaveChangesAsync();
        }

        public Stock GetBySSN(Guid ssn)
        {
            Debug.WriteLine("Stock GetBySSN method executed.");
            return _context.Stock.FirstOrDefault(x => x.SSN == ssn);
        }

        public async Task Update(Stock stock)
        {
            Debug.WriteLine("Stock Update method executed.");
            _context.Stock.Update(stock);
            await _context.SaveChangesAsync();
        }
    }
}
