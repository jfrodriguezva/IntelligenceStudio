# Risks and assumptions

Phase Zero assumptions are reversible defaults, not claims that external accounts or services have been verified. No provider account was contacted, dependency installed or resource provisioned.

| ID | Risk / assumption | Mitigation / decision | Owner role and gate |
|---|---|---|---|
| R-01 | API-Football plan may lack selected seasons, xG, lineups or historical depth | Verify official contract and account coverage; capability matrix; null unsupported data; no purchased upgrade implicitly | Product owner + integration implementer, before real import; repeat before statistics/ML |
| R-02 | Quota/cost may make polling or backfill impractical | Bounded scopes, paging/retry budgets, manual v0.1 sync; estimate calls before enabling schedules | Product owner, before paid requests/scheduled polling |
| R-03 | Retention/redistribution rights may constrain raw payloads, logos and training datasets | Record permitted uses and retention; separate metadata/body lifecycle; no prohibited scraping | Product owner, before storing real responses or publishing provider assets |
| R-04 | Imported history cannot prove historical knowledge time | Prospective observation history now; label reconstruction vs as-of replay; no backdated availability | ML implementer, before first dataset/model |
| R-05 | Madrid-only history is insufficient for valid models | Explicit connected competition/opponent dataset and time-based evaluation | Analyst + ML implementer, v0.3 entry |
| R-06 | Provider status, timezone, extra-time and identity semantics vary | Adapter contract tests, canonical mappings, Unknown state, explicit score periods | Integration implementer, v0.1 |
| R-07 | Retries/concurrency can duplicate or regress data | Durable runs, unique keys, leases, fixture locks, atomic observations, stale-response tests | Backend implementer, v0.1 |
| R-08 | Identity provider and hosting exposure are undecided | Loopback-only dev identity; fail closed; ADR before shared deployment | Product owner + security implementer, before hosting |
| R-09 | Large target stack invites premature scaffolding | Two initial modules, one DB, no ML deployment/cache/assets until needed | Implementer, each slice review |
| R-10 | Descriptive analytics and predictive features could be duplicated | Formula/producer ownership; Python predictive transforms over frozen evidence | Backend + ML implementers, v0.2/v0.3 |
| R-11 | Future information leaks through corrections or fitted transforms | Available-at history, frozen inputs, chronological fit/calibration, explicit tests | ML implementer, every model change |
| R-12 | Model confidence/AI prose can be mistaken for evidence | Separate labels, provenance, abstain on insufficient data, human editorial review | Analyst + frontend implementer, respective feature |
| R-13 | Azure service lifecycle/pricing/region and dependency compatibility are unverified | Check official docs and cost estimate; pin/test versions; deployment ADR | Deployment implementer, before provisioning |
| R-14 | In-process scheduling conflicts with replicas or scale-to-zero | Single replica initially; durable recovery; clustering/domain locks before scale | Deployment implementer, before hosted schedules/scaling |
| R-15 | Backups without artifact consistency may fail recovery | Manifest/version references, retention and restore drill; proposed RPO/RTO validated | Operator, before production |
| R-16 | Directory has no Git metadata, CI or application tooling | Initialize repository/ignore rules and remote/CI as explicit implementation tasks; do not claim Git diff validation now | Implementer, v0.1 foundation |

## Defaults adopted for design

One workspace and configured focus team; regulation-time outcome semantics; canonical UUIDs; PostgreSQL module schemas and one migration stream; UTC instants; nullable unsupported data; manually requested bounded imports; same-origin UI/API; optional local development identity with strict loopback guard. Supported competitions, initial window, provider keys and quota limits are configuration requiring validation before account use, not hardcoded choices.

No unresolved issue prevents finishing Phase Zero documentation. External-account gates do prevent claiming real provider integration or production readiness. Revisit assumptions through ADRs when evidence changes, without rewriting historical accepted decisions.
