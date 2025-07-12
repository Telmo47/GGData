# Imagem base com .NET 8.0 para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Imagem com .NET 8.0 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Já estás no contexto da pasta GGData/, por isso copia apenas o .csproj diretamente
COPY *.csproj ./
RUN dotnet restore

# Copiar o resto dos ficheiros
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GGData.dll"]
