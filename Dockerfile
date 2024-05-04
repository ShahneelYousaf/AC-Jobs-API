#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AC-Jobs-API/AC-Jobs-API.csproj", "AC-Jobs-API/"]
COPY ["AC-Jobs-API-Service-Layer/AC-Jobs-API-Service-Layer.csproj", "AC-Jobs-API-Service-Layer/"]
COPY ["AC-Jobs-API-Repository-Layer/AC-Jobs-API-Repository-Layer.csproj", "AC-Jobs-API-Repository-Layer/"]
COPY ["AC-Jobs-API-Domian-Layer/AC-Jobs-API-Domian-Layer.csproj", "AC-Jobs-API-Domian-Layer/"]
RUN dotnet restore "AC-Jobs-API/AC-Jobs-API.csproj"
COPY . .
WORKDIR "/src/AC-Jobs-API"
RUN dotnet build "AC-Jobs-API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AC-Jobs-API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AC-Jobs-API.dll"]