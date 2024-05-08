FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as build

WORKDIR /src

COPY ["src/PlaceApi.Application/PlaceApi.Application.csproj", "src/PlaceApi.Application/"]
COPY ["src/PlaceApi.Domain/PlaceApi.Domain.csproj", "src/PlaceApi.Domain/"]
COPY ["src/PlaceApi.Infrastructure/PlaceApi.Infrastructure.csproj", "src/PlaceApi.Infrastructure/"]
COPY ["src/PlaceApi.Web/PlaceApi.Web.csproj", "src/PlaceApi.Web/"]

RUN dotnet clean "src/PlaceApi.Application/PlaceApi.Application.csproj"
RUN dotnet clean "src/PlaceApi.Domain/PlaceApi.Domain.csproj"
RUN dotnet clean "src/PlaceApi.Infrastructure/PlaceApi.Infrastructure.csproj"
RUN dotnet clean "src/PlaceApi.Web/PlaceApi.Web.csproj"

RUN dotnet restore "src/PlaceApi.Web/PlaceApi.Web.csproj" -r linux-musl-x64 /p:PublishReadyToRun=true

COPY . .

RUN dotnet publish "src/PlaceApi.Web/PlaceApi.Web.csproj" -c Release -r linux-musl-x64 -o /app/publish /p:PublishTrimmed=false  /p:PublishReadyToRun=true /p:PublishSingleFile=true --self-contained true

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine as final

EXPOSE  5000

ENV ASPNETCORE_URLS=http://+:5000;

RUN  apk add curl vim

RUN adduser --disabled-password --home /app --gecos '' nonroot && chown -R nonroot /app

USER nonroot

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT [ "./PlaceApi.Web" ]