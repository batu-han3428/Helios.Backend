#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Helios.EDC/Helios.eCRF/Helios.eCRF.csproj", "Helios.EDC/Helios.eCRF/"]
COPY ["Helios.EDC/Helios.Common/Helios.Common.csproj", "Helios.EDC/Helios.Common/"]
COPY ["Helios.EDC/Helios.Core/Helios.Core.csproj", "Helios.EDC/Helios.Core/"]
RUN dotnet restore "./Helios.EDC/Helios.eCRF/./Helios.eCRF.csproj"
COPY . .
WORKDIR "/src/Helios.EDC/Helios.eCRF"
RUN dotnet build "./Helios.eCRF.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Helios.eCRF.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Helios.eCRF.dll"]