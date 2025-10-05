# ==========================
# 🏗️  Stage 1 - Build
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto primeiro para aproveitar cache do Docker
COPY ["BankMore.Transfer/BankMore.Transfer.Api/BankMore.Transfer.Api.csproj", "BankMore.Transfer/BankMore.Transfer.Api/"]
COPY ["BankMore.Transfer/BankMore.Transfer.Application/BankMore.Transfer.Application.csproj", "BankMore.Transfer/BankMore.Transfer.Application/"]
COPY ["BankMore.Transfer/BankMore.Transfer.Domain/BankMore.Transfer.Domain.csproj", "BankMore.Transfer/BankMore.Transfer.Domain/"]
COPY ["BankMore.Transfer/BankMore.Transfer.Infrastructure/BankMore.Transfer.Infrastructure.csproj", "BankMore.Transfer/BankMore.Transfer.Infrastructure/"]

# Restaura dependências (otimiza cache)
RUN dotnet restore "BankMore.Transfer/BankMore.Transfer.Api/BankMore.Transfer.Api.csproj"

# Copia o restante do código-fonte
COPY . .

# Compila a aplicação em modo Release
WORKDIR "/src/BankMore.Transfer/BankMore.Transfer.Api"
RUN dotnet build "BankMore.Transfer.Api.csproj" -c Release -o /app/build

# ==========================
# 🚀  Stage 2 - Publish
# ==========================
FROM build AS publish
RUN dotnet publish "BankMore.Transfer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================
# 🌐  Stage 3 - Runtime
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .

# EntryPoint
ENTRYPOINT ["dotnet", "BankMore.Transfer.Api.dll"]