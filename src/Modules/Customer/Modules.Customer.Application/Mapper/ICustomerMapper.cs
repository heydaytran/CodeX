using Modules.Customer.Application.DTO;


namespace Modules.Customer.Application.Mapper;

public interface ICustomerMapper
{
    CustomerDTO CustomerToDto(Domain.Entities.Customer customer);
    Domain.Entities.Customer CustomerFromDto(CustomerDTO customerDTO);
    void UpdateCustomerFromDto(CustomerDTO dto, Domain.Entities.Customer target);
}
