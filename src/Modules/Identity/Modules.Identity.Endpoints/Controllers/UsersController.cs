using Microsoft.AspNetCore.Authentication.JwtBearer;
using Modules.Identity.Application.DTO;
using Modules.Identity.Application.UserUseCase.Me;
using Modules.Identity.Application.UserUseCase.UpdateUser;

namespace Modules.Identity.Endpoints.Controllers;

[Tags("Users")]
[ApiController]
[Route("v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    
    [MapToApiVersion(1)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<MeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MeResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Me(CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new MeQuery(), cancellationToken);

        return result.Match(
            response => Results.Json(new ApiResponse<MeResponse>(response)),
            errors => Results.Json(new ApiResponse<MeResponse>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }

    [MapToApiVersion(1)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateUser([FromRoute] Guid id, UpdateUserCommand updateUserCommand,CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(updateUserCommand, cancellationToken);

        return result.Match(
            response => Results.Json(new ApiResponse<UserDTO>(response)),
            errors => Results.Json(new ApiResponse<UserDTO>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }
}