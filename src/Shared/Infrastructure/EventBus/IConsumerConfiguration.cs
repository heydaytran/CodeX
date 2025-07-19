namespace Infrastructure.EventBus;

public interface IConsumerConfiguration
{
    void AddConsumers(IRegistrationConfigurator registrationConfigurator);
}