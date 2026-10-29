using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace GestionOrdinateurs.Client.Services;

public class JwtAuthenticationStateProvider(
    ISessionStorageService sessionStorage) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await sessionStorage.GetItemAsync<string>("jwtToken");
        return CreateAuthenticationState(token);
    }

    public void NotifyUserAuthentication(string token)
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(CreateAuthenticationState(token)));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(CreateAuthenticationState(null)));
    }

    private static AuthenticationState CreateAuthenticationState(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new JwtSecurityTokenHandler()
            .ReadJwtToken(token)
            .Claims;
        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
