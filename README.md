# Place

## Introduction

Place is an open-source platform for managing tickets and events, developed using the Dotnet framework. It allows event organizers to sell tickets and oversee attendees without incurring service fees from third-party ticketing providers.

Using Place, you can explore popular events nearby, receive tailored event suggestions, and see what events your friends are attending. 
You can also access your tickets and event details directly from your phone, tablet or pc. 
Discover new activities through your phone, from concerts and festivals to workshops, conferences, free events, and more.

Event organizers have the ability to monitor sales, view analytics, and handle attendee check-ins right from the app. 
Place provides a complete solution for event management and promotion, helping users stay informed and connected.d promotion, helping users stay informed and connected.

## Installation

### Prerequisites

For local development without docker

- [.NET 8 SDK](https://dotnet.microsoft.com/fr-fr/download/dotnet/8.0) installed
- [PostgreSQL](https://www.postgresql.org/download) Instance running


### Setup

- Clone project
```bash
git clone https://github.com/osscameroon/place-api && cd place-api
``` 
<br>

- Restore dotnet tools
```bash
dotnet tool restore
``` 
- Setup husky
```bash
dotnet husky install
``` 

- Clean, restore and build solution



Then chmod sudo chmod -R 777 ./*

```bash
dotnet nuke
``` 
NOTE: If you are on Mac OS you should run the following command instead:
```bash
chmod -R 777 ./*
``` 
```bash
sudo dotnet nuke
``` 

- Setup database

Please refer to the [documentation](docs/Documentation.md) for more information about database environment variables in appSettings.json. (Mandatory)

```bash
docker-compose -f docker/compose/databases/database-compose.yml up -d
```

- Run the project
```bash
dotnet run --project src/PlaceApi.Host
```






## Documentation
Please refer to the [documentation](docs/Documentation.md) for more information. (Mandatory)
## Contributing


Read our [contributing guidelines](CONTRIBUTING.md) to learn about our development process, how to propose bugfixes and improvements, and how to build and test your changes.

## License

Place is a free and open source project, released under the permissible [MIT license](LICENSE).