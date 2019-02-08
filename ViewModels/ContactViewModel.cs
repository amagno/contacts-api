using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContactsAPI.ViewModels
{
    public class PhoneViewModel
    {
        [Required]
        public string Phone { get; set; }
    }
    public class AddressViewModel
    {
        [Required]
        public string Street { get; set; }
        [Required]
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
    public class AddContactViewModel
    {
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public List<PhoneViewModel> Phones { get; set; }
        public List<AddressViewModel> Addresses { get; set; }
    }
    public class UpdateContactViewModel
    {
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}