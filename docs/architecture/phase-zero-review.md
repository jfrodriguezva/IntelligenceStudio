# Phase Zero critical review

Date: 2026-09-08. Scope: documentation and design only. Reviewer: implementing assistant self-review, not an independent architecture audit.

## Repository inspection and outcome

Read AGENTS.md completely as authoritative context. Initial file enumeration found only AGENTS.md. `git status --short` reported that the directory is not a Git repository; there were no existing source projects, migrations, tests or docs to preserve or build. Added README and documentation; left AGENTS.md unchanged and did not scaffold the application or initialize deployment infrastructure.

Phase Zero is complete as an architecture/product initialization package. The design is ready to guide MC-01 and the first vertical slice. Runtime correctness, provider compatibility, dependency compatibility and production readiness remain implementation work.

## Required critical questions

| Question | Finding and correction incorporated in final design |
|---|---|
| Is anything over-engineered? | The target stack is much larger than v0.1 needs. Restrict first implementation to Football, Acquisition, API/web and PostgreSQL. Defer Redis, MinIO activation, ML deployment, MLflow/boosting/SHAP, AI and tactical libraries until consumed. No empty future-module scaffolding. |
| Are bounded contexts meaningful? | Football owns facts; Acquisition owns source integration; Predictions owns historical quantitative results; Betting owns price comparison; editorial tools own human work. AI is a technical execution capability. Ownership and allowed arrows are explicit in module-boundaries. |
| Are responsibilities duplicated? | Potential duplicate feature math was resolved: .NET Intelligence owns descriptive formula-versioned metrics; Python owns predictive transforms. Predictions orchestrates/persists. AI owns execution metadata while callers own generated business content. |
| Can the MVP be simpler? | Use manual bounded sync, one configured competition/season for the first demonstration, one API replica, one database/context/migration stream, and a basic Match Center. No full player ingest or live polling promise. Keep transactional history because later temporal integrity depends on it. |
| Is ML isolated correctly? | A private versioned runtime receives immutable data, never business DB credentials or user workflows. Define the contract now; implement it when Elo is introduced. Long training can start as documented batch work. |
| Is provider coupling controlled? | Adapter DTOs remain in Acquisition, typed mappings preserve canonical IDs, and Football receives its own import contracts. Unknown statuses and period-specific scores prevent false canonical assumptions. Coverage and legal retention are explicit account gates. |
| Can historical predictions be reproduced? | Freeze model/artifact/environment, feature definitions/values, source references, cutoff, seed/settings and components. Raw payloads alone are insufficient. Exact replay is qualified by legally retainable evidence and numerical tolerance. Actual corrections append evaluations. |
| Is temporal leakage prevented conceptually? | Event time and MIS availability are distinct. Late-arriving/corrected observations cannot enter an earlier cutoff. Historical reconstruction is labeled separately from as-of replay/prospective results. Fitting, calibration and preprocessing follow chronological partitions. |
| Are AI-generated claims clearly separated from facts? | Separate data types/ownership, provenance and UI labels; no AI update path to fact or prediction tables. Original notes and script versions remain traceable; analyst review remains explicit. |
| Is the first vertical slice small enough to finish? | Eight bounded tasks with dependencies and measurable acceptance cover one provider → DB → API → UI path. Synthetic CI and one permitted real-provider smoke check prove different things. Account access may delay real validation without justifying unrelated feature expansion. |

## Additional consistency findings and fixes

1. **Module transactions:** strict ownership without a transaction mechanism would leave mapping/fact/audit writes inconsistent. ADR-008 specifies a shared infrastructure unit of work, Football import facade and atomic accepted observations, with network calls outside transactions. Domain dependency still points Acquisition → Football.
2. **Database relationship cycle:** current fixture observation and its owning fixture form a deliberate FK cycle. The ER model specifies temporary null during insert, assignment within the transaction and ownership checks; PostgreSQL integration tests must prove it.
3. **Unchanged source refreshes:** reusing immutable observations would otherwise make fresh data look stale. Current fixtures track last successful confirmation separately; API freshness uses that value while historical features retain the original available_at.
4. **Stale-response ordering:** persistence must retain request-start/fetch times to support the importer ordering policy. Added those fields to raw metadata and documented the limit when providers omit trustworthy update timestamps.
5. **Prediction identity consistency:** individual foreign keys alone do not prove fixture/model/snapshot agreement. Added consistency validation and composite-constraint/test expectations to the prediction extension.
6. **Honest baseline output:** Elo needs an explicit three-way mapping; expected goals stay null until supported. Numerical evaluation begins with v0.3 rather than waiting for the v0.5 dashboard.
7. **Score semantics:** regulation, extra time and penalties are distinct; ambiguous totals cannot settle regulation markets. Unknown kickoff/status remain visible rather than invented.
8. **Durable jobs:** persisting work before scheduling can leave a crash gap. A dispatcher claims queued SyncRuns, with lease recovery and idempotent replay. Persistent Quartz alone does not solve duplicate domain writes.
9. **Secure local progress:** identity-provider choice is deferred to a hosting ADR, but admin endpoints are not anonymous. A development identity is restricted to explicit loopback Development mode and must fail closed elsewhere.

## Phase Zero requirement coverage

| AGENTS.md section 21 item | Primary artifact |
|---|---|
| 1. System architecture | [System context](system-context.md), [containers](container-view.md) |
| 2. Bounded contexts/modules | [Module boundaries](module-boundaries.md) |
| 3. Dependency rules | [Module boundaries](module-boundaries.md), [ADR-001](../adr/001-modular-monolith.md) |
| 4. Initial domain model | [Domain model](../domain/domain-model.md), [language](../domain/ubiquitous-language.md) |
| 5. Initial ER model | [ER model](../domain/er-model.md) |
| 6. Business invariants | [Invariant enforcement matrix](../domain/invariants.md) |
| 7. API boundaries | [REST API design](../api/api-boundaries.md) |
| 8. Provider integration | [Import design](provider-integration.md), [ADR-004](../adr/004-provider-abstraction.md) |
| 9. ML boundary | [Service contract](../ml/service-contract.md), [modeling](../ml/modeling-strategy.md), [governance](../ml/feature-governance.md) |
| 10. Local development architecture | [Local plan](local-development.md) |
| 11. Testing strategy | [Test plan and quality gates](testing-strategy.md) |
| 12. Deployment strategy | [Azure deployment plan](deployment-strategy.md) |
| 13. Security | [Security model](../security/security-model.md) |
| 14. Observability | [Signals and runbook](observability-strategy.md) |
| 15. Risks and assumptions | [Risk register](../product/risks-and-assumptions.md) |
| 16. ADR backlog | [Eight accepted ADRs and pending decisions](../adr/README.md) |
| 17. Phased implementation backlog | [v0.1–v1.0 backlog](../product/implementation-backlog.md) |

## Verification and limits

Final document checks passed: 32 authored Markdown documents, 76 local links, 7 Mermaid blocks, 1 parsed JSON example, 17 Phase Zero coverage entries, 15 invariant entries and 8 accepted ADRs; zero issues. Checks cover titles, local Markdown link targets, balanced code fences, trailing whitespace, encoding replacement characters and JSON example parsing. Final inventory contains only Markdown files. Mermaid diagrams are included for context, containers, module dependencies, domain lifecycle, ER relationships and ingestion sequence. Their blocks are structurally checked; no Mermaid rendering toolchain exists in this repository, so visual rendering is not certified. Manual review checked all 17 deliverables, 15 authoritative invariants, allowed dependency directions, transaction/retry behavior and roadmap order.

Git diff review is unavailable because Git metadata is absent. Review instead used the initial one-file inventory, final file enumeration and direct content inspection of new documents. No application builds, runtime tests, migrations or deployment checks were run: there are no corresponding projects. No external accounts, package/service versions or provider rights were verified. These limits are recorded in risks and task gates rather than represented as completed checks.

## Next step

Implement MC-01 followed by the v0.1 backlog. Resolve ADR-010 before real provider capture and ADR-009/011 before shared hosting. No unresolved architectural contradiction requires starting a future module to unblock the Match Center.

