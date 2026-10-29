using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthentificationApi.Services;

/// <summary>
/// Demande un token à Keycloak avec le login / mot de passe de l'utilisateur (grant_type=password).
/// </summary>
public class KeycloakAuthService(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<TokenResponse?> LoginAsync(string username, string password)
    {
        // Endpoint OIDC de Keycloak qui délivre les tokens
        var tokenEndpoint = $"{configuration["Keycloak:BaseUrl"]}/realms/{configuration["Keycloak:Realm"]}/protocol/openid-connect/token";

        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", configuration["Keycloak:ClientId"]! },
            { "client_secret", configuration["Keycloak:ClientSecret"]! }, // fourni par .env (Docker) ou user-secrets (local)
            { "username", username },
            { "password", password }
        };

        using var response = await httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));

        // Mauvais identifiants (401) ou client mal configuré : pas de token
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(responseContent);
    }
}

/// <summary>
/// Réponse du token endpoint de Keycloak 
/// </summary>
public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}
