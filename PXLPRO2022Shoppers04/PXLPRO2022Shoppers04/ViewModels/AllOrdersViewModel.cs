using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Models;
using System.Collections.Generic;
using System.Linq;

namespace PXLPRO2022Shoppers04.ViewModels
{
    public class AllOrdersViewModel
    {
        AppDbContext _context;
        IdentityUser _identityUser;

        public AllOrdersViewModel(AppDbContext context, IdentityUser user)
        {
            _context = context;
            _identityUser = user;
        }

        public List<Order> Orders => new List<Order>(_context.Orders.Include(x => x.User).Include(x => x.Address)
            .Where(x => x.User == _identityUser));

        public List<OrderLine> OrderLines => new List<OrderLine>(_context.OrderLines.Include(x => x.Order)
            .Include(x => x.Order.User).Include(x => x.Product).Where(x => x.Order.User == _identityUser));
    }
}