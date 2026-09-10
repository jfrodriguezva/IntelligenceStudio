# Phased implementation backlog

These are planned tasks, not implemented features. Finish each coherent slice and its checks before expanding scope. Owners below describe responsibilities; one developer may perform all roles. No effort estimates assume unverified provider coverage.

## Phase Zero

Architecture, domain/ER model, boundaries, invariants, contracts, operations, security, risks, decisions and this backlog are delivered as documentation. The [critical review](../architecture/phase-zero-review.md) records corrections and remaining implementation gates. Phase Zero does not authorize claiming a running application or completed real-provider integration.

## v0.1 — Match Center Foundation

| ID | Deliverable / owner | Dependencies | Acceptance criteria |
|---|---|---|---|
| MC-01 | Repository and minimal local foundation / implementer | Phase Zero | Initialize Git if still absent; add appropriate ignores/config templates; create only required API/web projects and PostgreSQL Compose infrastructure; pin toolchains; README contains tested setup, migration, run and check commands; secrets excluded. |
| MC-02 | Football and Acquisition persistence / backend | MC-01 | Implement entities/mappings/runs/observations from ER model; migration from empty DB passes; canonical IDs independent of provider IDs; constraints, atomic import and current-observation ownership tested in real PostgreSQL. |
| MC-03 | Provider contract and capability check / integration + product owner | Phase Zero; MC-01 for code | Verify current official API contract and account coverage; document status/score/page semantics, selected scope, permitted capture/retention, credentials mechanism and quota estimate (ADR-010); deterministic synthetic contract fixtures available. Real calls wait for account gate. |
| MC-04 | Durable bounded importer / backend | MC-02, MC-03 | Admin request persists queued run; Quartz claims/recover leases; imports configured team/opponents, competition/season, upcoming and recent finished fixtures; paging/retry budgets enforced; duplicate/concurrent/crash replay safe; partial errors and corrected observations retained. |
| MC-05 | Read/sync API and access control / backend | MC-02, MC-04 | Explicit paginated/filterable DTOs and OpenAPI; fixture detail, run creation/status, Problem Details and idempotency contract; same-origin auth boundary, Admin sync and CSRF tested; guarded local identity only in loopback Development. |
| MC-06 | Fixture list and basic Match Center / frontend | MC-05 | Generated client/types; focus/season/date navigation; canonical fixture selection; participants, kickoff, status, period-specific scores and freshness; loading/empty/error/stale/null states; keyboard-accessible navigation. |
| MC-07 | Integration, diagnostics and CI / implementer | MC-01 through MC-06 | Backend/frontend builds, lint/format, unit/PostgreSQL/architecture/UI/E2E checks and image builds pass; provider failure leaves data readable; traces correlate sync; generated contract drift checked; commands documented. |
| MC-08 | Real-provider acceptance and slice review / analyst + implementer | MC-03 gate, MC-07 | One explicitly bounded permitted sync reaches UI; repeat causes no duplicates; captured evidence policy honored; record actual smoke outcome, remaining coverage limits and review diff. No ML/odds/content/tactics scaffolding added. |

MC-02 and provider contract research can proceed independently after foundation, but integration depends on both. Start with a single configured competition/season and a bounded date range for the first demonstrated import, then prove the same implementation supports additional configured seasons/competitions. Reference import includes opponents; no full player database is required.

The slice is complete only when the permitted real-provider path and deterministic acceptance suite both work. If account access is unavailable, label the result as local integration complete with real-provider validation pending, not completed v0.1.

## Later releases

| Release / IDs | Deliverables | Dependencies and acceptance |
|---|---|---|
| v0.2 ST-01–03 | Team match statistics observation history; formula-versioned recent form/rolling metrics; basic Match Intelligence UI | v0.1 and coverage check. Missing xG remains unavailable; corrected statistics preserve history; window/denominator/competition semantics tested; no future data used. |
| v0.3 ML-01–04 | Dataset/feature governance; separate Python runtime; Elo with three-way mapping; stored prediction and baseline backtest | v0.2, ADR-012/013. Adequate opponent history, chronological fitting, honest cohort labels, immutable snapshots/model artifacts, private contract tests, Brier/Log Loss/calibration, leakage/replay tests. |
| v0.4 GO-01–03 | Poisson, justified Dixon-Coles extension, goal/score distribution and simulation | v0.3. Period semantics explicit; nonnegative expectations; mass/tail accounting; reproducible seed/settings; held-out baseline comparison. |
| v0.5 EV-01–03 | Evaluation dashboard, reliability/calibration plots, historical model/cohort breakdowns and drift | v0.3–0.4. Predicted-vs-actual references frozen result and actual revision; corrected results append evaluation; sample sizes and cohorts visible. |
| v0.6 BT-01–03 | Manual/permitted imported odds snapshots; normalization/value formulas; model-vs-market view | v0.5. Timestamp/market/period/line identity; implied and normalized probabilities separated; calculation version retained; later prices cannot alter old assessments; no automated bets. |
| v0.7 NB-01–03 | Fast keyboard note capture; tags/player/clip associations; AI grouping with execution provenance | v0.1 facts plus necessary player identities. Original notes remain traceable, corrections append revisions; groups link source note IDs; AI cannot mutate notes/facts; operational failures preserve manual workflow. |
| v0.8 CT-01–03 | Pre/post-match episode outlines, talking points, predicted-vs-actual block, script revisions | v0.5 and v0.7. Evidence drill-through, versioned prompt/output, analyst review and content ownership; no silent publication or regeneration. |
| v0.9 TC-01–03 | Konva pitch canvas, formations, players, arrows, zones and saved scenes | v0.1 identities; link with Content when present. Normalized coordinates, versioned format, resize/round-trip tests, fixture/segment association and export checks. |
| v1.0 WF-01–03 | Integrated pre/live/post-match navigation, recovery paths, editorial workflow and production hardening | Prior releases and hosting gates. Analyst completes an entire match cycle with traceable evidence; critical E2E workflows and restore/security checks pass. |

These are release ordering targets from AGENTS.md, not a mandate for empty dependencies: Notebook/Tactics need Football identities, not numerical model code. Implement richer player facts only when a consuming feature needs them.

## Cross-cutting deployment track

OPS-01: decide identity/exposure and ADR-009 before shared access. OPS-02: reviewed Azure IaC, priced topology, backups/restore, secret handling and ADR-011 before provisioning. OPS-03: cadence/quota and replica recovery/ADR-015 before hosted scheduling/scaling. Add Redis or analytical read models only when ADR-014 has measurements. Apply security, provenance, observability, relevant tests and documentation within every slice, not as a final cleanup release.

## Definition of done per implementation task

Affected builds, lint/format and relevant tests pass; migrations validated where changed; logs/errors and security/data integrity reviewed; boundaries honored; generated contracts/docs current; final diff reviewed and temporary/debug code removed. Report actual commands/results and known limitations. Do not postpone first-model numerical evaluation to the v0.5 dashboard.
