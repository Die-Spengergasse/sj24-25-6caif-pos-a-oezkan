using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class CashDesk
    {
        [Key]
        public int Id { get; set; }
        
        [MaxLength(50)]
        public string Location { get; set; } = string.Empty;
        
        public Cashier? Cashier { get; set; }

        protected CashDesk() { }

        public CashDesk(string location, Cashier? cashier)
        {
            Location = location;
            Cashier = cashier;
        }
    }
}