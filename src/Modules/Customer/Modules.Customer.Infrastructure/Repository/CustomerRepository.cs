using Persistence;
using Modules.Customer.Domain.Entities;
using Modules.Customer.Persistence;
using Modules.Customer.Domain.Abstractions;


namespace Modules.Customer.Infrastructure.Repository;

public class CustomerRepository : GenericRepository<Domain.Entities.Customer, Guid>, ICustomerRepository
{
    public CustomerRepository(CustomerDbContext context) : base(context)
    {
    }

    public async Task<Domain.Entities.Customer?> GetByEmailAsync(string email)
    {
        return await _dbSet
                   .Include(c => c.ContactInformation)
                   .FirstOrDefaultAsync(c => c.ContactInformation.Email == email);
    }
}
