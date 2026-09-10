# Business invariants and enforcement

These implement AGENTS.md section 9. Enforcement is planned; tests are required when the corresponding slice is built.

| ID | Rule | Enforcement and proof |
|---|---|---|
| INV-01 | Historical predictions are immutable | Insert-only result path; application DB role denied update/delete on result tables; integration test rejects mutation. Run state remains separately mutable. |
| INV-02 | Model versions are identifiable | Immutable version/artifact digest and FK from every run/result; reject unknown or mismatched response version. |
| INV-03 | Feature sets are versioned | Published definitions append-only; snapshot FK and schema validation; unknown version rejected. |
| INV-04 | Every pre-match prediction knows its cutoff | Required UTC cutoff, snapshot ID, generation time and kickoff-as-known; persistence validation. |
| INV-05 | Features cannot contain future knowledge | Observation availability ≤ cutoff, event eligibility by feature semantics; cutoff and generation before kickoff-as-known; chronological backtest/leakage tests. See feature governance for limitations. |
| INV-06 | Provider IDs differ from domain IDs | MIS-generated UUIDs and typed mapping uniqueness/FKs; duplicate and multi-provider tests. |
| INV-07 | Provider DTOs cannot leak into domain | Adapter normalization into Football-owned contracts; architecture test blocks provider namespace dependencies. |
| INV-08 | AI interpretation is not a statistic | Separate content type/storage and UI labels; evidence references and tests that generated values cannot update fact tables. |
| INV-09 | Analyst opinion does not alter model output | Separate analyst record; no prediction edit command; authorization and persistence tests. |
| INV-10 | Prediction differs from betting value | Betting references a prediction and odds snapshot; no odds dependency in baseline model and no probability mutation by assessment. |
| INV-11 | Odds are timestamped snapshots | Required observed/captured times and market semantics; updates create snapshots. |
| INV-12 | Market changes preserve past evaluation | ValueAssessment retains snapshot IDs and calculation version; regression test with later odds. |
| INV-13 | Raw and derived data remain distinct | Source observations vs formula-versioned metrics; null/missing data never silently coerced to zero. |
| INV-14 | Predictions are reproducible where practical | Freeze values/evidence/artifact/configuration/seed/environment; repeat inference tolerance documented per model. |
| INV-15 | Analytics retain provenance | Evidence references from observations through metrics, snapshots, predictions, and content; drill-through test and retention policy. |

## Additional constraints

Probabilities must be finite, each in [0,1], and sum to 1 within a specified contract tolerance (initial proposal: 1e-6). Reject materially invalid outputs; never silently normalize a broken model response. Expected goal counts are finite and nonnegative when supported, otherwise null with a reason. Confidence has a versioned definition; no arbitrary percentage.

Repeated and concurrent imports must not create duplicate mappings, fixtures, or prediction results. An old response cannot overwrite a newer accepted observation. Canonical updates, accepted evidence, and mappings commit atomically; raw capture and failed-run audit may survive independently. External calls cannot hold a database transaction open.

Original notes and generated script history are retained. Edits append revisions; AI grouping points to original note IDs. Tactical coordinates are normalized and scene formats versioned. Only authorized users can mutate their permitted resources; hidden UI controls do not enforce authorization.
