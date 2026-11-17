using System.Net.Http.Json;
using System.Text.Json;
using Integrator.Utils;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Registration.Contracts;

namespace Integrator.Functions;

public class RegistrationFunction(IHttpClientFactory httpClientFactory, ILogger<RegistrationFunction> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    //[Authorize(AuthenticationSchemes = "integrator-scheme")]
    [Function("Register")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "registrations")] HttpRequest req,  IMapper mapper)
    {
        try
        {
            RegistrationRequest? registrationRequest = await JsonSerializer.DeserializeAsync<RegistrationRequest>(req.Body, JsonOptions); 
            
            if (registrationRequest is null)
            {
                logger.LogError("Request body is null or invalid.");
                return new BadRequestObjectResult("Invalid request body.");
            }
            
            var (validationResult, validationErrors) = RegistrationHelpers.ValidateRegistration(registrationRequest);
            if (!validationResult) 
            {
                logger.LogError("Invalid registration data." + " Validation errors: {ValidationErrors}", JsonSerializer.Serialize(validationErrors));
                return new BadRequestObjectResult("Invalid registration data.");
            }
            
            var membershipRequest = RegistrationHelpers.MaptoMembership(registrationRequest);
            
            var client = httpClientFactory.CreateClient("MembershipApi");
            using var response = await client.PostAsJsonAsync("api/membership", membershipRequest);
             if (!response.IsSuccessStatusCode)
             {
                 var body = await response.Content.ReadAsStringAsync();
                 logger.LogError("Membership API failed {Status}: {Body}", response.StatusCode, body);
                 return new ObjectResult(new { error = "Membership API error", details = body })
                 {
                     StatusCode = (int)response.StatusCode
                 };
             }
        } 
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize request body: {requestBody}", req.Body);
            return new BadRequestObjectResult("Invalid JSON format.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred. Request body: {requestBody}", req.Body);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        return new AcceptedResult("Registration accepted for processing.", null);
    }
}