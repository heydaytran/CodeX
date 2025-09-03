using Domain.Primitives;
using Modules.Customer.Domain.Events;

namespace Modules.Customer.Domain.Entities;

public sealed class Customer : AggregateRoot<Guid>
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public string Gender { get; private set; } = string.Empty;
    public ContactInformation ContactInformation { get; private set; } = new();
    public bool EmailNotificationsEnabled { get; private set; }
    public bool SmsNotificationsEnabled { get; private set; }

    private Customer() { }

    private Customer(Guid id) : base(id) { }

    public static Customer Create(
        Guid id,
        Guid tenantId,
        Guid userId,
        string title,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string gender,
        ContactInformation contactInformation,
        bool emailNotificationsEnabled,
        bool smsNotificationsEnabled)
    {
        var customer = new Customer(id);
        var @event = new CustomerCreatedDomainEvent(
            id,
            tenantId,
            userId,
            title,
            firstName,
            lastName,
            dateOfBirth,
            gender,
            contactInformation,
            emailNotificationsEnabled,
            smsNotificationsEnabled);
        customer.ApplyChange(@event);
        return customer;
    }

    public void UpdateDetails(
        string title,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string gender,
        ContactInformation contactInformation)
    {
        var @event = new CustomerDetailsUpdatedDomainEvent(
            Id,
            title,
            firstName,
            lastName,
            dateOfBirth,
            gender,
            contactInformation);
        ApplyChange(@event);
    }

    public void UpdatePreferences(bool emailNotificationsEnabled, bool smsNotificationsEnabled)
    {
        var @event = new CustomerPreferencesUpdatedDomainEvent(
            Id,
            emailNotificationsEnabled,
            smsNotificationsEnabled);
        ApplyChange(@event);
    }

    public void Delete()
    {
        var @event = new CustomerDeletedDomainEvent(Id);
        ApplyChange(@event);
    }

    private void On(CustomerCreatedDomainEvent @event)
    {
        Id = @event.CustomerId;
        TenantId = @event.TenantId;
        UserId = @event.UserId;
        Title = @event.Title;
        FirstName = @event.FirstName;
        LastName = @event.LastName;
        DateOfBirth = @event.DateOfBirth;
        Gender = @event.Gender;
        ContactInformation = @event.ContactInformation;
        EmailNotificationsEnabled = @event.EmailNotificationsEnabled;
        SmsNotificationsEnabled = @event.SmsNotificationsEnabled;
    }

    private void On(CustomerDetailsUpdatedDomainEvent @event)
    {
        Title = @event.Title;
        FirstName = @event.FirstName;
        LastName = @event.LastName;
        DateOfBirth = @event.DateOfBirth;
        Gender = @event.Gender;
        ContactInformation = @event.ContactInformation;
    }

    private void On(CustomerPreferencesUpdatedDomainEvent @event)
    {
        EmailNotificationsEnabled = @event.EmailNotificationsEnabled;
        SmsNotificationsEnabled = @event.SmsNotificationsEnabled;
    }

    private void On(CustomerDeletedDomainEvent _)
    {
        DeletedAt = DateTime.UtcNow;
    }

    public static Customer Rehydrate(IEnumerable<IDomainEvent> events)
    {
        var customer = new Customer();
        customer.Load(events);
        return customer;
    }
}

