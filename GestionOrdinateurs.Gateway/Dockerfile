# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# La Gateway ne dépend pas de Shared : seul son csproj est nécessaire pour le restore
COPY ./GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj ./GestionOrdinateurs.Gateway/
RUN dotnet restore ./GestionOrdinateurs.Gateway

# Copie du reste du code source puis publication
COPY ./GestionOrdinateurs.Gateway/ ./GestionOrdinateurs.Gateway/
RUN dotnet publish ./GestionOrdinateurs.Gateway -c Release -o out --no-restore

# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENV ASPNETCORE_ENVIRONMENT=Development
# Écoute sur le même port qu'en local (launchSettings.json)
ENV ASPNETCORE_URLS=http://+:5010

EXPOSE 5010

ENTRYPOINT ["dotnet", "GestionOrdinateurs.Gateway.dll"]
