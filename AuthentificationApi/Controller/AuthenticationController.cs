using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionOrdinateurs.Shared.Enums;
using GestionOrdinateurs.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthentificationApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost] // POST: api/authenticate
    public async Task<IActionResult> Authenticate(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login)) // Vérification si le login est vide ou null
        {
            return BadRequest("Le login est obligatoire.");
        }

        var client = httpClientFactory.CreateClient("GestionOrdinateursApi");
        using var response = await client.GetAsync(
            $"api/users/{Uri.EscapeDataString(request.Login)}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound("L'utilisateur n'existe pas.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Impossible de récupérer l'utilisateur.");
        }

        var user = await response.Content.ReadFromJsonAsync<User>();
        if (user is null)
        {
            return Problem("La réponse de l'API utilisateurs est invalide.");
        }

        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("La clé Jwt:Key est absente de la configuration.");
        var key = Encoding.ASCII.GetBytes(jwtKey);

        return Ok(GenTokenJWT(key, user.Role, user.Login));
    }

    /// <summary>
    /// Génère le token JWT après vérification de l'utilisateur.
    /// </summary>
    private static string GenTokenJWT(byte[] keyP, RoleEnum role, string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userId),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor 
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyP),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public record LoginRequest(string Login);
