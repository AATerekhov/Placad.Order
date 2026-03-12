# Placad.Order

Purchase transaction record microservice — ASP.NET Core 8.0 Web API.

## Project Structure

```
src/OrderService/
├── OrderService.sln
└── OrderService/           # Main project
    ├── Controllers/        # API controllers
    ├── Properties/         # launchSettings.json
    ├── Program.cs          # Entry point & DI setup
    ├── OrderService.csproj
    ├── Dockerfile
    ├── appsettings.json
    └── appsettings.Development.json
```

## Build & Run

```bash
# Build
dotnet build src/OrderService/OrderService/OrderService.csproj

# Run (HTTP on localhost:5118)
dotnet run --project src/OrderService/OrderService/OrderService.csproj

# Run with Docker
dotnet run --project src/OrderService/OrderService/OrderService.csproj --launch-profile "Container (Dockerfile)"
```

Swagger UI available at `http://localhost:5118/swagger` in development.

## Key Details

- **Framework**: .NET 8.0, ASP.NET Core minimal hosting (no Startup.cs)
- **Ports**: HTTP 5118, HTTPS 7149; Docker: 8080/8081
- **Nullable reference types** and **implicit usings** enabled
- **NuGet packages**: `Swashbuckle.AspNetCore` (Swagger), `Microsoft.VisualStudio.Azure.Containers.Tools.Targets`
- **License**: Apache 2.0

## Conventions

- Controller-based routing with `[Route("[controller]")]` attributes
- Constructor injection for `ILogger<T>`
- No test project yet — add xUnit or NUnit under `src/OrderService/` when needed
