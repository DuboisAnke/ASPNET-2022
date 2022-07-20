using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Helpers;
using PXLPRO2022Shoppers04.Models;
using PXLPRO2022Shoppers04.ViewModels;
using Scriban;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronPdf;

namespace PXLPRO2022Shoppers04.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public OrderController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Order
        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(new AllOrdersViewModel(_context, user));
        }

        // GET: Order/Details/5
        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        public async Task<IActionResult> Details()
        {
            ClientInfoOrderViewModel clientInfoOrderViewModel = new ClientInfoOrderViewModel();
            var user = await _userManager.GetUserAsync(User);
            var selectedClient = _context.Clients.FirstOrDefault(x => x.IdentityUserId == user.Id);
            if (selectedClient != null)
            {
                var newClient = new Client
                {
                    ClientId = selectedClient.ClientId,
                    Name = selectedClient.Name,
                    FirstName = selectedClient.FirstName,
                    IdentityUserId = user.Id,
                    User = user
                };
                clientInfoOrderViewModel.Client = newClient;

                var clientAddresses = _context.Addresses.Where(x => x.ClientId == newClient.ClientId);
                clientInfoOrderViewModel.Client.Addresses = new List<Address>(clientAddresses);
                ViewData["AddressList"] =
                    new SelectList(clientAddresses, "AddressId", "FullAddress");
            }

            ShoppingCart shoppingCart = _context.ShoppingCarts.FirstOrDefault(x => x.ClientId == user.Id);
            List<CartItem> cartItems = new List<CartItem>();
            if (shoppingCart != null)
            {
                cartItems = new List<CartItem>(_context.ShoppingCartItems.Include(x => x.Product)
                    .Where(x => x.ShoppingCartId == shoppingCart.ShoppingCartId));
            }

            clientInfoOrderViewModel.CartItems = cartItems;
            return View(clientInfoOrderViewModel);
        }


        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientInfoOrderViewModel clientInfoOrderViewModel)
        {
            //TO DO: Save the passed address into Order.Address!
            var user = await _userManager.GetUserAsync(User);
            ShoppingCart shoppingCart = _context.ShoppingCarts.FirstOrDefault(x => x.ClientId == user.Id);
            List<CartItem> cartItems = new List<CartItem>();
            if (shoppingCart != null)
            {
                cartItems = new List<CartItem>(_context.ShoppingCartItems.Include(x => x.Product)
                    .Where(x => x.ShoppingCartId == shoppingCart.ShoppingCartId));
            }

            if (user != null && cartItems.Count > 0)
            {
                var address = _context.Addresses.FirstOrDefault(x => x.AddressId == clientInfoOrderViewModel.AddressId);
                var order = new Order
                {
                    User = user,
                    Address = address
                };
                _context.Add(order);
                await _context.SaveChangesAsync();


                List<CartItem> cartItemsWithoutEnoughStock = new List<CartItem>();
                foreach (var item in cartItems)
                {
                    var orderline = new OrderLine
                    {
                        Order = order,
                        OrderId = order.OrderId,
                        Product = item.Product,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };

                    _context.OrderLines.Add(orderline);
                    await _context.SaveChangesAsync();

                    //TODO: Check first if quantity of product is lower or equals to ordering quantity
                    var enoughStock = await APIHelper.DecreaseStock(item.Product.SSN, item.Quantity);
                    //if (!enoughStock)
                    //{
                    //    cartItemsWithoutEnoughStock.Add(item);

                    //}
                }

                //if (cartItemsWithoutEnoughStock.Count > 0)
                //{
                //    ModelState.AddModelError("","Not enough in stock");
                //    return RedirectToAction("Details");
                //}


                var shoppingCartToDelete = _context.ShoppingCarts.FirstOrDefault(x =>
                    x.ShoppingCartId == shoppingCart.ShoppingCartId);
                if (shoppingCartToDelete != null) _context.ShoppingCarts.Remove(shoppingCartToDelete);
                var shoppingCartItemsToDelete =
                    _context.ShoppingCartItems.Where(x => x.ShoppingCartId == shoppingCartToDelete.ShoppingCartId);
                if (shoppingCartItemsToDelete.Any())
                {
                    foreach (var itemToDelete in shoppingCartItemsToDelete)
                    {
                        _context.ShoppingCartItems.Remove(itemToDelete);
                    }

                    await _context.SaveChangesAsync();
                }


                return RedirectToAction("ThankYouForOrderingPage", new { orderid = order.OrderId });
            }
            else
            {
                return RedirectToAction("Login", "Account", new
                { errorMsg = "Order failed. Please login first." });
            }
        }

        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        [HttpGet]
        public async Task<IActionResult> ThankYouForOrderingPage(int orderid)
        {
            ClientInfoOrderViewModel clientInfoOrderViewModel = new ClientInfoOrderViewModel();
            var order = _context.Orders.FirstOrDefault(x => x.OrderId == orderid);
            clientInfoOrderViewModel.Order = order;

            ICollection<OrderLine> orderLines = new List<OrderLine>();
            orderLines = _context.OrderLines.Include(y => y.Product).Where(x => x.OrderId == orderid).ToList();
            clientInfoOrderViewModel.Order.OrderLines = orderLines;

            var user = await _userManager.GetUserAsync(User);
            var selectedClient = _context.Clients.Include(y => y.Addresses)
                .FirstOrDefault(x => x.IdentityUserId == user.Id);
            clientInfoOrderViewModel.Client = selectedClient;


            var addres = selectedClient.Addresses.FirstOrDefault(x => x.AddressId == order.Address.AddressId);
            clientInfoOrderViewModel.Address = addres;


            return View(clientInfoOrderViewModel);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        public async Task<IActionResult> DownloadOrderPDF(int orderId)
        {
            var orderlines = _context.OrderLines.Where(x => x.OrderId == orderId).Include(x => x.Product);
            var orderObj = _context.Orders.Include(x => x.Address).FirstOrDefault(x => x.OrderId == orderId);


            double totalPrice = Math.Round(orderlines.Sum(x => x.Product.Price * x.Quantity), 2);

            var template = Template.Parse($"<img style='width:100px;margin-left:-30px' src='https://media.discordapp.net/attachments/892722636419899413/980561116830765086/unknown.png'><div style='display:flex; justify-content:space-between;'><p>Ordernr: {orderId}</p> <p>Order Date: {orderObj.OrderDate}</p></div>"+$"<p>Delivery Address: {orderObj.Address.FullAddress}</p>" + @"
<table >
  <tr>
    <th style='text-align:left;width:100vw'>Product</th>
    <th style='text-align:left;width:100vw'>Quantity</th>
    <th style='text-align:left;width:100vw'>Price</th>
  </tr>
 {{ for orderline in orderlines }}
  <tr>
    <td >{{orderline.product.name}}</td>
    <td >{{orderline.quantity}}</td>
    <td >€ {{orderline.product.price * orderline.quantity}}</td>
  </tr>
 {{ end }}
</table>
<hr>
" + $"<p style='text-align:right;'>Total price: € {totalPrice}</p>");
            var result = template.Render(new { Orderlines = orderlines });


            // Instantiate Renderer
            var Renderer = new ChromePdfRenderer();
            // Create a PDF from a HTML string using C#
            using var pdf = Renderer.RenderHtmlAsPdf(result);
            string downloadFolderPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\" + "Downloads";
            // Export to a file or Stream
            //pdf.SaveAs(downloadFolderPath + $@"\order{orderId}--{orderObj.OrderDate.Split(' ')[0].Replace('/','-')}.pdf");

            return File(pdf.BinaryData, "application/pdf", $@"\order{orderId}--{orderObj.OrderDate.Split(' ')[0].Replace('/', '-')}.pdf");
            return RedirectToAction(nameof(Index));
        }
    }
}