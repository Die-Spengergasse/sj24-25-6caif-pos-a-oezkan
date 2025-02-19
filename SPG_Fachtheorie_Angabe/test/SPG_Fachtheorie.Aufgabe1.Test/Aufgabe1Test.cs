using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Linq;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    [Collection("Sequential")]
    public class Aufgabe1Test
    {
        private AppointmentContext GetEmptyDbContext()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppointmentContext>()
                .UseSqlite(connection)
                .Options;

            var db = new AppointmentContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        // Creates an empty DB in Debug\net8.0\cash.db
        [Fact]
        public void CreateDatabaseTest()
        {
            using var db = GetEmptyDbContext();
        }

        [Fact]
        public void AddCashierSuccessTest()
        {
            using var db = GetEmptyDbContext();
            
            var cashier = new Cashier(
                "C001", 
                "Max", 
                "Mustermann", 
                new Address("Teststraße 1", "1234", "Wien"),
                15.5m);
            
            db.Cashiers.Add(cashier);
            db.SaveChanges();
            db.ChangeTracker.Clear();
            
            var loadedCashier = db.Cashiers.Include(c => c.Address).FirstOrDefault(c => c.RegistrationNumber == "C001");
            Assert.NotNull(loadedCashier);
            Assert.Equal("C001", loadedCashier.RegistrationNumber);
            Assert.Equal("Cashier", loadedCashier.Type);
        }

        [Fact]
        public void AddPaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            
            var cashDesk = new CashDesk("Kassa 1", null);
            db.CashDesks.Add(cashDesk);
            
            var payment = new Payment(DateTime.Now, PaymentType.Cash, cashDesk);
            db.Payments.Add(payment);
            db.SaveChanges();
            db.ChangeTracker.Clear();
            
            Assert.Single(db.Payments.ToList());
        }

        [Fact]
        public void EmployeeDiscriminatorSuccessTest()
        {
            using var db = GetEmptyDbContext();
            
            var cashier = new Cashier(
                "C001", 
                "Max", 
                "Mustermann", 
                new Address("Teststraße 1", "1234", "Wien"),
                15.5m);
            
            db.Cashiers.Add(cashier);
            db.SaveChanges();
            db.ChangeTracker.Clear();
            
            var loadedEmployee = db.Employees.Include(e => e.Address).FirstOrDefault(e => e.RegistrationNumber == "C001");
            Assert.NotNull(loadedEmployee);
            Assert.Equal("Cashier", loadedEmployee.Type);
        }
    }
}
