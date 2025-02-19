using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public abstract class Employee
    {
        [Key]
        [MaxLength(50)]
        public string RegistrationNumber { get; private set; } = string.Empty;
        
        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;
        
        public Address? Address { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Type { get; private set; } = string.Empty;

        protected Employee() { }

        protected Employee(string registrationNumber, string firstName, string lastName, Address? address)
        {
            RegistrationNumber = registrationNumber;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            Type = GetType().Name;  // Setzt den Discriminator-Wert automatisch basierend auf dem Klassennamen
        }
    }
}