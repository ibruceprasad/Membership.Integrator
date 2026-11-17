using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Registration.Contracts;

public class MembershipRequest
{
    [JsonPropertyName("member")]
    [Required]
    public Member? Member { get; set; }
}

public class Member
{
    [JsonPropertyName("given_name")]
    [Required, StringLength(30, MinimumLength = 2)]
    [RegularExpression(@"^[\p{L}][\p{L}\s'-]*$",
        ErrorMessage = "given_name may only contain letters, spaces, hyphens and apostrophes.")]
    public string GivenName { get; set; } = string.Empty;

    [JsonPropertyName("family_name")]
    [Required, StringLength(30, MinimumLength = 2)]
    [RegularExpression(@"^[\p{L}][\p{L}\s'-]*$",
        ErrorMessage = "family_name may only contain letters, spaces, hyphens and apostrophes.")]
    public string FamilyName { get; set; } = string.Empty;

    [JsonPropertyName("dob")]
    [Required, DateFormat("dd/MM/yyyy")]
    public string DateOfBirth { get; set; }

    [JsonPropertyName("contact")]
    [Required]
    public Contact? Contact { get; set; }

    [JsonPropertyName("plan_code")]
    [JsonRequired]
    [AllowedValues('S', 'F', 'C')]
    public char PlanCode { get; set; }

    [JsonPropertyName("source")]
    [Required, StringLength(50)]
    public string Source { get; set; } = string.Empty;     // e.g. "MILKYWAY"
}

public class Contact
{
    [JsonPropertyName("email")]
    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;
}