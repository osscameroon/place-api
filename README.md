# Place

## Introduction

Place is an open-source platform for managing tickets and events, developed using the Dotnet framework. It allows event organizers to sell tickets and oversee attendees without incurring service fees from third-party ticketing providers.

Using Place, you can explore popular events nearby, receive tailored event suggestions, and see what events your friends are attending.
You can also access your tickets and event details directly from your phone, tablet or pc.
Discover new activities through your phone, from concerts and festivals to workshops, conferences, free events, and more.

Event organizers have the ability to monitor sales, view analytics, and handle attendee check-ins right from the app.
Place provides a complete solution for event management and promotion, helping users stay informed and connected.

## Installation

### Prerequisites

For local development you will need:

- [.NET 9 SDK](https://dotnet.microsoft.com/fr-fr/download/dotnet/9.0)
- [Docker](https://www.docker.com/products/docker-desktop/) for running databases and containerization
- [Make](https://www.gnu.org/software/make/) for running development commands

### Setup

1. Clone the project
```bash
git clone https://github.com/osscameroon/place-api && cd place-api
```

2. Restore dotnet tools
```bash
dotnet tool restore
```

3. Setup husky
```bash
dotnet husky install
```

4. Start the development environment
```bash
# Start databases and build the project
make dev-env

# Or start components separately:
make db-up      # Start databases only
make restore    # Restore NuGet packages
make build      # Build the solution
```

5. Run tests
```bash
# Run all tests
make test

# Or run specific test types
make test-unit          # Run unit tests
make test-integration   # Run integration tests
```

6. Build and run the application
```bash
# Quick build without tests
make dev

# Or full build with tests
make all
```

### Available Make Commands

#### Database Management
```bash
make db-up        # Start development databases
make db-down      # Stop databases
make db-restart   # Restart databases
make db-logs      # Display database logs
make db-clean     # Remove volumes and restart databases
```

#### Build Commands
```bash
make restore      # Restore NuGet packages
make build        # Build solution
make test         # Run all tests
make publish      # Publish application
make docker       # Build Docker image
make all          # Execute restore, build, test, and docker
make clean        # Clean build files
make dev          # Quick build without tests
```

#### Development Environment
```bash
make dev-env      # Setup complete dev environment (DBs + build)
make dev-start    # Start dev environment
make dev-stop     # Stop dev environment
```

For more details about available commands, run:
```bash
make help
```

## Documentation
Please refer to the [documentation](docs/Documentation.md) for more information.

## Contributing
Read our [contributing guidelines](CONTRIBUTING.md) to learn about our development process, how to propose bugfixes and improvements, and how to build and test your changes.

## License
Place is a free and open source project, released under the permissible [MIT license](LICENSE).
