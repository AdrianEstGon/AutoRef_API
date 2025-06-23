# Imagen base para build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copia archivos y restaura dependencias
COPY *.csproj .
RUN dotnet restore

# Copia el resto del código
COPY . .
RUN dotnet publish -c Release -o out

# Imagen base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Puerto que tu API usa (ajústalo si es diferente)
EXPOSE 5000

# Comando de inicio
ENTRYPOINT ["dotnet", "AutoRef_API.dll"]
