using Core.Models.RabbitMQ.Responses;
using EasyNetQ;

namespace PongService.RabbitMQ;

public class MessageHandler : BackgroundService
{
    private void HandleCredentialValidationResponse(CredentialValidationResponse response)
    {
        // Do something ...
    }

    private void HandlePermissionResponse(PermissionResponse response)
    {
        // Do something ...
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest")
        );

        // Listen for a message with a given TOPIC, TYPE, and then assign a method to handle it
        messageClient.Listen<CredentialValidationResponse>(HandleCredentialValidationResponse, "credential-validation-response");
        messageClient.Listen<PermissionResponse>(HandlePermissionResponse, "permission-response");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
