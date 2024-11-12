# Environnement Variables

## Table of Contents
- [Core Settings](#core-settings)
    - [Logging Configuration](#logging-configuration)
    - [Application Settings](#application-settings)
    - [Allowed Hosts](#allowed-hosts)
- [API Documentation](#api-documentation)
    - [Swagger Settings](#swagger-settings)
- [Identity & Authentication](#identity--authentication)
    - [Identity Database](#identity-database)
    - [Authentication Rules](#authentication-rules)
- [Logging System](#logging-system)
    - [Console Logging](#console-logging)
    - [File Logging](#file-logging)
    - [Seq Logging](#seq-logging)
- [API Versioning](#api-versioning)
- [Database Settings](#database-settings)
    - [Connection Configuration](#connection-configuration)
    - [Migration Options](#migration-options)
- [Security Considerations](#security-considerations)

## Core Settings

### Logging Configuration
Default logging levels for the application:
```json
{
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

| Setting | Description | Values |
|---------|-------------|--------|
| `Default` | General logging level | `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical` |
| `Microsoft.AspNetCore` | Framework logging level | `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical` |

### Application Settings
Basic application identification:
```json
{
  "app": {
    "name": "Place Profile Api"
  }
}
```

| Setting | Description |
|---------|-------------|
| `name` | Service identifier |

### Allowed Hosts
Host configuration:
```json
{
  "AllowedHosts": "*"
}
```

| Setting | Description | Values |
|---------|-------------|--------|
| `AllowedHosts` | Allowed host names | `"*"` or specific domains |

## API Documentation

### Swagger Settings
API documentation configuration:
Refer to the project [Place.Core.Swagger](../src/Common/Place.Core.Swagger/Place.Core.Swagger.csproj) for more
details.
```json
{
  "Swagger": {
    "enabled": true,
    "reDocEnabled": false,
    "name": "v1",
    "title": "Profile Api",
    "version": "v1",
    "routePrefix": "swagger",
    "includeSecurity": true,
    "apiVersions": ["1.0"]
  }
}
```

| Setting | Description | Default |
|---------|-------------|---------|
| `enabled` | Enable Swagger | `true` |
| `reDocEnabled` | Enable ReDoc UI | `false` |
| `name` | API name | `"v1"` |
| `title` | UI title | `"Profile Api"` |
| `version` | API version | `"v1"` |
| `routePrefix` | URL prefix | `"swagger"` |
| `includeSecurity` | Security documentation | `true` |
| `apiVersions` | Supported versions | `["1.0"]` |

## Identity & Authentication

### Identity Database
Identity storage configuration:
Refer to the project [Place.Core.Identity](../src/Common/Place.Core.Identity/Place.Core.Identity.csproj) for more

```json
{
  "Identity": {
    "Database": {
      "Host": "localhost",
      "Port": 5489,
      "Username": "postgres",
      "Password": "postgres",
      "Database": "PlaceApiIdentity"
    }
  }
}
```

| Setting | Purpose | Default |
|---------|---------|---------|
| `Host` | Database server | `"localhost"` |
| `Port` | PostgreSQL port | `5489` |
| `Username` | DB user | `"postgres"` |
| `Password` | DB password | `"postgres"` |
| `Database` | DB name | `"PlaceApiIdentity"` |

### Authentication Rules
Authentication configuration:
```json
{
  "Identity": {
    "Authentication": {
      "TokenExpiration": "01:00:00",
      "RequireConfirmedEmail": true,
      "RequireConfirmedAccount": false
    }
  }
}
```

| Setting | Purpose | Default |
|---------|---------|---------|
| `TokenExpiration` | JWT validity | `"01:00:00"` |
| `RequireConfirmedEmail` | Email verification | `true` |
| `RequireConfirmedAccount` | Account verification | `false` |

## Logging System
Comprehensive logging configuration:
Refer to the project [Place.Core.Logging](../src/Common/Place.Core.Logging/Place.Core.Logging.csproj) for more

```json
{
  "logger": {
    "applicationName": "profile-service",
    "excludePaths": ["/ping", "/metrics"],
    "level": "information",
    "console": {"enabled": true},
    "file": {
      "enabled": true,
      "path": "logs/logs.txt",
      "interval": "day"
    },
    "seq": {
      "enabled": true,
      "url": "http://localhost:5341",
      "token": "secret"
    }
  }
}
```

### Console Logging
| Setting | Purpose | Default |
|---------|---------|---------|
| `enabled` | Enable console output | `true` |

### File Logging
| Setting | Purpose | Default |
|---------|---------|---------|
| `enabled` | Enable file output | `true` |
| `path` | Log file location | `"logs/logs.txt"` |
| `interval` | Rotation period | `"day"` |

### Seq Logging
| Setting | Purpose | Default |
|---------|---------|---------|
| `enabled` | Enable Seq | `true` |
| `url` | Seq server URL | `"http://localhost:5341"` |
| `token` | API key | `"secret"` |

## API Versioning
Version control configuration:
Refer to the project [Place.Core.Versioning](../src/Common/Place.Core.Versioning/Place.Core.Versioning.csproj) for more

```json
{
  "apiVersioning": {
    "enabled": true,
    "defaultVersion": "1.0",
    "assumeDefaultVersionWhenUnspecified": true,
    "reportApiVersions": true,
    "versionReaderType": "All",
    "headerName": "X-Api-Version",
    "queryStringParam": "api-version"
  }
}
```

| Setting | Purpose | Default |
|---------|---------|---------|
| `enabled` | Enable versioning | `true` |
| `defaultVersion` | Fallback version | `"1.0"` |
| `assumeDefaultVersionWhenUnspecified` | Use default if unspecified | `true` |
| `reportApiVersions` | Show available versions | `true` |
| `versionReaderType` | Version source | `"All"` |
| `headerName` | Header field name | `"X-Api-Version"` |
| `queryStringParam` | Query parameter name | `"api-version"` |

## Database Settings

### Connection Configuration
Main database settings:
Refer to the project [Place.Core.Database](../src/Common/Place.Core.Database/Place.Core.Database.csproj) for more

```json
{
  "Database": {
    "Provider": "Postgres",
    "Connection": {
      "Host": "localhost",
      "Port": 5489,
      "Database": "PlaceApiProfile",
      "Username": "postgres",
      "Password": "postgres",
      "CommandTimeout": 30,
      "EnableRetryOnFailure": true,
      "MaxRetryCount": 3,
      "AdditionalParameters": {
        "Pooling": "true",
        "Maximum Pool Size": "100"
      }
    }
  }
}
```

| Setting | Purpose | Default |
|---------|---------|---------|
| `Provider` | Database type | `"Postgres"` |
| `Host` | Server address | `"localhost"` |
| `Port` | Server port | `5489` |
| `Database` | Database name | `"PlaceApiProfile"` |
| `CommandTimeout` | Query timeout (seconds) | `30` |
| `EnableRetryOnFailure` | Retry failed connections | `true` |
| `MaxRetryCount` | Maximum retries | `3` |
| `Pooling` | Connection pooling | `"true"` |
| `Maximum Pool Size` | Max connections | `"100"` |

### Migration Options
Database migration configuration:
```json
{
  "Database": {
    "Migration": {
      "AutoMigrate": true,
      "SeedData": true,
      "MigrationsHistoryTableSchema": "public",
      "IdempotentMigrations": true
    }
  }
}
```

| Setting | Purpose | Default |
|---------|---------|---------|
| `AutoMigrate` | Auto-run migrations | `true` |
| `SeedData` | Initialize data | `true` |
| `MigrationsHistoryTableSchema` | History table schema | `"public"` |
| `IdempotentMigrations` | Safe reruns | `true` |

## Security Considerations

### Environment Variables
For production, replace sensitive data:
```json
{
  "Database": {
    "Connection": {
      "Password": "${DB_PASSWORD}"
    }
  }
}
```

### Environment-Specific Files
Override settings per environment:
- `appsettings.Development.json`
- `appsettings.Staging.json`
- `appsettings.Production.json`

### Production Guidelines
1. **Security**
    - Use environment variables
    - Secure database credentials
    - Appropriate token timeouts

2. **Performance**
    - Configure pool sizes
    - Set appropriate timeouts
    - Optimize logging levels

3. **Monitoring**
    - Enable necessary metrics
    - Configure proper logging
    - Set up health checks