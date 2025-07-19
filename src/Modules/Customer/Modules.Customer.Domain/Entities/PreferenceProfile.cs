using Domain.Primitives;
using Modules.Customer.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Customer.Domain.Entities;

public sealed class PreferenceProfile : Entity<Guid>
{
    public CustomerTypeEnum CustomerType { get; set; }
    public string PreferredDepartureRegion { get; set; } = string.Empty;
    public ContactMethodEnum PreferredContactMethod { get; set; }
    public string PreferredContactTime { get; set; } = string.Empty;
    public string LanguagePreference { get; set; } = string.Empty;
    public string TravelInterests { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}


