using Microsoft.AspNetCore.Mvc;
using PXLPRO2022Shoppers04_StockAPI.Services;

namespace PXLPRO2022Shoppers04_StockAPI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockLogController : ControllerBase
    {
        private IStockLogRepositoryInterface _stockLogRepository;

        public StockLogController(IStockLogRepositoryInterface stockLogRepository)
        {
            _stockLogRepository = stockLogRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_stockLogRepository.GetAll());
        }
    }
}
