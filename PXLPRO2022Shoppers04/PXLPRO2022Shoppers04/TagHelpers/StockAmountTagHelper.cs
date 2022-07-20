using Microsoft.AspNetCore.Razor.TagHelpers;
using PXLPRO2022Shoppers04.Helpers;
using System;

namespace PXLPRO2022Shoppers04.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("stock-amount")]
    public class StockAmountTagHelper : TagHelper
    {
        public Guid SSN { get; set; }
        public override async void Process(TagHelperContext context, TagHelperOutput output)
        {
            string stockamount = APIHelper.GetStockOfProduct(SSN).Result.Amount.ToString();
            if (APIHelper.GetStockOfProduct(SSN).Result.Amount <= 0)
            {
                stockamount = "SOLD OUT";
            }
            output.Content.SetHtmlContent($"{stockamount}");
        }
    }
}
