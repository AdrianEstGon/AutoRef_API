# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copiar todo el proyecto (ya estamos dentro del contexto correcto)
COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o out

# Etapa runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

USER 10001

# Puerto que tu API usa (ajústalo si es diferente)
EXPOSE 7187

# Comando de inicio
ENTRYPOINT ["dotnet", "AutoRef_API.dll"]
