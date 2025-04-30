# Use a imagem oficial do .NET SDK para construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie o arquivo da solução (.sln) e os arquivos de projetos (.csproj)
COPY EsportsApi.sln .
COPY EsportsApi.API/EsportsApi.API.csproj ./EsportsApi.API/
COPY EsportsApi.Application/EsportsApi.Application.csproj ./EsportsApi.Application/

# Restaure os pacotes NuGet
RUN dotnet restore

# Copie todo o código-fonte
COPY . .

# Publique a API
WORKDIR /src/EsportsApi.API
RUN dotnet publish -c Release -o /app

# Use a imagem oficial do .NET Runtime para executar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "EsportsApi.API.dll"]