using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Commands
{
    public record UpdatePaymentItemCommand(
        [Range(1, int.MaxValue, ErrorMessage = "ID must be > 0")]
        int Id,

        [Required(AllowEmptyStrings = false, ErrorMessage = "ArticleName is required")]
        string ArticleName,

        // amount > 0
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be > 0")]
        int Amount,

        // price > 0
        [Range(typeof(decimal), "0.01", "9999999999", ErrorMessage = "Price must be > 0")]
        decimal Price,

        // paymentId > 0
        [Range(1, int.MaxValue, ErrorMessage = "PaymentId must be > 0")]
        int PaymentId,

        DateTime? LastUpdated
    );
}
