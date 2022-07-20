using Microsoft.AspNetCore.Mvc;

namespace PXLPRO2022Shoppers04.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        public NavigationViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}