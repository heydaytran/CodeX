using Application.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Application.DTO;
using Modules.Customer.Application.Mapper;
using Modules.Customer.Domain.Abstractions;
using Modules.Customer.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Customer.Application.Customer.GetCustomer;

public class GetCustomerByIDHandler(ICustomerRepository customerRepository, ICustomerMapper customerMapper) : IQueryHandler<GetCustomerByIDQuery, CustomerDTO>
{
	private readonly ICustomerRepository _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    private readonly ICustomerMapper _customerMapper = customerMapper ?? throw new ArgumentNullException(nameof(customerMapper));

    public async Task<ErrorOr<CustomerDTO>> Handle(GetCustomerByIDQuery request, CancellationToken cancellationToken)
    {
        //var customer = await _dbContext.Customers
        //    .Include(c => c.ContactInformation)
        //    .Where(c => c.Id == request.Id)
        //    .FirstOrDefaultAsync(cancellationToken);

        var customer = await _customerRepository.Query()
            .Include(c => c.ContactInformation)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

		if (customer is null)
        {
            return Error.Failure("Customer not found");
        }

        
        return _customerMapper.CustomerToDto(customer);
    }
}