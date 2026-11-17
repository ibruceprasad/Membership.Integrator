using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Registration.Contracts;

namespace Integrator.Utils;

public   class RegistrationHelpers
{
    public static (bool, Dictionary<string, string?[]> response) ValidateRegistration(RegistrationRequest registrationRequest)
    {
        var errors = new List<ValidationResult>();
        
        var isValid = Validator.TryValidateObject(
            registrationRequest, new ValidationContext(registrationRequest), errors, validateAllProperties: true);

        if (!isValid)
        {
            var details = errors.Select(e => new
            {
                Fields = e.MemberNames.ToArray(),
                Message = e.ErrorMessage
            }).ToList();
            
            var response = errors
                .SelectMany(e => e.MemberNames.DefaultIfEmpty(string.Empty),
                    (e, member) => (member, e.ErrorMessage))
                .GroupBy(x => x.member)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

            return (false, response);
        }
        return (true, new Dictionary<string, string?[]>());
    }
    
 
    
    public static MembershipRequest MaptoMembership(RegistrationRequest registrationRequest)
    {
        const string DefaultSource = "MILKYWAY";
        var membershipRequest = new MembershipRequest()
        {
            Member = new Member()
            {
                GivenName = registrationRequest.FirstName,
                FamilyName = registrationRequest.LastName,
                DateOfBirth = registrationRequest.DateOfBirth,
                Contact = new Contact
                {
                    Email = registrationRequest.Email,
                     
                },
                PlanCode = RegistrationMappingRegister.ToPlanCode(registrationRequest.MembershipType),
                Source = DefaultSource
            }
        };
        return membershipRequest;
    }
    
 
    
 

   
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
}