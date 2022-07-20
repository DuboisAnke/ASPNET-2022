using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PXLPRO2022Shoppers04_StockAPI.Models
{
    public class StockLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SLID { get; set; }
        [ForeignKey("SSN")]
        public Guid SSN { get; set; }
        public string Message { get; set; }
    }
}
