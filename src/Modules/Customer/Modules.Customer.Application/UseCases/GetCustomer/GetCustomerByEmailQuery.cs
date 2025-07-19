using Application.Messaging;
using Modules.Customer.Application.DTO;

namespace Modules.Customer.Application.Customer.GetCustomer;

public record GetCustomerByEmailQuery(string Email) : IQuery<CustomerDTO>;
public record GetCustomerByIDQuery(Guid Id) : IQuery<CustomerDTO>;