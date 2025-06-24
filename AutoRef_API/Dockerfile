# Etapa de build con .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Etapa de runtime con .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

USER 10001

# Puerto que tu API usa (ajústalo si es diferente)
EXPOSE 8080

# Comando de inicio
ENTRYPOINT ["dotnet", "AutoRef_API.dll"]
