using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Models;
using PXLPRO2022Shoppers04.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly AppDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public ShoppingCartController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string errorMsg)
        {
            var user = await _userManager.GetUserAsync(User);
            List<CartItem> cartItems = new List<CartItem>();
            if (user != null)
            {
                var shoppingcart = _context.ShoppingCarts.FirstOrDefault(x => x.ClientId == user.Id);
                if (shoppingcart == null)
                {
                    var newShoppingCart = new ShoppingCart(_context)
                    {
                        ClientId = user.Id,
                    };

                    _context.ShoppingCarts.Add(newShoppingCart);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var items = _context.ShoppingCartItems.Where(x => x.ShoppingCartId == shoppingcart.ShoppingCartId);
                    foreach (var cartItem in items)
                    {
                        var prod = _context.Products.FirstOrDefault(x => x.ProductId == cartItem.ProductId);
                        cartItem.Product = prod;
                        cartItems.Add(cartItem);
                    }
                }

                return View(new ShoppingcartInfoViewModel
                {
                    CartItems = cartItems,
                    ErrorMessage = errorMsg
                });
            }


            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> QuantityChange(int id)
        {
            var quantity = int.Parse(Request.Form["quantity"][0]);
            var user = await _userManager.GetUserAsync(User);
            var shoppingcart = _context.ShoppingCarts.FirstOrDefault(x => x.ClientId == user.Id);

            List<CartItem> items =
                new List<CartItem>(
                    _context.ShoppingCartItems.Where(x => x.ShoppingCartId == shoppingcart.ShoppingCartId));
            foreach (var cartItem in items)
            {
                if (cartItem.ProductId == id)
                {
                    cartItem.Quantity = quantity;
                    _context.ShoppingCartItems.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }
    }
}