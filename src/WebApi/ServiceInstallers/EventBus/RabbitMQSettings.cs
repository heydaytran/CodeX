namespace WebApi.ServiceInstallers.EventBus;

internal class RabbitMqSettings
{
    public string? Host { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? VirtualHost { get; set; }
}