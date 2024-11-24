APP_NAME := place.api
PUBLISH_OUTPUT := publish
COMPOSE_DB_FILE := docker/compose/databases/database-compose.yml
DOCKER_REPO := genjirusuchiwa/place
SOLUTION := Place.sln
API_PROJECT := src/Place.API/Place.API.csproj

ARCH := $(shell uname -m)
OS := $(shell uname -s)

ifeq ($(OS),Darwin)
    ifeq ($(ARCH),arm64)
        RUNTIME := osx-arm64
        DOCKER_ARCH := arm64
    else
        RUNTIME := osx-x64
        DOCKER_ARCH := x64
    endif
else
    ifeq ($(ARCH),x86_64)
        RUNTIME := linux-x64
        DOCKER_ARCH := x64
    else ifeq ($(ARCH),aarch64)
        RUNTIME := linux-arm64
        DOCKER_ARCH := arm64
    else
        RUNTIME := linux-x64
        DOCKER_ARCH := x64
    endif
endif

GREEN := \033[0;32m
RED := \033[0;31m
YELLOW := \033[0;33m
BLUE := \033[0;34m
NC := \033[0m


help:
	@echo "Usage:"
	@echo ""
	@echo "Development Database Commands:"
	@echo "  make db-up               - Start development databases"
	@echo "  make db-down             - Stop databases"
	@echo "  make db-restart          - Restart databases"
	@echo "  make db-logs             - Display database logs"
	@echo "  make db-clean            - Remove volumes and restart databases"
	@echo ""
	@echo "Build Commands:"
	@echo "  make restore             - Restore NuGet packages"
	@echo "  make build               - Build solution"
	@echo "  make test                - Run all tests"
	@echo "  make test-unit           - Run unit tests"
	@echo "  make test-integration    - Run integration tests"
	@echo "  make publish             - Publish application"
	@echo "  make docker              - Build Docker image"
	@echo "  make all                 - Execute restore, build, test, and docker"
	@echo "  make clean               - Clean build files"
	@echo "  make dev                 - Quick build without tests"
	@echo ""
	@echo "Full Development Setup:"
	@echo "  make dev-env             - Setup complete dev environment (DBs + build)"
	@echo "  make dev-start           - Start dev environment"
	@echo "  make dev-stop            - Stop dev environment"
	@echo ""
	@echo "Options:"
	@echo "  RUNTIME=xxx             - Specify runtime (linux-musl-x64, linux-musl-arm64)"
	@echo ""
	@echo "Detected architecture: $(ARCH)"
	@echo "Default runtime: $(RUNTIME)"

# Database commands
db-up:
	@echo "$(BLUE)🚀  Starting databases...$(NC)"
	docker-compose -f $(COMPOSE_DB_FILE) up -d
	@echo "$(GREEN)✅  Databases started$(NC)"

db-down:
	@echo "$(BLUE)🛑  Stopping databases...$(NC)"
	docker-compose -f $(COMPOSE_DB_FILE) down
	@echo "$(GREEN)✅  Databases stopped$(NC)"

db-restart: db-down db-up
	@echo "$(GREEN)✅  Databases restarted$(NC)"

db-logs:
	@echo "$(BLUE)📋  Database logs:$(NC)"
	docker-compose -f $(COMPOSE_DB_FILE) logs -f

db-clean:
	@echo "$(BLUE)🧹  Complete database cleanup...$(NC)"
	docker-compose -f $(COMPOSE_DB_FILE) down -v
	@echo "$(YELLOW)🚀  Restarting clean databases...$(NC)"
	docker-compose -f $(COMPOSE_DB_FILE) up -d
	@echo "$(GREEN)✅  Databases reset$(NC)"


clean:
	@echo "$(YELLOW)📦 Clean the solution...$(NC)"
	dotnet clean
	@echo "$(GREEN)✅  Solution cleaned$(NC)"
 
restore: clean
	@echo "$(YELLOW)📦 Restoring NuGet packages...$(NC)"
	@echo "$(BLUE)🔄  Restoring runtime-specific dependencies for $(RUNTIME)...$(NC)"
	dotnet restore $(SOLUTION) -r $(RUNTIME)
	@echo "$(GREEN)✅  Restore completed$(NC)"

build: restore
	@echo "$(YELLOW)🔨 Building solution...$(NC)"
	dotnet build $(SOLUTION) -c Release --no-restore
	@echo "$(GREEN)✅  Build completed$(NC)"

test-unit: build
	@echo "$(YELLOW)🧪  Running unit tests...$(NC)"
	@dotnet test $(SOLUTION) \
		--configuration Release \
		--no-build \
		--filter "Category=Unit" \
		--verbosity minimal \
		--logger "trx;LogFileName=unit_tests.trx" && \
		echo "$(GREEN)✅  Unit tests completed$(NC)" || \
		(echo "$(RED)❌  Unit tests failed$(NC)" && exit 1)

test-integration: build
	@echo "$(YELLOW)🧪 Running integration tests...$(NC)"
	@dotnet test $(SOLUTION) \
		--configuration Release \
		--no-build \
		--filter "Category=Integration" \
		--verbosity minimal \
		--logger "trx;LogFileName=integration_tests.trx" && \
		echo "$(GREEN)✅  Integration tests completed$(NC)" || \
		(echo "$(RED)❌  Integration tests failed$(NC)" && exit 1)

test: build
	@echo "$(YELLOW)🧪 Running all tests...$(NC)"
	@dotnet test $(SOLUTION) \
		--configuration Release \
		--no-build \
		--verbosity minimal \
		--logger "trx;LogFileName=all_tests.trx" && \
		echo "$(GREEN)✅  All tests completed$(NC)" || \
		(echo "$(RED)❌  Tests failed$(NC)" && exit 1)

# Function to run specific test project
test-project:
	@if [ -z "$(PROJECT)" ]; then \
		echo "$(RED)❌  Please specify a project with PROJECT=path/to/test/project.csproj$(NC)"; \
		exit 1; \
	fi
	@echo "$(YELLOW)🧪 Running tests for $(PROJECT)...$(NC)"
	@dotnet test $(PROJECT) \
		--configuration Release \
		--verbosity normal \
		--logger "console;verbosity=detailed" && \
		echo "$(GREEN)✅  Project tests completed$(NC)" || \
		(echo "$(RED)❌  Project tests failed$(NC)" && exit 1)

# Debug test command
test-debug:
	@echo "$(YELLOW)🔍  Running tests with verbose output...$(NC)"
	@dotnet test $(SOLUTION) \
		--configuration Release \
		--verbosity detailed \
		--logger "console;verbosity=detailed" && \
		echo "$(GREEN)✅  Debug tests completed$(NC)" || \
		(echo "$(RED)❌  Debug tests failed$(NC)" && exit 1)

publish: build
	@echo "$(YELLOW)📦  Publishing $(APP_NAME) for $(RUNTIME)...$(NC)"
	@echo "$(BLUE)🚀  Building single file application...$(NC)"
	dotnet publish $(API_PROJECT) \
		-c Release \
		-o $(PUBLISH_OUTPUT)/$(RUNTIME) \
		--runtime $(RUNTIME) \
		--self-contained true \
		/p:PublishTrimmed=true \
		/p:PublishSingleFile=true \
		/p:GenerateStaticWebAssetsManifest=false
	@echo "$(GREEN)✅  Publish completed$(NC)"

docker:
	@echo "$(YELLOW)🐳 Building Docker image for $(RUNTIME)...$(NC)"
	docker build -f Dockerfile \
		--build-arg RUNTIME=$(RUNTIME) \
		-t $(DOCKER_REPO):$(VERSION)-$(DOCKER_ARCH) .
	@echo "$(GREEN)✅  Docker build completed$(NC)"
	@echo "Tagged as: $(DOCKER_REPO):$(VERSION)-$(DOCKER_ARCH)"


dev-env: db-up restore build
	@echo "$(GREEN)✅  Development environment ready$(NC)"

dev-start: db-up
	@echo "$(GREEN)✅  Development environment started$(NC)"

dev-stop: db-down
	@echo "$(GREEN)✅  Development environment stopped$(NC)"

clean:
	@echo "$(YELLOW)🧹 Cleaning...$(NC)"
	dotnet clean $(SOLUTION)
	rm -rf $(PUBLISH_OUTPUT)
	@echo "$(GREEN)✅  Clean completed$(NC)"

all: restore build test publish docker
	@echo "$(GREEN)✅  All tasks completed successfully$(NC)"

dev: restore build publish docker
	@echo "$(GREEN)✅  Dev build completed$(NC)"

.PHONY: help restore build test test-unit test-integration publish docker clean all dev \
	db-up db-down db-restart db-logs db-clean dev-env dev-start dev-stop