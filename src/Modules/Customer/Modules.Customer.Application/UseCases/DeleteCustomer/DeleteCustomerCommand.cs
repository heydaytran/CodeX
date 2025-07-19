using Application.Messaging;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Customer.Application.Customer.DeleteCustomer
{
    public record DeleteCustomerCommand(Guid Id) : ICommand<bool>;
}
