FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ARG RUNTIME

RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app

USER dotnetuser
WORKDIR /app

COPY /publish/${RUNTIME}/* .
ENTRYPOINT ["./Place.API"]