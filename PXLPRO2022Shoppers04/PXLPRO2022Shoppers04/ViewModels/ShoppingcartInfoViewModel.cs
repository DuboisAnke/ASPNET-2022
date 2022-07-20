using PXLPRO2022Shoppers04.Models;
using System.Collections.Generic;

namespace PXLPRO2022Shoppers04.ViewModels
{
    public class ShoppingcartInfoViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public string ErrorMessage { get; set; }
    }
}