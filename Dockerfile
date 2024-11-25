FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS base
WORKDIR /app
EXPOSE 5000

ARG RUNTIME

RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app

USER dotnetuser
WORKDIR /app

COPY /publish/${RUNTIME}/* .
RUN rm appsettings.*.json
ENTRYPOINT ["./Place.API"]