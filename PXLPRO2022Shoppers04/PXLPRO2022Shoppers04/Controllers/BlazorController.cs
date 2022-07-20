using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PXLPRO2022Shoppers04.Helpers;

namespace PXLPRO2022Shoppers04.Controllers
{
    public class BlazorController : Controller
    {

        [Authorize(Roles = RoleHelper.AdminRole)]
        public IActionResult Index()
        {

            return View("_BlazorServer_Host");
        }

    }
}
