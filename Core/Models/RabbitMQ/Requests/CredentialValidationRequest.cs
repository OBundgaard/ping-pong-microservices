

namespace Core.Models.RabbitMQ.Requests;

public class CredentialValidationRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
