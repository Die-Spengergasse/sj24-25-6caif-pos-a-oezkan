using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Commands
{
    public record UpdateConfirmedCommand : IValidatableObject
    {
        public DateTime? Confirmed { get; init; }
        public UpdateConfirmedCommand() { }

        public UpdateConfirmedCommand(DateTime? confirmed)
        {
            Confirmed = confirmed;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Confirmed.HasValue)
            {
                var now = DateTime.UtcNow;
                if (Confirmed.Value > now.AddMinutes(1))
                {
                    yield return new ValidationResult(
                        "Confirmed cannot be more than 1 minute in the future.",
                        new[] { nameof(Confirmed) }
                    );
                }
            }
        }
    }
}
