namespace Authentication.ProblemDetails;

public class ForbiddenProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
{
    public ForbiddenProblemDetails(string? details = null)
    {
        Title = "Forbidden";
        Detail = details;
        Status = 403;
        Type = "https://httpstatuses.com/403";
    }
}