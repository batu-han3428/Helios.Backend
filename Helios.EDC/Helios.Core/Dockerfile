#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN curl -L https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh  | sh

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY nuget.config .
COPY ["Helios.EDC/Helios.Core/Helios.Core.csproj", "Helios.EDC/Helios.Core/"]
COPY ["Helios.Common/Helios.Common/Helios.Common.csproj", "Helios.Common/Helios.Common/"]
RUN dotnet restore "./Helios.EDC/Helios.Core/./Helios.Core.csproj"
COPY . .
WORKDIR "/src/Helios.EDC/Helios.Core"
RUN dotnet build "./Helios.Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Helios.Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Helios.Core.dll"]