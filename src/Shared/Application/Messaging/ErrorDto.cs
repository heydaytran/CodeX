namespace Application.Messaging;

public readonly record struct ErrorDto(int Type, string Code, string Description)
{
    public static ErrorDto From(Error error) => 
        new()
        {
            Type = (int)error.Type,
            Code = error.Code,
            Description = error.Description,
        };

    public Error ToError() => 
        Error.Custom(Type, Code, Description);
}
