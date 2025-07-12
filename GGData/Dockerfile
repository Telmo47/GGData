# Usar imagem base com SDK do .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Imagem para build com SDK do .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os ficheiros do projeto e restaurar dependências
COPY GGData/*.csproj ./GGData/
RUN dotnet restore GGData/GGData.csproj

# Copiar tudo e compilar
COPY . .
WORKDIR /src/GGData
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GGData.dll"]
