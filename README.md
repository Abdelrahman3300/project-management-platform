# Project Management Platform

A full-stack project management application built with **Angular 19**,
**ASP.NET Core Web API (.NET 9)**, and **PostgreSQL**. The platform
supports authenticated project and task workflows, project members, task
comments, and dashboard statistics.

> This project is under active development. Features and setup
> instructions may change.

## Features

-   User registration and JWT bearer authentication
-   Protected API endpoints
-   Project management
-   Project membership management
-   Task creation, retrieval, updating, and deletion
-   Task statuses: Todo, In Progress, Completed, and Cancelled
-   Task priorities, due dates, and assignment to project members
-   Task comments
-   Dashboard statistics and overdue-task tracking
-   Task filtering by search term, status, priority, and assignee
-   Backend API integration tests
-   Docker-based PostgreSQL development environment and CI workflow

## Technology Stack

**Frontend** - Angular 19 - TypeScript - HTML and CSS

**Backend** - C# - ASP.NET Core Web API - .NET 9 - Entity Framework Core
9 - JWT bearer authentication - ASP.NET Core built-in OpenAPI

**Database and tooling** - PostgreSQL - Npgsql Entity Framework Core
provider - Docker / Docker Compose - xUnit - GitHub Actions

## Architecture

The backend uses a layered structure:

  -----------------------------------------------------------------------
  Project                             Responsibility
  ----------------------------------- -----------------------------------
  `FullStackProject.Domain`           Domain entities and enums

  `FullStackProject.Application`      DTOs, service contracts,
                                      application services, and
                                      validation

  `FullStackProject.Infrastructure`   Entity Framework Core, database
                                      configuration, and repositories

  `FullStackProject.API`              HTTP controllers, authentication,
                                      dependency injection, and API
                                      startup

  `FullStackProject.API.Tests`        API integration tests
  -----------------------------------------------------------------------

The Angular application is located in `frontend/full-stack-project`.

## Prerequisites

-   .NET 9 SDK
-   Node.js and npm
-   PostgreSQL, or Docker Desktop with Docker Compose
-   Git

## Getting Started

### 1. Clone the repository

``` bash
git clone https://github.com/Abdelrahman3300/project-management-platform.git
cd project-management-platform
```

### 2. Configure the backend

Configure the API's PostgreSQL connection string and JWT settings using
local development configuration or environment variables. Use the exact
configuration keys expected by the API.

Do not commit database passwords, JWT signing keys, or other secrets.
Use a strong JWT signing key of at least 32 bytes.

### 3. Restore backend dependencies

From the repository root:

``` bash
dotnet restore ./backend/FullStackProject.API/FullStackProject.API.csproj
```

### 4. Apply database migrations

``` bash
dotnet ef database update --project ./backend/FullStackProject.Infrastructure/FullStackProject.Infrastructure.csproj --startup-project ./backend/FullStackProject.API/FullStackProject.API.csproj
```

If needed, install the matching EF Core CLI tool:

``` bash
dotnet tool install --global dotnet-ef --version 9.0.6
```

### 5. Run the API

``` bash
dotnet run --project ./backend/FullStackProject.API/FullStackProject.API.csproj
```

When enabled, the built-in OpenAPI document is available at:

``` text
/openapi/v1.json
```

The actual host and port depend on the API launch profile and local
configuration.

### 6. Run the Angular frontend

Open another terminal:

``` bash
cd frontend/full-stack-project
npm install
npm start
```

If there is no `start` script in `package.json`, run:

``` bash
npx ng serve
```

Open the local URL printed by Angular CLI (commonly
`http://localhost:4200`). Ensure the frontend API base URL matches the
backend address.

## Running Tests

Run the backend integration tests from the repository root:

``` bash
dotnet test ./backend/FullStackProject.API.Tests/FullStackProject.API.Tests.csproj -c Release
```

To collect code coverage:

``` bash
dotnet test ./backend/FullStackProject.API.Tests/FullStackProject.API.Tests.csproj -c Release --collect:"XPlat Code Coverage"
```

Coverage output is generated under the test project's `TestResults`
directory.

## API Documentation

The API uses ASP.NET Core's built-in OpenAPI integration. Check
`/openapi/v1.json` on the running API when OpenAPI is enabled. Swagger
UI is not assumed to be configured.

## Security Notes

-   Keep secrets outside source control.
-   Use a strong JWT signing key (at least 32 bytes).
-   Restrict CORS to trusted frontend origins.
-   Do not expose PostgreSQL publicly in production.
-   Use separate configuration for development, testing, and production.
-   Review authorization rules before deployment; administrative
    user-management operations should be limited to appropriately
    authorized users.

## Project Structure

``` text
project-management-platform/
├── backend/
│   ├── FullStackProject.API/
│   ├── FullStackProject.Application/
│   ├── FullStackProject.Domain/
│   ├── FullStackProject.Infrastructure/
│   └── FullStackProject.API.Tests/
├── frontend/
│   └── full-stack-project/
└── README.md
```

## Development Status

This project is actively developed. Backend features, integration tests,
security improvements, frontend functionality, containerization, and
deployment preparation are being implemented incrementally.

## Contributing

1.  Fork the repository.
2.  Create a feature branch.
3.  Implement your change and add or update tests.
4.  Run the test suite.
5.  Open a pull request with a clear description.

## License

No license has been specified yet. Add a license file if you intend to
grant others permission to use, modify, or distribute this project.
