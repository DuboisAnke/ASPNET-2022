using PXLPRO2022Shoppers04.Models;
using System.Collections.Generic;

namespace PXLPRO2022Shoppers04.ViewModels
{
    public class ClientInfoOrderViewModel
    {
        public Order Order { get; set; }
        public List<CartItem> CartItems { get; set; }
        public Client Client { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public List<Address> Addresses { get; set; }
    }
}