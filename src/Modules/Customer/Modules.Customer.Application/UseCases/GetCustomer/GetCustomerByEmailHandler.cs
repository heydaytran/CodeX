using Application.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Application.DTO;
using Modules.Customer.Application.Mapper;
using Modules.Customer.Domain.Abstractions;
using Modules.Customer.Persistence;

namespace Modules.Customer.Application.Customer.GetCustomer;

public class GetCustomerByEmailHandler(ICustomerRepository customerRepository, ICustomerMapper customerMapper) : IQueryHandler<GetCustomerByEmailQuery, CustomerDTO>
{
    private readonly ICustomerRepository _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    private readonly ICustomerMapper _customerMapper = customerMapper ?? throw new ArgumentNullException(nameof(customerMapper));


    public async Task<ErrorOr<CustomerDTO>> Handle(GetCustomerByEmailQuery request, CancellationToken cancellationToken)
    {
        //richard
        var customer = await _customerRepository.GetByEmailAsync(request.Email);

        if (customer is null)
        {
            return Error.Failure("Customer not found");
        }

        var customerDTO = _customerMapper.CustomerToDto(customer);

        return customerDTO;
    }
}