# Użycie obrazu bazowego .NET SDK
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Kompilacja aplikacji
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ZTP_projekt.csproj", "./"]
RUN dotnet restore "ZTP_projekt.csproj"
COPY . .
RUN dotnet build "ZTP_projekt.csproj" -c Release -o /app/build

# Publikacja aplikacji
FROM build AS publish
RUN dotnet publish "ZTP_projekt.csproj" -c Release -o /app/publish

# Finalny kontener
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZTP_projekt.dll"]
