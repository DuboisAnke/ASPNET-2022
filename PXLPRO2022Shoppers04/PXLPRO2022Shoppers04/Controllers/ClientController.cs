using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Helpers;
using PXLPRO2022Shoppers04.Models;
using PXLPRO2022Shoppers04.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.Controllers
{
    public class ClientController : Controller
    {
        private readonly AppDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public ClientController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Client
        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        public async Task<IActionResult> Index(ClientInfoViewModel clientInfoViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

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
                clientInfoViewModel.Client = newClient;

                var clientAddresses = _context.Addresses.Where(x => x.ClientId == newClient.ClientId);

                if (clientAddresses.Any())
                {
                    List<Address> addresses = new List<Address>();
                    foreach (var address in clientAddresses)
                    {
                        addresses.Add(address);
                    }

                    clientInfoViewModel.Addresses = addresses;
                }
            }

            return View(clientInfoViewModel);
        }


        // POST: Client/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ClientInfoViewModel clientInfoViewModel)
        {
            Client client = await _context.Clients.FindAsync(id);
            client.Name = clientInfoViewModel.Client.Name;
            client.FirstName = clientInfoViewModel.Client.FirstName;
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            IdentityUser identityUser = await _context.Users.FindAsync(client.IdentityUserId);
            if (identityUser != null)
            {
                identityUser.Email = clientInfoViewModel.Client.User.Email;
                identityUser.UserName = clientInfoViewModel.Client.User.UserName;

                await _userManager.UpdateAsync(identityUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("");
        }

        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(int id, ClientInfoViewModel clientInfoViewModel)
        {
            if (ModelState.IsValid)
            {
                var address = await _context.Addresses.FindAsync(id);
                address.StreetName = clientInfoViewModel.Address.StreetName;
                address.HouseNumber = clientInfoViewModel.Address.HouseNumber;
                address.ZIP = clientInfoViewModel.Address.ZIP;
                address.City = clientInfoViewModel.Address.City;
                address.Country = clientInfoViewModel.Address.Country;

                _context.Addresses.Update(address);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("");
        }

        [Authorize(Roles = RoleHelper.AdminRole + ", " + RoleHelper.ClientRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddress(ClientInfoViewModel clientInfoViewModel)
        {
            if (ModelState.IsValid)
            {
                var address = clientInfoViewModel.Address;
                address.ClientId = clientInfoViewModel.Client.ClientId;

                _context.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index");
        }

        

        private bool ClientExists(string id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }
    }
}