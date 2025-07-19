namespace Modules.Customer.Application.Customer.GetCustomer;

public record GetCustomerResponse(string? title, string? firstName, string? lastName, DateOnly dateOfBirth, string? gender, string? email, string? phoneNumber, string? mobileNumber, string? addressLine1, string? addressLine2, string? city, string? postcode, string? county, string? country);