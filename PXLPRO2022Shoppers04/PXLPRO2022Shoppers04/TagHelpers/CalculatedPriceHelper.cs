using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace PXLPRO2022Shoppers04.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("calc-price")]
    public class CalculatedPriceHelper : TagHelper
    {
        public float Price { get; set; }
        public int Quantity { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var total = Price * Quantity;
            output.Content.SetHtmlContent(Math.Round(total, 2).ToString());
        }
    }
}