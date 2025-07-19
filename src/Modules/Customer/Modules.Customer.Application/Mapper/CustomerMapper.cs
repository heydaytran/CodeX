using Modules.Customer.Application.DTO;
using Riok.Mapperly.Abstractions;
using Modules.Customer.Domain.Entities;
using System.Runtime.Serialization;

namespace Modules.Customer.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CustomerMapper : ICustomerMapper
{
    [MapProperty(nameof(Domain.Entities.Customer.ContactInformation), nameof(CustomerDTO.ContactInformation))]
    public partial CustomerDTO CustomerToDto(Domain.Entities.Customer customer);


    [MapProperty(nameof(CustomerDTO.ContactInformation), nameof(Domain.Entities.Customer.ContactInformation))]
    //[MapperIgnoreTarget(nameof(Domain.Entities.Customer.ContactInformation.Id))]
    public partial Domain.Entities.Customer CustomerFromDto(CustomerDTO customerDTO);


    [MapProperty(nameof(CustomerDTO.ContactInformation), nameof(Domain.Entities.Customer.ContactInformation))]
    public partial void UpdateCustomerFromDto(CustomerDTO dto, Domain.Entities.Customer target);
}
