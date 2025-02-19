namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Cashier : Employee
    {
        public decimal HourlyRate { get; set; }

        protected Cashier() : base() { }

        public Cashier(string registrationNumber, string firstName, string lastName, Address? address, decimal hourlyRate) 
            : base(registrationNumber, firstName, lastName, address)
        {
            HourlyRate = hourlyRate;
        }
    }
}