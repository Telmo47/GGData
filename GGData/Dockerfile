# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copiar o ficheiro csproj e restaurar as dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar o resto do código
COPY . ./

# Publicar a aplicação em modo Release na pasta /out
RUN dotnet publish -c Release -o out

# Etapa 2: Runtime para correr a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copiar os ficheiros publicados da etapa build
COPY --from=build /app/out ./

# Expõe a porta 5000 (ou a porta que usas na app)
EXPOSE 5000

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "GGData.dll"]
