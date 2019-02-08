using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using ContactsAPI.Models;
using ContactsAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Controllers
{
    [Route("api/contacts")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactsDbContext _db;
        public ContactsController(ContactsDbContext db)
        {
            _db = db;
        }
     
        /// <summary>
        /// Gera contatos aleartoriamente.
        /// </summary>
        [HttpGet("generate/{qtd?}")]
        public async Task<ActionResult<IEnumerable<Contact>>> GenerateContacts([FromRoute] int qtd = 1)
        {
            var fakePhone = new Faker<ContactPhone>()
                .RuleFor(c => c.Phone, (f, c) => f.Phone.PhoneNumber("(##) #####-####"));

            var fakeAddress = new Faker<ContactAddress>()
                .RuleFor(c => c.Street, (f, c) => f.Address.StreetAddress())
                .RuleFor(c => c.State, (f, c) => f.Address.StateAbbr())
                .RuleFor(c => c.ZipCode, (f, c) => f.Address.ZipCode());


            var fakeContact = new Faker<Contact>()
                .RuleFor(c => c.Name, (f, c) => f.Name.FullName())
                .RuleFor(c => c.Email, (f, c) => f.Internet.Email())
                .RuleFor(c => c.Phones, (f, c) => fakePhone.Generate(f.Random.Int(1, 5)))
                .RuleFor(c => c.Addresses, (f, c) => fakeAddress.Generate(f.Random.Int(1, 5)));


            var contacts = fakeContact.Generate(qtd);
            await _db.Contacts.AddRangeAsync(contacts);
            await _db.SaveChangesAsync();

            return contacts;
        }
        /// <summary>
        /// Obter todos os contatos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAllContacts([FromQuery] int? limit, [FromQuery] int? skip)
        {
            if (limit != null && skip == null)
            {
                return await _db.Contacts
                .Include(c => c.Phones)
                .Include(c => c.Addresses)
                .Take((int)limit)
                .ToListAsync();
            }
            if (limit != null && skip != null)
            {
                return await _db.Contacts
                .Include(c => c.Phones)
                .Include(c => c.Addresses)
                .Take((int)limit)
                .Skip((int)skip)
                .ToListAsync();
            }

            return await _db.Contacts
                .Include(c => c.Phones)
                .Include(c => c.Addresses)
                .ToListAsync();
        }
        /// <summary>
        /// Obter um contato por id.
        /// </summary>
        [HttpGet("{contactId:int}")]
        public async Task<ActionResult<Contact>> GetContact([FromRoute] int contactId)
        {
            return await _db.Contacts
                .Where(c => c.Id == contactId)
                .Include(c => c.Phones)
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Criar um novo contato.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AddContact([FromBody] AddContactViewModel model)
        {
            var exists = await _db.Contacts.FirstOrDefaultAsync(c => c.Email == model.Email);

            if (exists != null)
            {
                return BadRequest(new {
                    Email = new List<string> { $"E-mail: {model.Email} already exists" }
                });
            }

            var contact = new Contact
            {
                Name = model.Name,
                Email = model.Email
            };

            if (model.Phones != null)
            {
                contact.Phones = model.Phones.Select(p => new ContactPhone
                {
                    Phone = p.Phone
                }).ToList();
            }

            if (model.Addresses != null)
            {
                contact.Addresses = model.Addresses.Select(a => new ContactAddress
                {
                    Street = a.Street,
                    State = a.State,
                    ZipCode = a.ZipCode
                }).ToList();
            }

            await _db.Contacts.AddAsync(contact);
            await _db.SaveChangesAsync();

            return StatusCode(201);
        }
        /// <summary>
        /// Atualizar um contato por id.
        /// </summary>
        [HttpPut("{contactId:int}")]
        public async Task<ActionResult<Contact>> UpdateContact([FromRoute] int contactId, [FromBody] UpdateContactViewModel model)
        {
            var exists = await _db.Contacts.FirstOrDefaultAsync(c => c.Email == model.Email);

            if (exists != null)
            {
                return BadRequest(new {
                    Email = new List<string> { $"E-mail: {model.Email} already exists" }
                });
            }

            var contact = await _db.Contacts.FindAsync(contactId);

            if (contact == null)
            {
                return BadRequest($"ContactId: {contactId} not exists");
            }
            contact.Name = model.Name;
            contact.Email = model.Email;
      
            _db.Contacts.Update(contact);
            await _db.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Remove um contato por id.
        /// </summary>
        [HttpDelete("phone/{contactId:int}")]
        public async Task<ActionResult> RemoveContact([FromRoute] int contactId)
        {
            var contact = await _db.Contacts.FindAsync(contactId);

            if (contact == null)
            {
                return BadRequest($"ContactId: {contactId} not exists");
            }

            _db.Contacts.Remove(contact);
            await _db.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Adiciona um telefone ao contato.
        /// </summary>
        [HttpPost("phone/{contactId:int}")]
        public async Task<IActionResult> AddPhone([FromRoute] int contactId, [FromBody] PhoneViewModel model)
        {
            var contact = await _db.Contacts.FindAsync(contactId);

            if (contact == null)
            {
                return BadRequest($"ContactId: {contactId} not exists");
            }

            contact.Phones.Add(new ContactPhone { Phone = model.Phone });
            _db.Contacts.Update(contact);
            await _db.SaveChangesAsync();

            return StatusCode(201);
        }
        /// <summary>
        /// Atualizar um telefone por id.
        /// </summary>
        [HttpPut("phone/{contactPhoneId:int}")]
        public async Task<IActionResult> UpdatePhone([FromRoute] int contactPhoneId, [FromBody] PhoneViewModel model)
        {
            var phone = _db.ContactsPhones.Find(contactPhoneId);

            if (phone == null)
            {
                return BadRequest($"ContactPhoneId: {contactPhoneId} not exists");
            }

            phone.Phone = model.Phone;

            _db.ContactsPhones.Update(phone);
            await _db.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Remove um telefone por id.
        /// </summary>
        [HttpDelete("phone/{contactPhoneId:int}")]
        public async Task<IActionResult> RemovePhone([FromRoute] int contactPhoneId)
        {
            var phone = _db.ContactsPhones.Find(contactPhoneId);

            if (phone == null)
            {
                return BadRequest($"ContactPhoneId: {contactPhoneId} not exists");
            }

            _db.ContactsPhones.Remove(phone);
            await _db.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Adiciona um endereço ao contato.
        /// </summary>
        [HttpPost("address/{contactId:int}")]
        public async Task<IActionResult> AddAddress([FromRoute] int contactId, [FromBody] AddressViewModel model)
        {
            var address = new ContactAddress
            {
                Street = model.Street,
                State = model.State,
                ZipCode = model.ZipCode,
            };

            await _db.ContactsAddresses.AddAsync(address);
            await _db.SaveChangesAsync();

            return StatusCode(201);
        }
        /// <summary>
        /// Atualizar um endereço por id.
        /// </summary>
        [HttpPut("address/{contactAddressId:int}")]
        public async Task<IActionResult> UpdateAddress([FromRoute] int contactAddressId, [FromBody] AddressViewModel model)
        {
            var address = _db.ContactsAddresses.Find(contactAddressId);

            if (address == null)
            {
                return BadRequest($"ContactAddressId: {contactAddressId} not exists");
            }

            address.Street = model.Street;
            address.State = model.State;
            address.ZipCode = model.ZipCode;

            _db.ContactsAddresses.Update(address);
            await _db.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Remover um endereço por id.
        /// </summary>
        [HttpDelete("address/{contactAddressId:int}")]
        public async Task<IActionResult> RemoveAddress([FromRoute] int contactAddressId)
        {
            var address = _db.ContactsAddresses.Find(contactAddressId);

            if (address == null)
            {
                return BadRequest($"ContactAddressId: {contactAddressId} not exists");
            }

            _db.ContactsAddresses.Remove(address);
            await _db.SaveChangesAsync();

            return Ok();
        }

    }
}
