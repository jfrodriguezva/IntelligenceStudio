# Madrid Intelligence Studio

Madrid Intelligence Studio (MIS) is a football intelligence product, initially focused on Real Madrid and the content property **Madrid, hagámoslo real**. It will connect match facts, tactical analysis, probabilistic predictions, analyst notes, and evidence-based content production.

## Current status

Phase Zero is documented. Phase One has started with the v0.1 local foundation: a .NET API health/status surface, a Next.js web shell, SQL Server local configuration, CI definition and safe configuration templates. Football persistence and the provider integration are not implemented yet.

The active architecture is Next.js, .NET 10, SQL Server, API-Football and a future Python ML runtime. The documentation describes intended behavior, not implemented capabilities.

## Start here

- [Product vision and MVP scope](docs/product/product-vision.md)
- [Execution phases](docs/product/execution-phases.md)
- [Documentation index](docs/README.md)
- [System context](docs/architecture/system-context.md) and [container view](docs/architecture/container-view.md)
- [Module ownership and dependency rules](docs/architecture/module-boundaries.md)
- [Domain model](docs/domain/domain-model.md), [ER model](docs/domain/er-model.md), and [invariants](docs/domain/invariants.md)
- [Architecture decisions and decision backlog](docs/adr/README.md)
- [Implementation backlog and acceptance criteria](docs/product/implementation-backlog.md)
- [Risks and assumptions](docs/product/risks-and-assumptions.md)
- [Phase Zero critical review](docs/architecture/phase-zero-review.md)

## First implementation milestone

Finish one flow: API-Football → canonical football data → SQL Server → .NET API → Next.js Match Center. Support a configured focus team, selected competitions/seasons, upcoming fixtures, and recent completed fixtures. Prove repeatable imports, traceability, error handling, access control, and frontend/backend integration.

Target stack: Next.js 16, React and TypeScript; .NET 10, ASP.NET Core, EF Core and SQL Server; plus a separate Python/FastAPI ML runtime when prediction work starts. Dependency versions and provider coverage require verification during implementation.

## Local development

Prerequisites: .NET SDK `10.0.400`, Node `24`, pnpm `11`, and a local SQL Server instance. The default development connection uses a dedicated SQL Server login stored in .NET user-secrets.

```powershell
dotnet user-secrets set "ConnectionStrings:MisDatabase" "Server=localhost;Database=MadridIntelligenceStudio;User Id=mis_app;Password=REPLACE_ME;Encrypt=True;TrustServerCertificate=True" --project services/api/Mis.Api.csproj
dotnet user-secrets set "FootballData:ApiFootball:ApiKey" "REPLACE_ME" --project services/api/Mis.Api.csproj
dotnet run --project services/api/Mis.Api.csproj --urls http://localhost:5080
pnpm --dir apps/web install
pnpm --dir apps/web dev
```

Replace the two placeholders locally; do not paste either secret into chat, source code, `.env.example`, Git or the browser. The API exposes `/health/live`, `/health/ready`, and `/api/v1/system/status`. The web forwards `/api/*` to `http://localhost:5080` by default. The provider key is not used until the manual import slice is implemented.

Run the current checks with:

```powershell
dotnet build services/api/Mis.Api.csproj --configuration Release
pnpm --dir apps/web typecheck
pnpm --dir apps/web lint
pnpm --dir apps/web build
```

See the [local development plan](docs/architecture/local-development.md) and [testing strategy](docs/architecture/testing-strategy.md) for requirements that will be added with persistence and provider work.
