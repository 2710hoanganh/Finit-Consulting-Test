namespace TechnicalTest.Api.Infrastructure.MessageQueue;

public interface IMessageQueueService
{
    Task PublishAsync<T>(T message, string queueName);
    Task SubscribeAsync<T>(string queueName, Func<T, Task> handler);
}
