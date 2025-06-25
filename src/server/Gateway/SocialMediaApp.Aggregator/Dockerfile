#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/server/Gateway/SocialMediaApp.Aggregator/SocialMediaApp.Aggregator.csproj", "src/server/Gateway/SocialMediaApp.Aggregator/"]
COPY ["src/server/Shared/BuildingBlocks/BuildingBlocks.csproj", "src/server/Shared/BuildingBlocks/"]
RUN dotnet restore "./src/server/Gateway/SocialMediaApp.Aggregator/SocialMediaApp.Aggregator.csproj"
COPY . .
WORKDIR "/src/src/server/Gateway/SocialMediaApp.Aggregator"
RUN dotnet build "./SocialMediaApp.Aggregator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SocialMediaApp.Aggregator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialMediaApp.Aggregator.dll"]