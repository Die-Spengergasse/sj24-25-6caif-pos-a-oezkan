using System;
using System.Collections.Generic;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public PaymentType Type { get; set; }
        public CashDesk CashDesk { get; set; } = null!;
        public List<PaymentItem> Items { get; set; } = new();

        protected Payment() { }

        public Payment(DateTime dateTime, PaymentType type, CashDesk cashDesk)
        {
            DateTime = dateTime;
            Type = type;
            CashDesk = cashDesk;
        }
    }
}