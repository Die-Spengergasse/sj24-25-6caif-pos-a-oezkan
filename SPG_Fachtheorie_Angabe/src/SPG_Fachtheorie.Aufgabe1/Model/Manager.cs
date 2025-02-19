namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Manager : Employee
    {
        public decimal Salary { get; set; }

        protected Manager() : base() { }

        public Manager(string registrationNumber, string firstName, string lastName, Address? address, decimal salary) 
            : base(registrationNumber, firstName, lastName, address)
        {
            Salary = salary;
        }
    }
}