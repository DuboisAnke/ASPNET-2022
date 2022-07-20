using PXLPRO2022Shoppers04.Models;
using System.Collections.Generic;

namespace PXLPRO2022Shoppers04.ViewModels
{
    public class ClientInfoViewModel
    {
        public Client Client { get; set; }
        public Address Address { get; set; }
        public List<Address> Addresses { get; set; }
    }
}