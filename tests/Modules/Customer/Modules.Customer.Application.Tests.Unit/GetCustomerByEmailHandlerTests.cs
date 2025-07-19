using Modules.Customer.Application.Customer.GetCustomer;
using Modules.Customer.Application.DTO;
using Modules.Customer.Application.Mapper;
using Modules.Customer.Domain.Abstractions;
using Modules.Customer.Domain.Entities;

public class GetCustomerByEmailHandlerTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly ICustomerMapper _mapper = Substitute.For<ICustomerMapper>();

    [Fact]
    public async Task Handle_ShouldReturnError_WhenCustomerNotFound()
    {
        var query = new GetCustomerByEmailQuery("test@example.com");
        _repository.GetByEmailAsync(query.Email).Returns(Task.FromResult<Customer?>(null));
        var handler = new GetCustomerByEmailHandler(_repository, _mapper);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Be("Customer not found");
        await _mapper.DidNotReceive().CustomerToDto(Arg.Any<Customer>());
    }

    [Fact]
    public async Task Handle_ShouldReturnCustomerDto_WhenCustomerExists()
    {
        var customer = new Customer { Id = Guid.NewGuid(), ContactInformation = new ContactInformation { Email = "test@example.com" } };
        var dto = new CustomerDTO { Id = customer.Id, ContactInformation = new ContactInformationDTO { Email = customer.ContactInformation.Email } };
        var query = new GetCustomerByEmailQuery(customer.ContactInformation.Email);
        _repository.GetByEmailAsync(query.Email).Returns(Task.FromResult<Customer?>(customer));
        _mapper.CustomerToDto(customer).Returns(dto);
        var handler = new GetCustomerByEmailHandler(_repository, _mapper);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().Be(dto);
        await _mapper.Received(1).CustomerToDto(customer);
    }
}
