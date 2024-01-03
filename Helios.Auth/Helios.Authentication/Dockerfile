#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Helios.Auth/Helios.Authentication/Helios.Authentication.csproj", "Helios.Auth/Helios.Authentication/"]
COPY ["Helios.EDC/Helios.Common/Helios.Common.csproj", "Helios.EDC/Helios.Common/"]
COPY ["Helios.Auth/Helios.EventBus.Base/Helios.EventBus.Base.csproj", "Helios.Auth/Helios.EventBus.Base/"]
RUN dotnet restore "./Helios.Auth/Helios.Authentication/./Helios.Authentication.csproj"
COPY . .
WORKDIR "/src/Helios.Auth/Helios.Authentication"
RUN dotnet build "./Helios.Authentication.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Helios.Authentication.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Helios.Authentication.dll"]