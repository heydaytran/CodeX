namespace Authentication.Authorization;

public static class Policies
{
    public const string OnlyCustomers = "OnlyCustomers";
    public const string OnlyAdmins = "OnlyAdmins";
    public const string OnlyThirdParties = "OnlyThirdParties";
    public const string ApiKeyAndUser = "ApiKeyAndUser";
}