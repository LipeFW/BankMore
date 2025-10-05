# ==========================
# 🏗️  Stage 1 - Build
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto primeiro para aproveitar cache do Docker
COPY ["BankMore.Account/BankMore.Account.Api/BankMore.Account.Api.csproj", "BankMore.Account/BankMore.Account.Api/"]
COPY ["BankMore.Account/BankMore.Account.Application/BankMore.Account.Application.csproj", "BankMore.Account/BankMore.Account.Application/"]
COPY ["BankMore.Account/BankMore.Account.Domain/BankMore.Account.Domain.csproj", "BankMore.Account/BankMore.Account.Domain/"]
COPY ["BankMore.Account/BankMore.Account.Infrastructure/BankMore.Account.Infrastructure.csproj", "BankMore.Account/BankMore.Account.Infrastructure/"]

# Restaura dependências (otimiza cache)
RUN dotnet restore "BankMore.Account/BankMore.Account.Api/BankMore.Account.Api.csproj"

# Copia o restante do código-fonte
COPY . .

# Compila a aplicação em modo Release
WORKDIR "/src/BankMore.Account/BankMore.Account.Api"
RUN dotnet build "BankMore.Account.Api.csproj" -c Release -o /app/build

# ==========================
# 🚀  Stage 2 - Publish
# ==========================
FROM build AS publish
RUN dotnet publish "BankMore.Account.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================
# 🌐  Stage 3 - Runtime
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .

# EntryPoint
ENTRYPOINT ["dotnet", "BankMore.Account.Api.dll"]