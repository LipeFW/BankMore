# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.
# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

#ENV ASPNETCORE_URLS=http://+:80

# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BankMore.Account/BankMore.Account.Api/BankMore.Account.Api.csproj", "BankMore.Account/BankMore.Account.Api/"]
RUN dotnet restore "./BankMore.Account/BankMore.Account.Api/BankMore.Account.Api.csproj"
COPY . .
WORKDIR "/src/BankMore.Account/BankMore.Account.Api"
RUN dotnet build "./BankMore.Account.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN mkdir -p /app/data
RUN chmod 777 /app/data

VOLUME ["/app/data"]

ENV SQLITE_DB_PATH=/app/data/app.db

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankMore.Account.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Account.Api.dll"]