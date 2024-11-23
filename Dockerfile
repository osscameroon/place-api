FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443



# Final image

# create a new user and change directory ownership
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app

# impersonate into the new user
USER dotnetuser
WORKDIR /app

COPY /publish .
ENTRYPOINT ["./Place.API"]