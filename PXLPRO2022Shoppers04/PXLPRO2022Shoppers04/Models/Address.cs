using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PXLPRO2022Shoppers04.Models
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }
        [ForeignKey("ClientId")] public string ClientId { get; set; }
        public string StreetName { get; set; }
        public string HouseNumber { get; set; }
        public string ZIP { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string FullAddress => $"{StreetName} {HouseNumber}, {ZIP} {City}, {Country}";
        public Client Client { get; set; }
    }
}