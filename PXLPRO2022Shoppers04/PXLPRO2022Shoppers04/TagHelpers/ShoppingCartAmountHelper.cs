using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PXLPRO2022Shoppers04.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("cart-items-count")]
    public class ShoppingCartAmountHelper : TagHelper
    {
        private AppDbContext _context;
        private UserManager<IdentityUser> _userManager;
        public ClaimsPrincipal User { get; set; }

        public ShoppingCartAmountHelper(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var user = GetUser().Result;
            if (user != null)
            {
                var shoppingCart = _context.ShoppingCarts.FirstOrDefault(x => x.ClientId == user.Id);
                if (shoppingCart != null)
                {
                    var count = _context.ShoppingCartItems.Count(x => x.ShoppingCartId == shoppingCart.ShoppingCartId);
                    if (count > 0) output.Content.SetHtmlContent($"<sup style='color:red;'>{count}</sup>");
                }
            }
        }

        private async Task<IdentityUser> GetUser() => await _userManager.GetUserAsync(User);
    }
}