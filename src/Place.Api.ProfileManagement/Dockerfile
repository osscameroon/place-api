FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Place.Api.ProfileManagement/Place.Api.ProfileManagement.csproj", "src/Place.Api.ProfileManagement/"]
RUN dotnet restore "src/Place.Api.ProfileManagement/Place.Api.ProfileManagement.csproj"
COPY . .
WORKDIR "/src/src/Place.Api.ProfileManagement"
RUN dotnet build "Place.Api.ProfileManagement.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Place.Api.ProfileManagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Place.Api.ProfileManagement.dll"]
