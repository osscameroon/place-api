FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS base
WORKDIR /app
EXPOSE 5000

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG TARGETARCH
ARG BUILDPLATFORM
ARG RUNTIME

COPY src/. ./src
COPY Directory.Build.props ./src
COPY Directory.Packages.props ./src
COPY nuget.config ./src

WORKDIR /src/Place.API

RUN ls

RUN dotnet restore "Place.API.csproj" -a $TARGETARCH

RUN dotnet build "Place.API.csproj" -c Release -o /app/build -a $TARGETARCH

FROM build AS publish
RUN dotnet publish "Place.API.csproj" -c Release -o /app/publish \
    #--runtime alpine-x64 \
    --self-contained true \
    /p:PublishTrimmed=false \
    /p:PublishSingleFile=true \
    -a $TARGETARCH 


FROM --platform=$BUILDPLATFORM base AS final
ARG TARGETARCH
ARG BUILDPLATFORM

RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app

USER dotnetuser

WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["./Place.API"]