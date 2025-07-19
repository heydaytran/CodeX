namespace Modules.Customer.Infrastructure.ServiceInstallers;
using Domain.Abstractions;
using Modules.Customer.Infrastructure.Repository;

internal sealed class PersistenceServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSingleton<SoftDeleteInterceptor>()
            .AddDbContext<CustomerDbContext>((serviceProvider, options) =>
            {
                var connectionString = serviceProvider
                    .GetRequiredService<IOptions<ConnectionStringOptions>>().Value;

                var interceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.UseRelationalNulls();
                }).AddInterceptors(interceptor);
            })
            .AddScoped<ICustomerRepository, CustomerRepository>();

	// .AddScoped<ICheckoutsUnitOfWork, CheckoutsUnitOfWork>()
	// .AddScoped(typeof(IGenericRepository<,>), typeof(CheckoutRepository<,>));

}