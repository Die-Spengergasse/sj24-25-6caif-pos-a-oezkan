namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class PaymentItem
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public Payment Payment { get; set; } = null!;

        protected PaymentItem() { }

        public PaymentItem(int amount, decimal price, Payment payment)
        {
            Amount = amount;
            Price = price;
            Payment = payment;
        }
    }
}