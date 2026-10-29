using System.Text;
using AuthentificationApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        // 5150/7040 : client en local (dotnet run), 5000 : client sous Docker (nginx)
        policy.WithOrigins("http://localhost:5150", "https://localhost:7040", "http://localhost:5000", "http://localhost:5001")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });// Configuratioon du CORS pour le Blazor client
});
// URL de l'API métier : localhost en local, surchargée par docker-compose (http://gestionordinateurs-api:5087/)
var apiUrl = builder.Configuration["Services:GestionOrdinateursApi"] ?? "http://localhost:5087/";
builder.Services.AddHttpClient("GestionOrdinateursApi", client =>
{
    client.BaseAddress = new Uri(apiUrl);
});
// Service qui demande les tokens à Keycloak (HttpClient injecté)
builder.Services.AddHttpClient<KeycloakAuthService>();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
// Configure JWT authentication 
var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? throw new InvalidOperationException("La clé Jwt:Key est absente de la configuration.");
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();// authentication middleware
app.UseAuthorization();
app.MapControllers();

app.Run();
