using System;

namespace PXLPRO2022Shoppers04.ViewModels
{
    public class ProductStockViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid SSN { get; set; }
        public int StockAmount { get; set; }
    }
}
