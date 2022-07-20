using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Helpers;
using PXLPRO2022Shoppers04.Models;
using PXLPRO2022Shoppers04.Models.Categories;
using PXLPRO2022Shoppers04.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public ProductController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.ProductId == id);
            if (product != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    if (!_context.ShoppingCarts.Any(x => x.ClientId == user.Id))
                    {
                        var shoppingCart = new ShoppingCart(_context)
                        {
                            ClientId = user.Id
                        };
                        _context.ShoppingCarts.Add(shoppingCart);
                        await _context.SaveChangesAsync();

                        var foundProd = _context.Products.FirstOrDefault(x => x.ProductId == id);

                        var enoughStock = await APIHelper.GetStockOfProduct(foundProd.SSN);
                        if (enoughStock.Amount >= 1)
                        {
                            var cartItem = new CartItem
                            {
                                ProductId = id,
                                ShoppingCartId = shoppingCart.ShoppingCartId,
                                Product = foundProd,
                                Quantity = 1,
                            };
                            _context.ShoppingCartItems.Add(cartItem);
                        }
                        else
                        {
                            //TODO: Show error not enough stock?
                            ModelState.AddModelError("Stock", "Stock is out");
                            return RedirectToAction("Index", new { errorMsg = $"{foundProd.Name} is out of stock." });
                        }



                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var shoppingCart = _context.ShoppingCarts.FirstOrDefault(x => x.ClientId == user.Id);
                        if (shoppingCart != null)
                        {
                            if (_context.ShoppingCartItems.Any(x => x.ShoppingCartId == shoppingCart.ShoppingCartId
                                                                    && x.ProductId == product.ProductId))
                            {
                                var product2 = _context.ShoppingCartItems
                                    .FirstOrDefault(x => x.ShoppingCartId == shoppingCart.ShoppingCartId
                                                         && x.ProductId == product.ProductId);
                                if (product2 != null)
                                {
                                    var enoughStock = await APIHelper.GetStockOfProduct(product2.Product.SSN);
                                    if (enoughStock.Amount - 1 >= product2.Quantity)
                                    {
                                        product2.Quantity++;
                                    }
                                    else
                                    {
                                        //TODO: Show error not enough stock?

                                        return RedirectToAction("Index", new { errorMsg = $"{product2.Product.Name} is out of stock." });
                                    }
                                }

                                _context.ShoppingCartItems.Update(product2);
                                await _context.SaveChangesAsync();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                var foundProd = _context.Products.FirstOrDefault(x => x.ProductId == id);

                                var enoughStock = await APIHelper.GetStockOfProduct(foundProd!.SSN);
                                if (enoughStock.Amount >= 1)
                                {
                                    var cartItem = new CartItem
                                    {
                                        ProductId = id,
                                        ShoppingCartId = shoppingCart.ShoppingCartId,
                                        Product = foundProd,
                                        Quantity = 1,
                                    };
                                    _context.ShoppingCartItems.Add(cartItem);
                                }
                                else
                                {
                                    //TODO: Show error not enough stock?
                                    return RedirectToAction("Index", new { errorMsg = $"{foundProd.Name} is out of stock." });
                                }

                                await _context.SaveChangesAsync();
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account",
                        new { errorMsg = "Please login in order to add items to shopping cart." });
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var shoppingCart = _context.ShoppingCarts.FirstOrDefault(x => x.ClientId == user.Id);
                if (shoppingCart != null)
                {
                    var shoppingCartItem = _context.ShoppingCartItems
                        .FirstOrDefault(x => x.ShoppingCartId == shoppingCart.ShoppingCartId && x.ProductId == id);
                    if (shoppingCartItem != null)
                    {
                        _context.ShoppingCartItems.Remove(shoppingCartItem);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "ShoppingCart");
                    }
                }
            }

            return RedirectToAction("Index", "ShoppingCart",
                new { errorMsg = "Item cannot be removed because it does not exist in cart." });
        }

        public async Task<IActionResult> Index(string errorMsg)
        {
            return View(new ProductsPageViewModel(_context, errorMsg));
        }

        public IActionResult DetailsKeyboard(Keyboard keyboard)
        {
            return View(keyboard);
        }

        public IActionResult DetailsMouse(Mouse mouse)
        {
            return View(mouse);
        }

        public IActionResult DetailsMousePad(MousePad mousepad)
        {
            return View(mousepad);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(x => x.ProductCategory)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            if (product.GetType().Name.Equals(nameof(Keyboard)))
            {
                return RedirectToAction("DetailsKeyboard", product);
            }

            if (product.GetType().Name.Equals(nameof(Mouse)))
            {
                return RedirectToAction("DetailsMouse", product);
            }

            if (product.GetType().Name.Equals(nameof(MousePad)))
            {
                return RedirectToAction("DetailsMousePad", product);
            }

            return NotFound();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}