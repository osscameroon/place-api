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
- [PostgreSQL 16](https://www.postgresql.org/download) Instance running
- [Redis](https://redis.io/fr) Instance running


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

- Clean, restore and build solution
```bash
dotnet clean
dotnet restore
dotnet build
``` 

- Run the project
```bash
dotnet workload restore
dotnet run --project src/PlaceApi.Host/
```


## Documentation

## Contributing


Read our [contributing guidelines](CONTRIBUTING.md) to learn about our development process, how to propose bugfixes and improvements, and how to build and test your changes.

## License

Place is a free and open source project, released under the permissible [MIT license](LICENSE).