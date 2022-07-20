using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Helpers;
using PXLPRO2022Shoppers04.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.Controllers
{
    public class AddressController : Controller
    {
        private readonly AppDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public AddressController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        public IActionResult CreateAddress()
        {
            return View();
        }

        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddress(Address address)
        {
            if (ModelState.IsValid)
            {
                var newAddress = address;
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var client = _context.Clients.FirstOrDefault(x => x.IdentityUserId == user.Id);
                    newAddress.ClientId = client.ClientId;
                    _context.Add(newAddress);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", "Client");
        }

        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddressFromOrdering(Address address)
        {
            if (ModelState.IsValid)
            {
                var newAddress = address;
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var client = _context.Clients.FirstOrDefault(x => x.IdentityUserId == user.Id);
                    newAddress.ClientId = client.ClientId;
                    _context.Add(newAddress);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Details", "Order");
        }
    }
}