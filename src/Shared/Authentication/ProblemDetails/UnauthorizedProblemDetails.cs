namespace Authentication.ProblemDetails;

public class UnauthorizedProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
{
    public UnauthorizedProblemDetails(string? details = null)
    {
        Title = "Unauthorized";
        Detail = details;
        Status = 401;
        Type = "https://httpstatuses.com/401";
    }
}