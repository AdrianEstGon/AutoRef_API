# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

# Etapa final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Crear un usuario no-root (por ejemplo, UID 10014)
RUN adduser --uid 10014 --disabled-password --gecos "" appuser
COPY --from=build /app/out .

# Usar ese usuario
USER appuser

# Puerto que tu API usa (ajústalo si es diferente)
EXPOSE 7187

# Comando de inicio
ENTRYPOINT ["dotnet", "AutoRef_API.dll"]
