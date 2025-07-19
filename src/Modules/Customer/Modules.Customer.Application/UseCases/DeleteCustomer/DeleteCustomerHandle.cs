using Application.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Domain.Abstractions;
using Modules.Customer.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Customer.Application.Customer.DeleteCustomer
{
    public class DeleteCustomerHandle(ICustomerRepository customerRepository, TimeProvider timeProvider)
    : ICommandHandler<DeleteCustomerCommand, bool>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<ErrorOr<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.Query().AsTracking()
            .Include(c => c.ContactInformation)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (customer is null)
            {
                return Error.NotFound("Customer.NotFound", "Customer not found.");
            }

            customer.DeletedAt = _timeProvider.GetUtcNow().UtcDateTime;
            await _customerRepository.SaveChangesAsync(false, cancellationToken);

            return true;
        }
    }
}
