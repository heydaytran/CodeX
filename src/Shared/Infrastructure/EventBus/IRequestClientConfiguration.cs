namespace Infrastructure.EventBus;

public interface IRequestClientConfiguration
{
    void AddRequestClients(IRegistrationConfigurator registrationConfigurator);
}
