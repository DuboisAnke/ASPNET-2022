using Microsoft.EntityFrameworkCore;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Models;
using System.Collections.Generic;
using System.Linq;

namespace PXLPRO2022Shoppers04.ViewModels
{
    public class ProductsPageViewModel
    {
        private AppDbContext _context;
        public string ErrorMessage { get; set; }

        public ProductsPageViewModel(AppDbContext context, string errorMsg)
        {
            _context = context;
            ErrorMessage = errorMsg;
        }

        public List<Product> GetAmountOfProductsOfCategory(string type, short amount)
        {
            return _context.Products.Include(x => x.ProductCategory)
                .Where(x => x.ProductCategory.Name == type).Take(amount).ToList();
        }
    }
}