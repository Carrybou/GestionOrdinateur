# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copie des csproj seuls puis restore (couche mise en cache tant que les dépendances ne changent pas)
COPY ./GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj ./GestionOrdinateurs.Client/
COPY ./GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj ./GestionOrdinateurs.Shared/
RUN dotnet restore ./GestionOrdinateurs.Client

# Copie du reste du code source puis publication
COPY ./GestionOrdinateurs.Client/ ./GestionOrdinateurs.Client/
COPY ./GestionOrdinateurs.Shared/ ./GestionOrdinateurs.Shared/
RUN dotnet publish ./GestionOrdinateurs.Client -c Release -o out --no-restore

# =========================
# Runtime : Blazor WebAssembly = fichiers statiques, servis par nginx (pas besoin du runtime .NET)
# =========================
FROM nginx:alpine AS runtime

# Le site publié se trouve dans le dossier wwwroot de la sortie
COPY --from=build /app/out/wwwroot /usr/share/nginx/html

# Configuration nginx : port 5000 + redirection des routes vers index.html
COPY ./GestionOrdinateurs.Client/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 5000

CMD ["nginx", "-g", "daemon off;"]
