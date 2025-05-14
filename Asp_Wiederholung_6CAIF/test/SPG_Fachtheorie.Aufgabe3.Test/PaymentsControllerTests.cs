using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;
using SPG_Fachtheorie.Aufgabe3.Dtos;
using System.Net.Http.Headers;
using SPG_Fachtheorie.Aufgabe3.Commands;

namespace SPG_Fachtheorie.Aufgabe3.Test
{
    public class PaymentsControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory _factory;

        public PaymentsControllerTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData(null, null, 2)]         // Alle Zahlungen
        [InlineData(1, null, 1)]            // Alle Zahlungen von Cashdesk 1
        [InlineData(2, null, 1)]            // Alle Zahlungen von Cashdesk 2
        [InlineData(null, "2024-05-13", 1)] // Alle Zahlungen ab 13.05.2024
        [InlineData(1, "2024-05-13", 0)]    // Keine Zahlungen von Cashdesk 1 ab 13.05.2024
        public async Task GetPayments_WithFilters_ReturnsFilteredResults(int? cashDesk, string? dateFrom, int expectedCount)
        {
            // Arrange
            string url = "/api/payments";
            var queryParams = new List<string>();
            
            if (cashDesk.HasValue)
                queryParams.Add($"cashDesk={cashDesk}");
            
            if (!string.IsNullOrEmpty(dateFrom))
                queryParams.Add($"dateFrom={dateFrom}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            // Act
            var response = await _client.GetAsync(url);
            
            // Assert
            response.EnsureSuccessStatusCode();
            var payments = await response.Content.ReadFromJsonAsync<List<PaymentDto>>();
            Assert.NotNull(payments);
            Assert.Equal(expectedCount, payments.Count);

            // Prüfe, ob alle Datensätze dem Filter entsprechen
            if (cashDesk.HasValue)
            {
                Assert.True(payments.All(p => p.CashDeskNumber == cashDesk.Value));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                var dateFromValue = DateTime.Parse(dateFrom);
                Assert.True(payments.All(p => p.PaymentDateTime >= dateFromValue));
            }
        }

        [Fact]
        public async Task GetPaymentById_WithValidId_ReturnsPayment()
        {
            // Arrange - Da wir eine In-Memory-Datenbank nutzen, ist 1 eine gültige ID
            int paymentId = 1;

            // Act
            var response = await _client.GetAsync($"/api/payments/{paymentId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var payment = await response.Content.ReadFromJsonAsync<PaymentDetailDto>();
            Assert.NotNull(payment);
            Assert.Equal(paymentId, payment.Id);
        }

        [Fact]
        public async Task GetPaymentById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidPaymentId = 999;

            // Act
            var response = await _client.GetAsync($"/api/payments/{invalidPaymentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(2, HttpStatusCode.NotFound)]  // Payment existiert nicht in der Test-Umgebung
        [InlineData(1, HttpStatusCode.BadRequest)] // Bereits bestätigter Payment (ID 1)
        [InlineData(999, HttpStatusCode.NotFound)] // Nicht existierender Payment
        public async Task PatchPayment_WithDifferentScenarios_ReturnsExpectedStatusCode(int paymentId, HttpStatusCode expectedStatusCode)
        {
            // Act
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _client.PatchAsync($"/api/payments/{paymentId}", content);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Theory]
        [InlineData(2, true, HttpStatusCode.NoContent)]  // Payment kann erfolgreich gelöscht werden
        [InlineData(4, true, HttpStatusCode.NoContent)] // Payment 4 wird erfolgreich gelöscht
        [InlineData(1, false, HttpStatusCode.BadRequest)] // Kann Payment 1 nicht ohne Items löschen
        [InlineData(999, true, HttpStatusCode.NotFound)] // Nicht existierender Payment
        public async Task DeletePayment_WithDifferentScenarios_ReturnsExpectedStatusCode(int paymentId, bool deleteItems, HttpStatusCode expectedStatusCode)
        {
            // Act
            var response = await _client.DeleteAsync($"/api/payments/{paymentId}?deleteItems={deleteItems}");

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }
} 