namespace WebApi.Utilities;

public static class LandingPageHtml
{
    public static string GetContent()
    {
        return @"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Welcome to the API</title>
            <style>
                body { font-family: Arial, sans-serif; text-align: center; margin: 50px; }
                a { color: #007bff; text-decoration: none; font-weight: bold; }
            </style>
        </head>
        <body>
            <h1>Welcome to the .NET Core REST API!</h1>
            <p>Explore the API documentation:</p>
            <p><a href='/swagger' target='_blank'>Go to Swagger UI</a></p>
        </body>
        </html>
        ";
    }
}
