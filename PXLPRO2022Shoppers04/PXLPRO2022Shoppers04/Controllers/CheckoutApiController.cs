using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Collections.Generic;

namespace PXLPRO2022Shoppers04.Controllers
{
    [Route("create-checkout-session")]
    public class CheckoutApiController : Controller
    {
        [HttpPost]
        public ActionResult Create(float price = 50)
        {
            var domain = "http://localhost:4242";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                        Price = $"{price}",
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + "/success.html",
                CancelUrl = domain + "/cancel.html",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}