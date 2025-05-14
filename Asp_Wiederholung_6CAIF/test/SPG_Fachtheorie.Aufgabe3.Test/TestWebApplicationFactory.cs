using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe1.Services;
using System.Linq;

namespace SPG_Fachtheorie.Aufgabe3.Test
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Komplett neue ServiceCollection erstellen, um Konflikte zu vermeiden
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Entferne bestehende DbContext-Registrierung
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppointmentContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Entferne bestehende Services mit manueller Iteration
                var paymentServiceDescriptor = services.Where(d => d.ServiceType == typeof(PaymentService)).ToList();
                foreach (var desc in paymentServiceDescriptor)
                {
                    services.Remove(desc);
                }
                
                var employeeServiceDescriptor = services.Where(d => d.ServiceType == typeof(EmployeeService)).ToList();
                foreach (var desc in employeeServiceDescriptor)
                {
                    services.Remove(desc);
                }

                // In-Memory-Datenbank registrieren
                services.AddDbContext<AppointmentContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Services neu registrieren
                services.AddScoped<EmployeeService>();
                services.AddScoped<PaymentService>();

                // Build Service Provider für die Test-Datenbank-Initialisierung
                var sp = services.BuildServiceProvider();

                // Datenbank initialisieren
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppointmentContext>();
                    db.Database.EnsureCreated();

                    // Testdaten einfügen
                    DataGenerator.SeedDatabase(db);
                }
            });
        }
    }

    public static class DataGenerator
    {
        public static void SeedDatabase(AppointmentContext db)
        {
            // Löschen aller vorhandenen Daten
            db.PaymentItems.RemoveRange(db.PaymentItems);
            db.Payments.RemoveRange(db.Payments);
            db.CashDesks.RemoveRange(db.CashDesks);
            db.Employees.RemoveRange(db.Employees);
            db.SaveChanges();

            // Testdaten hinzufügen
            var cashDesk1 = new CashDesk(1);
            var cashDesk2 = new CashDesk(2);
            db.CashDesks.AddRange(cashDesk1, cashDesk2);

            var today = DateOnly.FromDateTime(DateTime.Now);
            var manager = new Manager(
                123, "Manager", "Max", today, 3000m, null, "Audi");
            var cashier = new Cashier(
                456, "Cashier", "Carl", today, 2000m, null, "Kassa");
            
            db.Employees.AddRange(manager, cashier);

            // Zahlung 1 - bestätigt, Cashdesk 1
            var payment1 = new Payment(
                cashDesk1, 
                new DateTime(2024, 5, 10, 10, 0, 0, DateTimeKind.Utc), 
                cashier, 
                PaymentType.Cash)
            {
                Confirmed = new DateTime(2024, 5, 10, 10, 15, 0, DateTimeKind.Utc)
            };
            db.Payments.Add(payment1);

            // Zahlung 2 - nicht bestätigt, Cashdesk 1
            var payment2 = new Payment(
                cashDesk1, 
                new DateTime(2024, 5, 13, 9, 0, 0, DateTimeKind.Utc), 
                cashier, 
                PaymentType.Cash);
            db.Payments.Add(payment2);

            // Zahlung 3 - bestätigt, Cashdesk 2
            var payment3 = new Payment(
                cashDesk2, 
                new DateTime(2024, 5, 13, 11, 0, 0, DateTimeKind.Utc), 
                manager, 
                PaymentType.CreditCard)
            {
                Confirmed = new DateTime(2024, 5, 13, 11, 30, 0, DateTimeKind.Utc)
            };
            db.Payments.Add(payment3);

            // Zahlung 4 - nicht bestätigt, Cashdesk 2
            var payment4 = new Payment(
                cashDesk2, 
                new DateTime(2024, 5, 14, 14, 0, 0, DateTimeKind.Utc), 
                manager, 
                PaymentType.Cash);
            db.Payments.Add(payment4);

            // Zahlungspositionen hinzufügen
            db.PaymentItems.Add(new PaymentItem("Artikel 1", 2, 9.99m, payment1));
            db.PaymentItems.Add(new PaymentItem("Artikel 2", 1, 19.99m, payment1));

            db.PaymentItems.Add(new PaymentItem("Artikel 3", 3, 5.99m, payment2));

            db.PaymentItems.Add(new PaymentItem("Artikel 4", 1, 99.99m, payment3));
            db.PaymentItems.Add(new PaymentItem("Artikel 5", 2, 49.99m, payment3));

            db.PaymentItems.Add(new PaymentItem("Artikel 6", 1, 29.99m, payment4));

            db.SaveChanges();
        }
    }
} 