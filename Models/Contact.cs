using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactsAPI.Models
{
    [Table("contacts")]
    public class Contact
    {
        public Contact()
        {
            Phones = new List<ContactPhone>();
            Addresses = new List<ContactAddress>();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public List<ContactPhone> Phones { get; set; }
        public List<ContactAddress> Addresses { get; set; }
    }
}