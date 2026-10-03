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
| Domain | `src/Domain/` | Entities, value objects, domain services, repository interfaces, backstop validators (depends on FluentValidation) |
| Application | `src/Application/` | MediatR handlers, commands/queries, DI extension |
| Infra | `src/Infra/` | Repository implementations, DI extension |
| Api | `src/Api/` | ASP.NET Core entry point, Minimal API endpoints |
| Tests | `src/Tests/` | xUnit tests (currently only scaffold) |

**Key**: All `DependencyInjection.cs` files live in each layer's root and register that layer's services.

## Conventions

- **MediatR (CQRS)**: commands/queries in `Features/{Entity}/{Operation}/`, each with a `*Query`/`*Command` record and a matching `*Handler` class
- **Nullable** and **ImplicitUsings** enabled in every `.csproj`
- **Entities** hold no input-shape validation (no required/max-length/future-date checks, no `Max*` constants) — constructors only normalize (trim) and assign; validating `Create()` factories run the Domain backstop validators and throw `FluentValidation.ValidationException`
- **Validation split**: input shape lives in Application `*Validator` per command (primary gate → HTTP 400 via `ValidationExceptionHandler`) and in `Domain/.../Validation/*Validator` (dull backstop for direct Domain construction — presence/length only, no temporal or business logic); state-transition invariants stay in entities/services and throw `DomainException` subclasses carrying their own `StatusCode` (e.g. `ComplaintAlreadyCanceledException` → 409, `ComplaintCancellationDeniedException` → 403), mapped generically in `GlobalExceptionHandler`
- **Shared limits** live once in `Domain/Common/Validation/*Rules` (e.g. `CancellationRules.MaxReasonLength`) and are referenced by both Domain and Application validators — never duplicated, never on the entity
- **Time** comes from injected `TimeProvider` (`TimeProvider.System` registered in `AddApplication()`); services stamp `now` themselves so callers never pass timestamps around — no `DateTimeOffset.UtcNow` calls in entities/validators, no future-date checks anywhere; a future timestamp is unrepresentable by construction — tests use stub clocks
- **Auth boundary**: endpoints enforce authentication/authorization via `RequireAuthorization()`/policies at the Minimal API level; handlers assume `_currentUser.UserId!.Value` is present and never check `IsAuthenticated`/null or throw 401 — a missing user id is a misconfiguration and fails fast via `!`
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
