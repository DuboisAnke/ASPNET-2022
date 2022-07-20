using Microsoft.AspNetCore.Razor.TagHelpers;
using PXLPRO2022Shoppers04.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PXLPRO2022Shoppers04.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("calc-total-price-for-cart")]
    public class CalcTotalPriceForCartHelper : TagHelper
    {
        public List<CartItem> Items { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            double totalPrice = Math.Round(Items.Sum(x => x.Product.Price * x.Quantity), 2);
            output.Content.SetHtmlContent(totalPrice.ToString());
        }
    }
}