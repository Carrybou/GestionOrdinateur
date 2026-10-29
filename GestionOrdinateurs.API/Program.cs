using System.Text;
using GestionOrdinateurs.API.Context;
using GestionOrdinateurs.API.Service;
using GestionOrdinateurs.Shared.Enums;
using GestionOrdinateurs.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
    });
});
builder.Services.AddDbContext<ParcInformatiqueContext>();
builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();
builder.Services.AddScoped<ReparationService>();
builder.Services.AddScoped<JournalService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "GestionOrdinateurs",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Entrez votre token JWT."
    });

});
builder.Services.AddOpenApi();

// Ancienne validation (TP5) : token signé par AuthentificationApi avec la clé Jwt:Key
// var jwtKey = builder.Configuration["Jwt:Key"]
//     ?? throw new InvalidOperationException("La clé Jwt:Key est absente de la configuration.");
// var key = Encoding.ASCII.GetBytes(jwtKey);
//
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(key),
//         ValidateIssuer = false,
//         ValidateAudience = false
//     };
// });

// Nouvelle validation (TP6) : token délivré par Keycloak
// URL interne pour joindre Keycloak (localhost:8180 en local, keycloak:8080 sous Docker)
var keycloakInternalUrl = builder.Configuration["Keycloak:InternalUrl"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Le token doit avoir été émis par notre realm Keycloak
        ValidateIssuer = true,
        ValidIssuer = $"{builder.Configuration["Keycloak:BaseUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}",

        ValidateAudience = true,
        ValidAudience = "account",

        ValidateIssuerSigningKey = true,
        ValidateLifetime = false,

        // Récupère les clés publiques de Keycloak pour vérifier la signature du token
        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
        {
            var client = new HttpClient();
            var keyUri = $"{keycloakInternalUrl}/realms/{builder.Configuration["Keycloak:Realm"]}/protocol/openid-connect/certs";
            var response = client.GetAsync(keyUri).Result;
            var keys = new JsonWebKeySet(response.Content.ReadAsStringAsync().Result);

            return keys.GetSigningKeys();
        }
    };

    options.RequireHttpsMetadata = false; // Uniquement en développement
    options.SaveToken = true;
});
builder.Services.AddAuthorization();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ParcInformatiqueContext>();
    context.Database.Migrate();
    DbInitializer.Initialize(context);

    if (!context.Users.Any())
    {
        context.Users.AddRange(
            new User { Login = "user1", Role = RoleEnum.Admin },
            new User { Login = "user2", Role = RoleEnum.Gestionnaire },
            new User { Login = "user3", Role = RoleEnum.Gestionnaire });

        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();