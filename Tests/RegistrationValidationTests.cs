using System.ComponentModel.DataAnnotations;
using Integrator.Utils;
using Registration.Contracts;

namespace Tests;

public class RegistrationValidationTests
{
    [Fact]
    public void WhenValidRegistration_ThenValidationPassedSuccessfully()
    {
        var registration = new RegistrationRequest
        {
            FirstName = "John",
            LastName = "Smith",
            DateOfBirth = "1990-01-31",
            Email = "john@example.com",
            MembershipType = "Single",
            RegisteredAt = "2026-01-01T10:00:00Z"
        };
        bool isValid;
        Dictionary<string, string?[]> errors;
        (isValid, errors) = RegistrationHelpers.ValidateRegistration(registration);
        
        Assert.True(isValid);
        Assert.Empty(errors);
    }
    [Fact]
    public void WhenMissingRequiredFirstName_ThenValidationFails()
    {
        var registration = new RegistrationRequest
        {
            FirstName = "",
            LastName = "Smith",
            DateOfBirth = "1990-01-31",
            Email = "john@example.com",
            MembershipType = "Single",
            RegisteredAt = "2026-01-01T10:00:00Z"
        };
        bool isValid;
        Dictionary<string, string?[]> errors;
        (isValid, errors) = RegistrationHelpers.ValidateRegistration(registration);
        
        Assert.False(isValid);
        Assert.Contains("FirstName", errors.Keys);
        Assert.Equal("The FirstName field is required.", errors["FirstName"]?[0]);
        Assert.NotEmpty(errors);
    }
    
    [Fact]
    public void WhenInvalidDateOfBirth_ThenValidationFails()
    {
        var registration = new RegistrationRequest
        {
            FirstName = "John",
            LastName = "Smith",
            DateOfBirth = "1990/01/31",
            Email = "john@example.com",
            MembershipType = "Single",
            RegisteredAt = "2026-01-01T10:00:00Z"
        };
        bool isValid;
        Dictionary<string, string?[]> errors;
        (isValid, errors) = RegistrationHelpers.ValidateRegistration(registration);
        
        Assert.False(isValid);
        Assert.Contains("DateOfBirth", errors.Keys);
        Assert.Equal("DateOfBirth must be in yyyy-MM-dd format.", errors["DateOfBirth"]?[0]);
        Assert.NotEmpty(errors);
    }
    
    [Fact]
    public void WhenInvalidMembership_ThenValidationFails()
    {
        var registration = new RegistrationRequest
        {
            FirstName = "John",
            LastName = "Smith",
            DateOfBirth = "1990/01/31",
            Email = "john@example.com",
            MembershipType = "Unknown",
            RegisteredAt = "2026-01-01T10:00:00Z"
        };
        bool isValid;
        Dictionary<string, string?[]> errors;
        (isValid, errors) = RegistrationHelpers.ValidateRegistration(registration);
        
        Assert.False(isValid);
        Assert.Contains("MembershipType", errors.Keys);
        Assert.Contains("The field MembershipType is invalid.", errors["MembershipType"]?[0]);
        Assert.NotEmpty(errors);
    }
    
}