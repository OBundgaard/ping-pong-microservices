using Core.Models.RabbitMQ.Requests;
using EasyNetQ;

namespace PingService.RabbitMQ;

public class MessageHandler : BackgroundService
{
    private void HandleCredentialValidationRequest(CredentialValidationRequest request)
    {
        // Do something ...
    }

    private void HandlePermissionRequest(PermissionRequest request)
    {
        // Do something ...
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest")
        );

        // Listen for a message with a given TOPIC, TYPE, and then assign a method to handle it
        messageClient.Listen<CredentialValidationRequest>(HandleCredentialValidationRequest, "credential-validation-response");
        messageClient.Listen<PermissionRequest>(HandlePermissionRequest, "permission-response");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
