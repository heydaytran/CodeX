
using Persistence;
using Modules.Customer.Domain.Entities;

namespace Modules.Customer.Domain.Abstractions;
    public interface ICustomerRepository : IGenericRepository<Domain.Entities.Customer, Guid>
    {
        Task<Domain.Entities.Customer?> GetByEmailAsync(string email);
    }


