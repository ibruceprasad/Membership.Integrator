using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Registration.Contracts;

  [AttributeUsage(AttributeTargets.Property)]
    public sealed class DateFormatAttribute(string format = "yyyy-MM-dd") : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            return value is string s && !string.IsNullOrEmpty(s) &&
                   DateOnly.TryParseExact(s, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                ? ValidationResult.Success
                : new ValidationResult($"{context.DisplayName} must be in {format} format.",
                    [context.MemberName!]);
        }
    }
 