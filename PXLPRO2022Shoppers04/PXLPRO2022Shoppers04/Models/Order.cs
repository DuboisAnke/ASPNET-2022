using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PXLPRO2022Shoppers04.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public string OrderDate { get; set; } = DateTime.Now.ToString();
        public string OrderState { get; set; } = "Processed";
        public Address Address { get; set; }
        public virtual IdentityUser User { get; set; }
        public ICollection<OrderLine> OrderLines { get; set; }
    }
}