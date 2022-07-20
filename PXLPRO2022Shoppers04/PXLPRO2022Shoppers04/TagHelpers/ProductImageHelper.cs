using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PXLPRO2022Shoppers04.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("product-image")]
    public class ProductImageHelper : TagHelper
    {
        public string ProductImage { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.SetHtmlContent(ProductImage);
        }
    }
}