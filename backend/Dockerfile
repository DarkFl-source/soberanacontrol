# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os arquivos de solução e projetos
COPY ["SoberanaControl.sln", "./"]
COPY ["SoberanaControl.Domain/SoberanaControl.Domain.csproj", "SoberanaControl.Domain/"]
COPY ["SoberanaControl.Application/SoberanaControl.Application.csproj", "SoberanaControl.Application/"]
COPY ["SoberanaControl.Infrastructure/SoberanaControl.Infrastructure.csproj", "SoberanaControl.Infrastructure/"]
COPY ["SoberanaControl.API/SoberanaControl.API.csproj", "SoberanaControl.API/"]

# Restaurar dependências
RUN dotnet restore

# Copiar o resto do código fonte
COPY . .

# Compilar e publicar a API
WORKDIR "/src/SoberanaControl.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Estágio de Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SoberanaControl.API.dll"]
