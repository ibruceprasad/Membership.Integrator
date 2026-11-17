using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Registration.Contracts;

public class RegistrationRequest
{
    [Required, StringLength(30, MinimumLength = 2)]
    [RegularExpression(@"^[\p{L}][\p{L}\s'-]*$",
        ErrorMessage = "FirstName may only contain letters, spaces, hyphens and apostrophes.")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(30, MinimumLength = 2)]
    [RegularExpression(@"^[\p{L}][\p{L}\s'-]*$",
        ErrorMessage = "Lastname may only contain letters, spaces, hyphens and apostrophes.")]
    public string LastName { get; set; } = string.Empty;

    [Required, DateFormat("yyyy-MM-dd")]
    public string DateOfBirth { get; set; }

    [Required, EmailAddress, StringLength(100)]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "Email must be a valid address, e.g. name@example.com.")]
    public string Email { get; set; } = string.Empty;
    
    [Required, EnumDataType(typeof(MembershipType))]
    public string MembershipType { get; set; }
 
    [Required,DateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'")]
    public string RegisteredAt { get; set; }
}
 
 
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MembershipType
{
    Single = 1,
    Family = 2,
    Couple = 3
}



