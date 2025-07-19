using Microsoft.AspNetCore.Authentication.JwtBearer;
using Modules.Customer.Application.Customer.DeleteCustomer;
using Modules.Customer.Application.Customer.GetCustomer;
using Modules.Customer.Application.Customer.UpdateCustomer;
using Modules.Customer.Application.DTO;

namespace Modules.Customer.Endpoints.Controllers;

[Tags("Customers")]
[ApiController]
[Route("v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = Policies.OnlyThirdParties)]
public class CustomersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    [MapToApiVersion(1)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CustomerDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerDTO>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> CustomersByEmail([FromQuery] string email, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetCustomerByEmailQuery(email), cancellationToken);
        return result.Match(
            response => Results.Json(new ApiResponse<CustomerDTO>(response)),
            errors => Results.Json(new ApiResponse<CustomerDTO>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }

    [MapToApiVersion(1)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerDTO>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> CustomersId(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetCustomerByIDQuery(id), cancellationToken);

        return result.Match(
            response => Results.Json(new ApiResponse<CustomerDTO>(response)),
            errors => Results.Json(new ApiResponse<CustomerDTO>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }


    [MapToApiVersion(1)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerDTO>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateCustomer([FromRoute] Guid id, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        if (id != command.CustomerDTO.Id)
        {
            var error = new ErrorDetail
            {
                Code = "IdMismatch",
                Message = "ID in URL does not match ID in body"
            };

            return Results.Json(new ApiResponse<CustomerDTO>([error]), statusCode: StatusCodes.Status400BadRequest);
        }
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            response => Results.Json(new ApiResponse<CustomerDTO>(response)),
            errors => Results.Json(new ApiResponse<CustomerDTO>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }

    [MapToApiVersion(1)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteCustomer(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCustomerCommand(id), cancellationToken);

        return result.Match<IResult>(
            response => Results.Json(new ApiResponse<bool>(response)),
            errors => Results.Json(
                new ApiResponse<object>(
                    errors.Select(error => new ErrorDetail
                    {
                        Code = error.Code,
                        Message = error.Description
                    }).ToList()),
                statusCode: StatusCodes.Status404NotFound
            )
        );
    }


}