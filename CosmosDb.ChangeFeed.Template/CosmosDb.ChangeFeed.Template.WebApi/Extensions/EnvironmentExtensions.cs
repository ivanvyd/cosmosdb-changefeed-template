namespace CosmosDb.ChangeFeed.Template.WebApi.Extensions;

public static class EnvironmentExtensions
{
    public static bool IsLocal(this IWebHostEnvironment environment)
    {
        return environment.IsEnvironment("Local");
    }
}
