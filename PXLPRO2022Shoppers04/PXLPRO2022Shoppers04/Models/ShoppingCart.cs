using PXLPRO2022Shoppers04.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PXLPRO2022Shoppers04.Models
{
    public class ShoppingCart
    {
        private AppDbContext _context;
        public ShoppingCart()
        {
        }
        public ShoppingCart(AppDbContext context)
        {
            _context = context;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShoppingCartId { get; set; }
        [ForeignKey("ClientId")]
        public string ClientId { get; set; }
        public List<CartItem> CartItems = new List<CartItem>();
    }
}