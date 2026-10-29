using AuthentificationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthentificationApi.Controller;

/// <summary>
/// Nouvel endpoint de login via Keycloak .
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(KeycloakAuthService keycloakAuthService) : ControllerBase
{
    [HttpPost("login")] // POST: api/Auth/login
    public async Task<IActionResult> Login(KeycloakLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Le login et le mot de passe sont obligatoires.");
        }

        var token = await keycloakAuthService.LoginAsync(request.Username, request.Password);
        if (token is null)
        {
            return Unauthorized("Login ou mot de passe incorrect.");
        }

        return Ok(token);
    }
}

public record KeycloakLoginRequest(string Username, string Password);
