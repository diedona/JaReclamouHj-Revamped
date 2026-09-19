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
- **Contract-owned return types** are nested inside the abstraction that owns them (e.g. `JwtTokenResult` nested in `IJwtTokenGenerator`) — a result is meaningless outside the operation that produces it; this coupling is intentional and keeps related types cohesive. Do not create standalone result files in `Common/Interfaces/`.
- Solution uses `.slnx` format (XML, not traditional `.sln`)

## Gotchas

- **In-memory only** — `InMemoryComplaintRepository` has no persistence; data resets on restart
- **Only 1 scaffold test exists** — no real test coverage yet
- **No CI/CD, no README, no linting config** — project is early-stage
- **.vs/ folder** is tracked — it contains Visual Studio workspace files
- **Always boot-test with `dotnet run --project Api` from `src/`** — never exec the compiled `bin/.../JaReclamouHoje.Api.dll` from a random working directory. Content root follows the CWD, so a wrong CWD hides `appsettings.json`: Serilog's `ReadFrom.Configuration` finds no `Serilog` section → zero sinks → silent console, which reads as a "hang"/"crash" while the app is actually serving (or failing startup invisibly). A fallback Console sink in `AddSerilog()` keeps the logger noisy instead of mute.
- **Before diagnosing a startup hang**, confirm it's real: check for `Now listening` in logs, `curl /api/complaints`, and check the process (Kestrel Heartbeat/SocketEngine threads run even with a silent logger). Slow cold-start + silent logger has produced several false "hang" reports.
