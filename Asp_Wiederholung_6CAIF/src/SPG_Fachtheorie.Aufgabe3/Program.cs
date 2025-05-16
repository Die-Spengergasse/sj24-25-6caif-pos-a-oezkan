using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Services;
using System.Runtime.InteropServices.Marshalling;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);
        var app = builder.Build();
        ConfigureApp(app);
        app.Run();
    }

    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Service provider
        builder.Services.AddDbContext<AppointmentContext>(opt =>
        {
            opt.UseSqlite("DataSource=cash.db");
        });
        builder.Services.AddScoped<EmployeeService>();
        builder.Services.AddScoped<PaymentService>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void ConfigureApp(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            using (var db = scope.ServiceProvider.GetRequiredService<AppointmentContext>())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Seed();
            }
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.MapControllers();
    }
}