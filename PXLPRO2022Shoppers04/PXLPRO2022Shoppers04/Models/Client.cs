using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PXLPRO2022Shoppers04.Models
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string IdentityUserId { get; set; }
        public virtual IdentityUser User { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Order> Orders { get; set; }
        [NotMapped] public string FullName => $"{FirstName} {Name}";
    }
}