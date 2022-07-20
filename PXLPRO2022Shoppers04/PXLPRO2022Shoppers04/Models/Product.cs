using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PXLPRO2022Shoppers04.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "A name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "A price is required")]
        [DataType(DataType.Currency)] public float Price { get; set; }
        [Required(ErrorMessage = "A brand is required")]
        public string Brand { get; set; }
        [Required(ErrorMessage = "A color is required")]
        public string Color { get; set; }
        public bool HasRGB { get; set; }
        [Required(ErrorMessage = "A description is required")]
        public string Description { get; set; }
        public string ImageLink { get; set; }
        [ForeignKey("SSN")]
        public Guid SSN { get; set; }
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public Keyboard Keyboard { get; set; }
    }
}