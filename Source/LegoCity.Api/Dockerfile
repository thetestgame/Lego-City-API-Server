#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LegoCity.Api/LegoCity.Api.csproj", "LegoCity.Api/"]
RUN dotnet restore "LegoCity.Api/LegoCity.Api.csproj"
COPY . .
WORKDIR "/src/LegoCity.Api"
RUN dotnet build "LegoCity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LegoCity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update
RUN apt-get install -y bluez bluetooth

ENTRYPOINT ["sh", "docker_entrypoint.sh"]