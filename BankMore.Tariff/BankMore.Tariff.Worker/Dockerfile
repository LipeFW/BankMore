# ==========================
# 🏗️ Stage 1 - Build
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os arquivos de projeto primeiro para otimizar cache
COPY ["BankMore.Tariff/BankMore.Tariff.Worker/BankMore.Tariff.Worker.csproj", "BankMore.Tariff/BankMore.Tariff.Worker/"]
COPY ["BankMore.Tariff/BankMore.Tariff.Application/BankMore.Tariff.Application.csproj", "BankMore.Tariff/BankMore.Tariff.Application/"]
COPY ["BankMore.Tariff/BankMore.Tariff.Domain/BankMore.Tariff.Domain.csproj", "BankMore.Tariff/BankMore.Tariff.Domain/"]
COPY ["BankMore.Tariff/BankMore.Tariff.Infrastructure/BankMore.Tariff.Infrastructure.csproj", "BankMore.Tariff/BankMore.Tariff.Infrastructure/"]

# Restaura dependências
RUN dotnet restore "BankMore.Tariff/BankMore.Tariff.Worker/BankMore.Tariff.Worker.csproj"

# Copia todo o restante do código-fonte
COPY . .

# Compila a aplicação
WORKDIR "/src/BankMore.Tariff/BankMore.Tariff.Worker"
RUN dotnet build "BankMore.Tariff.Worker.csproj" -c Release -o /app/build

# ==========================
# 🚀 Stage 2 - Publish
# ==========================
FROM build AS publish
RUN dotnet publish "BankMore.Tariff.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================
# 🌐 Stage 3 - Runtime
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Copia a aplicação publicada
COPY --from=publish /app/publish .

# EntryPoint
ENTRYPOINT ["dotnet", "BankMore.Tariff.Worker.dll"]
