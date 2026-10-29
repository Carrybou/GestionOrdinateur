var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("AllowBlazorClient");
app.MapReverseProxy();

app.Run();
