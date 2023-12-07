



#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["admin_tool_api_ui/admin_tool_api_ui.csproj", "admin_tool_api_ui/"]
RUN dotnet restore "admin_tool_api_ui/admin_tool_api_ui.csproj"
COPY . .
WORKDIR "/src/admin_tool_api_ui"
RUN dotnet build "admin_tool_api_ui.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "admin_tool_api_ui.csproj" -c Release -o /app/publish


FROM node:16 AS build-web
COPY ./admin_tool_api_ui/ClientApp/package.json /admin_tool_api_ui/ClientApp/package.json
COPY ./admin_tool_api_ui/ClientApp/package-lock.json /admin_tool_api_ui/ClientApp/package-lock.json
WORKDIR /admin_tool_api_ui/ClientApp
RUN npm ci
COPY ./admin_tool_api_ui/ClientApp/ /admin_tool_api_ui/ClientApp
RUN npm run build




FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build-web /admin_tool_api_ui/ClientApp/build ./ClientApp/build
ENTRYPOINT ["dotnet", "admin_tool_api_ui.dll"]