using System;
using System.ComponentModel.DataAnnotations;

namespace PXLPRO2022Shoppers04_StockAPI.Models
{
    public class Stock
    {
        [Key]
        public Guid SSN { get; set; }
        public int Amount { get; set; }
    }
}
