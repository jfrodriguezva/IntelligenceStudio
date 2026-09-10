# Local development architecture

This is the planned setup, not a working installation guide. Phase Zero creates no Compose file or application projects.

## v0.1 environment

Use Docker Compose for PostgreSQL and persistent local volumes; bind infrastructure ports to loopback. Run .NET and Next.js locally for debugger/hot reload support, or document equivalent container commands when implemented. Provide Redis and MinIO through optional Compose profiles when cache/assets are first needed; the default first slice requires only PostgreSQL. This stages the expected AGENTS.md infrastructure without unused mandatory services.

The first implementation task must record tested prerequisites and exact commands for dependency installation, local secrets, infrastructure startup/readiness, migrations, development servers, tests, and teardown. Target .NET 10 and Next.js 16 come from AGENTS.md; pin compatible SDK, Node, package manager, Python and container versions when their projects are introduced. Commit lockfiles and configuration templates, never secrets.

## Configuration contract to implement

| Setting category | Planned contents |
|---|---|
| Database | Local connection string supplied through environment/user secret mechanism; independent app and migration identities |
| Provider | Base URL allowlist, API key environment variable, provider code, selected external focus-team/competition/season keys |
| Sync | Maximum date span/pages, request and retry budgets, timeout, cadence if enabled, stale thresholds |
| Web/API | Same-origin forwarding target, local origins, server-only secrets |
| Authentication | Explicit loopback-only development identity or configured real authentication; fail closed outside Development |
| Telemetry | Local structured output and optional collector endpoint; no personal data/raw payload logging |

Provide an `.env.example` with placeholders and ignore local secret files when initializing the implementation repository. Validate required configuration on startup with sanitized actionable errors. No real provider requests on startup without an explicit configured sync action.

## Repeatable developer workflow

1. Install documented pinned prerequisites and restore from lockfiles.
2. Copy placeholder configuration and set local secrets outside tracked files.
3. Start infrastructure and wait for readiness; apply migrations with the migration identity.
4. Start API and web; use deterministic synthetic data for ordinary development and tests.
5. Run explicit bounded real-provider import only after coverage/quota/retention configuration passes its gate.
6. Run relevant checks; stopping services preserves volumes. Any destructive reset command must clearly identify its local scope and be separately documented.

Offline fixtures must be labeled synthetic, include upcoming and finished matches, and never masquerade as live data. They must exercise the same normalization/contracts as the provider adapter. Python setup and artifact storage instructions arrive with the first ML slice, not as undocumented prerequisites for the Match Center.
