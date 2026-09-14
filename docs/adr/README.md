# Architecture decision records

Accepted records describe project design decisions, not implemented behavior. ADR-001 through ADR-007 implement directions mandated by AGENTS.md. ADR-008 specifies the local consistency/history design. Supersede records explicitly when decisions change; never silently rewrite their outcome.

| ADR | Decision | Status |
|---|---|---|
| [001](001-modular-monolith.md) | Modular monolith and vertical slices | Accepted |
| [002](002-postgresql.md) | PostgreSQL system of record | Superseded by ADR-017 |
| [003](003-python-ml-runtime.md) | Separate Python ML runtime | Accepted |
| [004](004-provider-abstraction.md) | Football provider abstraction | Accepted |
| [005](005-prediction-immutability.md) | Immutable prediction results | Accepted |
| [006](006-feature-versioning.md) | Feature versioning and temporal evidence | Accepted |
| [007](007-ai-provider-abstraction.md) | AI provider abstraction and execution provenance | Accepted |
| [008](008-ingestion-history-and-transactions.md) | Current projections plus observations and local transactions | Accepted |
| [017](017-sql-server-and-manual-refresh.md) | SQL Server system of record and manual data refresh | Accepted |

## Decision backlog

IDs below are reserved; decisions are not yet accepted.

| Proposed ADR | Decision needed / alternatives | Evidence and gate |
|---|---|---|
| ADR-009 | Shared-host identity provider and role mapping | Existing account ecosystem, cookie/CSRF flow, operating cost; before shared hosting |
| ADR-010 | Provider coverage, permitted retention and backfill/sync policy | Official API contract plus actual account plan, rights, request estimate; before real imports |
| ADR-011 | Azure IaC, regional services, ingress and recovery | Bicep vs Terraform, service lifecycle and priced topology, restore targets; before provisioning |
| ADR-012 | Dataset coverage, baseline fitting and promotion criteria | Available competition history, reconstruction limitations, chronological evaluation; before v0.3 training |
| ADR-013 | Artifact storage and retention | Dataset/model sizes, permitted retention, digest/versioning and recovery; before durable ML artifacts |
| ADR-014 | Redis activation or query read-model changes | Measured latency/throughput, invalidation and failure behavior; only if needed |
| ADR-015 | Scheduling cadence, clustering and scale behavior | Quota budget, job duration/recovery tests, replica/wake-up behavior; before hosted automation or scaling |
| ADR-016 | Collaboration ownership and content publication | Roles, ownership/workspace visibility, audit and explicit publish action; before multi-user editing/external publishing |

Each new ADR must include context, alternatives, decision/status, consequences, validation and trigger for reconsideration. See [implementation backlog](../product/implementation-backlog.md) for execution gates.
