# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BankMore.Tariff/BankMore.Tariff.Worker/BankMore.Tariff.Worker.csproj", "BankMore.Tariff/BankMore.Tariff.Worker/"]
RUN dotnet restore "./BankMore.Tariff/BankMore.Tariff.Worker/BankMore.Tariff.Worker.csproj"
COPY . .
WORKDIR "/src/BankMore.Tariff/BankMore.Tariff.Worker"
RUN dotnet build "./BankMore.Tariff.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankMore.Tariff.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Tariff.Worker.dll"]