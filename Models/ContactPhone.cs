using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactsAPI.Models
{
    [Table("contacts_phones")]
    public class ContactPhone
    {
        public int Id { get; set; }
        [Required]
        public string Phone { get; set; }
        // [Required]
        // public int ContactId { get; set; }
    }
}