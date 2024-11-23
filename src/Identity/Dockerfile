FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/PlaceAPi.Identity/PlaceAPi.Identity.csproj", "src/PlaceAPi.Identity/"]
RUN dotnet restore "src/PlaceAPi.Identity/PlaceAPi.Identity.csproj"
COPY . .
WORKDIR "/src/src/PlaceAPi.Identity"
RUN dotnet build "PlaceAPi.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlaceAPi.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlaceAPi.Identity.dll"]
