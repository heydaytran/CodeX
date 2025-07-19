using Authentication.Entities;
using ErrorOr;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Modules.Identity.Application.Auth.FacebookLogin;
using Modules.Identity.Application.Auth.GoogleLogin;
using Modules.Identity.Application.Auth.Login;
using Modules.Identity.Application.Auth.Logout;
using Modules.Identity.Application.Auth.RefreshToken;
using Modules.Identity.Application.ForgotPassword;
using Modules.Identity.Application.ResetPassword;
using Modules.Identity.Application.Signup;
using Modules.Identity.Domain.Auth;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Modules.Identity.Endpoints.Controllers;

[Tags("Auth")]
[ApiController]
[Route("v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AuthController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    
    [MapToApiVersion(1)]
    [HttpGet]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Login(LoginQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            response => Results.Json(new ApiResponse<LoginResponse>(response)),
            errors => Results.Json(new ApiResponse<LoginResponse>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }
    
    [MapToApiVersion(1)]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.Match(
            response => Results.Json(new ApiResponse<LoginResponse>(response)),
            errors => Results.Json(new ApiResponse<LoginResponse>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }
    
    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IResult> Signup([FromBody] SignupCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            response => Results.Json(new ApiResponse<Unit>(response)),
            errors => Results.Json(new ApiResponse<Unit>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }
    
    [MapToApiVersion(1)]
    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var result = await _sender.Send(new LogoutCommand(userId), cancellationToken);

        return result.Match(
            response => Results.Json(new ApiResponse<Unit>(response)),
            errors => Results.Json(new ApiResponse<Unit>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }
    
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
                response => Results.Json(new ApiResponse<Unit>(response)),
                errors => Results.Json(new ApiResponse<Unit>(errors.Select(error => new ErrorDetail
                {
                    Code = error.Code,
                    Message = error.Description
                }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }
    
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            response => Results.Json(new ApiResponse<Unit>(response)),
            errors => Results.Json(new ApiResponse<Unit>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }

    [MapToApiVersion(1)]
    [HttpGet("google")]
    [AllowAnonymous]
    public IResult GoogleLogin(CancellationToken cancellationToken = default)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/v1/auth/external-callback/google"
        };

        
        
        return Results.Challenge(properties, new[] { GoogleDefaults.AuthenticationScheme });
    }

    [MapToApiVersion(1)]
    [HttpGet("external-callback/google")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GoogleCallback(CancellationToken cancellationToken = default)
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!result.Succeeded || result.Principal == null)
        {
            return Results.Unauthorized();
        }

        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return Results.NotFound("Email not found in Google account.");
        }

        var command = new GoogleLoginCommand(fullName!, email);
        var res = await _sender.Send(command, cancellationToken);

        return res.Match(
            response => Results.Json(new ApiResponse<LoginResponse>(response)),
            errors => Results.Json(new ApiResponse<LoginResponse>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }


    [HttpGet("facebook")]
    [AllowAnonymous]
    public IResult FacebookLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/v1/auth/external-callback/facebook"
        };

        return Results.Challenge(properties, new[] { FacebookDefaults.AuthenticationScheme });
    }

    [MapToApiVersion(1)]
    [HttpGet("external-callback/facebook")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> FacebookCallback(CancellationToken cancellationToken = default)
    {
        var result = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
        if (!result.Succeeded || result.Principal == null)
        {
            return Results.Unauthorized();
        }

        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
       
        if (string.IsNullOrEmpty(email))
        {
            var facebookId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            email = $"fb_{facebookId}@facebook.local"; // fake email for Facebook login
        }

        var command = new FacebookLoginCommand(fullName!, email);
        var res = await _sender.Send(command, cancellationToken);

        return res.Match(
            response => Results.Json(new ApiResponse<LoginResponse>(response)),
            errors => Results.Json(new ApiResponse<LoginResponse>(errors.Select(error => new ErrorDetail
            {
                Code = error.Code,
                Message = error.Description
            }).ToList()), statusCode: StatusCodes.Status400BadRequest));
    }

}