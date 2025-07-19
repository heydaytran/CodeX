using Modules.Customer.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Customer.Application.DTO;

public class CustomerDTO
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    //public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public ContactInformationDTO ContactInformation { get; set; } = new ContactInformationDTO();

}
