# Testing and quality strategy

Phase Zero is documentation-only. Builds, migrations, application lint and runtime tests are not yet applicable; they must not be reported as passing. Document verification checks relative links, Markdown structure, Mermaid block structure, requirement coverage and cross-document consistency. Render Mermaid in the repository viewer when available; structural checks alone are not rendering proof.

## Implementation checks

| Layer | Tools | Important cases |
|---|---|---|
| Domain/application | xUnit | Distinct participants, nullable/period scores, fixture corrections, invalid input, idempotency/error classification |
| PostgreSQL integration | xUnit + Testcontainers | Migrations from empty DB, mapping constraints, atomic rollback, concurrency, leases, replay after crash, observation ownership/history |
| Architecture | .NET boundary tests | Domain has no provider/EF/HTTP types; allowed module dependency graph; DTO/persistence separation |
| Provider contract | Sanitized synthetic fixtures and controlled HTTP server | Multiple pages, 429/Retry-After, timeout, 5xx, malformed payload, unknown status, missing fields, partial failure, quota exhaustion |
| Frontend | Vitest + Testing Library | Loading/empty/error/stale views, period score labels, null kickoff, accessibility and navigation |
| End-to-end | Playwright with real API and PostgreSQL; stub external provider | Admin sync → run completion → list → Match Center; authorization; duplicate import; failed refresh preserves existing data |
| Python from v0.3 | pytest | Feature math, chronological windows, leakage, serialization, contract compatibility, deterministic transformations/replay, calibration/model evaluation |

Do not mock PostgreSQL when constraints, SQL, locking or transactions are under test. Mock/stub external providers to avoid quota and network dependence in required CI. Keep an opt-in, bounded, account-backed smoke test outside routine CI; record its result before claiming real integration works. Never commit captured licensed payloads or secrets without permission to retain them.

## v0.1 acceptance suite

- A clean database migrates and imports configured team/opponents, competitions/seasons, upcoming and finished fixtures.
- Repeated imports, parallel equivalent requests and overlapping scopes yield one canonical identity and coherent current observation.
- A crash after record commit but before page checkpoint replays safely; stale worker lease recovery is tested.
- Failed/malformed records remain audited; valid records survive partial failure; old data stays readable.
- Delayed older responses cannot replace newer accepted observations. Schedule and result corrections append history.
- Cookie-authenticated read access and Admin-only sync are enforced server-side, including direct HTTP requests and CSRF rejection.
- The web consumes generated contracts, displays honest freshness/unknown values, and opens a fixture by canonical ID.
- Logs correlate requests/jobs/import errors without exposing credentials or raw payloads.

## CI/CD gates

GitHub Actions should restore/install, lint/format, build backend/frontend, run unit/integration/frontend/E2E tests, check generated API drift, and build Docker images. Python formatting/type checks/tests apply once Python exists. Use pinned toolchains and locked dependencies; provision PostgreSQL through Testcontainers on a Docker-capable runner. Required failures block merge/deployment. Document required branch checks once a remote repository exists.

Run migration tests for empty and supported previous schema states when migrations change. Test backup restore and deployment smoke checks before hosting production. Use representative query plans before performance tuning; initial latency/error budgets are measured during the first slice rather than asserted without evidence.
