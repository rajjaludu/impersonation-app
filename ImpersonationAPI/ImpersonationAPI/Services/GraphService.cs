using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Threading.Tasks;

public class GraphService
{
    private readonly GraphServiceClient _graphClient;
    private readonly TokenCredential _credential;

    public GraphService(IConfiguration configuration)
    {
        var tenantId = configuration["AzureAdB2C:TenantId"];
        var clientId = configuration["AzureAdB2C:ClientId"];
        var clientSecret = configuration["AzureAdB2C:ClientSecret"];

        _credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        _graphClient = new GraphServiceClient(_credential);
    }

    /// <summary>
    /// Retrieves an access token for Microsoft Graph API.
    /// </summary>
    public async Task<string> GetImpersonationToken()
    {
        try
        {
            var tokenRequestContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
            var accessToken = await _credential.GetTokenAsync(tokenRequestContext, default);
            return accessToken.Token;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Token Generation Failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Retrieves user details from Microsoft Graph API.
    /// </summary>
    public async Task<User> GetUserDetails(string userId)
    {
        try
        {
            var user = await _graphClient.Users[userId]
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new[] { "id", "displayName", "mail", "userPrincipalName" };
                });

            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving user details: {ex.Message}");
            return null;
        }
    }
}
