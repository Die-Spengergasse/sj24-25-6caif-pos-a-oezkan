using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    [Owned]
    public class Address
    {
        [MaxLength(255)]
        public string Street { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string Zip { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string City { get; set; } = string.Empty;

        protected Address() { }

        public Address(string street, string zip, string city)
        {
            Street = street;
            Zip = zip;
            City = city;
        }
    }
}