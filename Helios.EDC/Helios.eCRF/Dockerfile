#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Helios.EDC/Helios.eCRF/Helios.eCRF.csproj", "Helios.EDC/Helios.eCRF/"]
RUN dotnet restore "Helios.EDC/Helios.eCRF/Helios.eCRF.csproj"
COPY . .
WORKDIR "/src/Helios.EDC/Helios.eCRF"
RUN dotnet build "Helios.eCRF.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Helios.eCRF.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Helios.eCRF.dll"]