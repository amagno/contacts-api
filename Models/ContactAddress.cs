using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactsAPI.Models
{
    [Table("contacts_addresses")]
    public class ContactAddress
    {
        public int Id { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}