using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PXLPRO2022Shoppers04.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("amount-of-orders")]
    public class AmountOfOrdersHelper : TagHelper
    {
        private AppDbContext _context;
        private UserManager<IdentityUser> _userManager;
        public ClaimsPrincipal User { get; set; }

        public AmountOfOrdersHelper(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var user = GetUser().Result;
            if (user != null)
            {
                var orders = _context.Orders.Where(x => x.User == user);
                if (orders.Any())
                {
                    output.Content.SetHtmlContent($"<sup style='color:red;'>{orders.Count()}</sup>");
                }
            }
        }

        private async Task<IdentityUser> GetUser() => await _userManager.GetUserAsync(User);
    }
}