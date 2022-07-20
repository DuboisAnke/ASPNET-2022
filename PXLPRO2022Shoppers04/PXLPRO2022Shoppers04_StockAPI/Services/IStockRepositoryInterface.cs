using PXLPRO2022Shoppers04_StockAPI.Models;
using PXLPRO2022Shoppers04_StockAPI.ViewModel;
using System;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04_StockAPI.Services
{
    public interface IStockRepositoryInterface
    {
        Stock GetBySSN(Guid ssn);
        Task Update(Stock stock);
        Task Delete(Guid ssn);
        Task<ErrorMessageViewModel> Add(Stock stock);
    }
}
