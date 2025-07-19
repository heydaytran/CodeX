using Infrastructure.EventBus;

namespace Infrastructure.Extensions;

public static class RegistrationConfiguratorExtensions
{
    public static void AddConsumersFromAssemblies(this IRegistrationConfigurator registrationConfigurator, params Assembly[] assemblies) =>
        InstanceFactory
            .CreateFromAssemblies<IConsumerConfiguration>(assemblies)
            .ForEach(consumerInstaller => consumerInstaller.AddConsumers(registrationConfigurator));

    public static void AddRequestClientsFromAssemblies(this IRegistrationConfigurator registrationConfigurator, params Assembly[] assemblies) =>
        InstanceFactory
            .CreateFromAssemblies<IRequestClientConfiguration>(assemblies)
            .ForEach(consumerInstaller => consumerInstaller.AddRequestClients(registrationConfigurator));
}