using Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Customer.Domain.Entities;

public sealed class MarketingCompliance : Entity<Guid>
{
    public Guid CustomerId { get; set; }
    public bool MarketingOptIn { get; set; }
    public bool EmailOptIn { get; set; }
    public bool SmsOptIn { get; set; }
    public DateTime? GdprConsentDate { get; set; }
    public bool DoNotContact { get; set; }
}

