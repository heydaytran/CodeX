using Application.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Application.DTO;
using Modules.Customer.Application.Mapper;
using Modules.Customer.Domain.Abstractions;
using Modules.Customer.Persistence;
using System.Linq;

namespace Modules.Customer.Application.Customer.UpdateCustomer;

public class UpdateCustomerHandler(ICustomerRepository customerRepository, 
    TimeProvider timeProvider,
    ICustomerMapper customerMapper) 
    : ICommandHandler<UpdateCustomerCommand, CustomerDTO>
{
	private readonly ICustomerRepository _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
	private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ICustomerMapper _customerMapper = customerMapper ?? throw new ArgumentNullException(nameof(customerMapper));


    public async Task<ErrorOr<CustomerDTO>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Get customer by ID
        var customer = await _customerRepository.Query()
            .Include(c => c.ContactInformation).AsTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CustomerDTO.Id, cancellationToken);

        // Update logic
        if (customer is null)
        {
            return Error.NotFound("Customer not found");
        }
        else
        {
            _customerMapper.UpdateCustomerFromDto(request.CustomerDTO, customer); // this make customer enitity updated with dto --> entity status was changed to modified
            customer.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;
        }

        // Save changes to the database
        var res = await _customerRepository.SaveChangesAsync(false,cancellationToken);
        if (res.IsError)
        {
            return res.Errors;
        }

        return request.CustomerDTO;
    }
}