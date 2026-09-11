# AGENTS.md

## Project

**JaReclamouHoje** — .NET 10 Clean Architecture API (Portuguese: "Did you complain today?").

## Build & Run

```bash
# from src/
dotnet build
dotnet run --project Api        # http://localhost:5001
dotnet test
dotnet test --filter "FullyQualifiedName~Scaffold"   # single test
```

## Architecture

Clean Architecture with dependency direction: **Domain → Application → Infra → Api**

| Layer | Path | Responsibility |
|-------|------|----------------|
| Domain | `src/Domain/` | Entities, enums, repository interfaces (no dependencies) |
| Application | `src/Application/` | MediatR handlers, commands/queries, DI extension |
| Infra | `src/Infra/` | Repository implementations, DI extension |
| Api | `src/Api/` | ASP.NET Core entry point, Minimal API endpoints |
| Tests | `src/Tests/` | xUnit tests (currently only scaffold) |

**Key**: All `DependencyInjection.cs` files live in each layer's root and register that layer's services.

## Conventions

- **MediatR (CQRS)**: commands/queries in `Features/{Entity}/{Operation}/`, each with a `*Query`/`*Command` record and a matching `*Handler` class
- **Nullable** and **ImplicitUsings** enabled in every `.csproj`
- **Entities** are currently immutable (private constructor, public constructor with all props, static `Create()` factory) — early stage, not a hard rule yet
- **Response records** use `FromEntity()` static method to map from domain entities
- **Namespaces** follow folder structure: `JaReclamouHoje.{Layer}.{Feature}`
- Solution uses `.slnx` format (XML, not traditional `.sln`)

## Gotchas

- **In-memory only** — `InMemoryComplaintRepository` has no persistence; data resets on restart
- **Only 1 scaffold test exists** — no real test coverage yet
- **No CI/CD, no README, no linting config** — project is early-stage
- **.vs/ folder** is tracked — it contains Visual Studio workspace files
