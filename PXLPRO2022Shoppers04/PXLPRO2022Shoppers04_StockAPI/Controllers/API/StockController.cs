using Microsoft.AspNetCore.Mvc;
using PXLPRO2022Shoppers04_StockAPI.Data;
using PXLPRO2022Shoppers04_StockAPI.Models;
using PXLPRO2022Shoppers04_StockAPI.Services;
using System;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04_StockAPI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockRepositoryInterface _stockrepo;
        private readonly IStockLogRepositoryInterface _stocklogrepo;
        DataContext _context;

        public StockController(IStockRepositoryInterface stockrepo, IStockLogRepositoryInterface stocklogrepo, DataContext context)
        {
            _stockrepo = stockrepo;
            _context = context;
            _stocklogrepo = stocklogrepo;

        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var models = _context.Stock;
            return Ok(models);
        }

        [HttpPost]
        public async Task<IActionResult> AddStock(Stock stock)
        {
            Stock newStock = new Stock
            {
                SSN = stock.SSN,
                Amount = stock.Amount
            };
            var result = await _stockrepo.Add(newStock);
            if (result != null)
            {
                await _stocklogrepo.Add(new StockLog
                {
                    SSN = newStock.SSN,
                    Message = $"Product with SSN: {newStock.SSN} couldn't be added because of an SSN that already exists in the database."
                });
                return CreatedAtAction("GetDetails", result);
            }

            await _stocklogrepo.Add(new StockLog
            {
                SSN = newStock.SSN,
                Message = $"Product with SSN: {newStock.SSN} added with stock amount: {newStock.Amount}"
            });

            return CreatedAtAction("GetDetails", newStock);
        }

        [HttpDelete("{ssn}")]
        public async Task<IActionResult> RemoveStock(Guid ssn)
        {
            await _stockrepo.Delete(ssn);
            await _stocklogrepo.Add(new StockLog
            {
                SSN = ssn,
                Message = $"Product with SSN: {ssn} deleted"
            });
            return AcceptedAtAction("GetDetails", $"{ssn} is removed from stock");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAmount(Stock stock)
        {
            Stock receivedStock = _stockrepo.GetBySSN(stock.SSN);
            if (receivedStock == null) return NotFound();
            //if (receivedStock.Amount < stock.Amount)
            //{
            //    //not enough stock -> return error not enough stock
            //}

            //if (receivedStock.Amount == 0)
            //{

            //}


            receivedStock.Amount = stock.Amount;


            await _stockrepo.Update(receivedStock);
            await _stocklogrepo.Add(new StockLog
            {
                SSN = receivedStock.SSN,
                Message = $"Product with SSN: {receivedStock.SSN} is updated with stock amount: {receivedStock.Amount}"
            });
            return Ok();
        }

        [HttpGet("{ssn}")]
        public ActionResult<Stock> GetStock(Guid ssn)
        {
            return _stockrepo.GetBySSN(ssn);
        }
    }
}
