using Application.Messaging;
using Modules.Customer.Application.DTO;

namespace Modules.Customer.Application.Customer.UpdateCustomer;

public class UpdateCustomerCommand : ICommand<CustomerDTO>
{
    public CustomerDTO CustomerDTO { get; set; } = new CustomerDTO();

}