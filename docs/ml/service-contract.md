# ML runtime boundary and contract

The .NET Predictions module owns orchestration, business model-version metadata, feature snapshot persistence, prediction runs and results. Python owns predictive feature computation, dataset construction, training, inference, simulation, backtesting, calibration and explanations. It has no access to business-table credentials, users, authorization, content workflows or football CRUD.

## Interaction sequence from v0.3

1. .NET selects immutable observations/derived metrics satisfying a cutoff, freezes an evidence manifest and creates a run.
2. Private Python feature computation receives that manifest and a published feature-set version. It returns typed feature values, missing-data diagnostics and transformation metadata.
3. .NET validates the response and persists an immutable FeatureSnapshot with provenance, cutoff, values and digest.
4. .NET requests inference for that exact snapshot and registered model version. Python computes; it never edits the snapshot or retrieves fresher match data.
5. .NET verifies identity, contract versions, finite probabilities and diagnostics, then atomically records the successful immutable result and completes the run. Failures update only operational run state.

## Proposed private HTTP surface

| Endpoint | Essential input | Output |
|---|---|---|
| `POST /internal/v1/features/compute` | Request ID, contract version, feature-set version, cutoff, evidence manifest/hash, immutable values or scoped object reference | Values, feature-set version, evidence digest, transform version, missing reasons |
| `POST /internal/v1/predictions/infer` | Request/run ID, model ID/version, snapshot ID/hash, typed values, result period, seed if applicable | Matching IDs/hash, probabilities, nullable expected goals, diagnostics, model/artifact digest, component results |
| `GET /health/ready` | Private platform probe | Minimal readiness |

Use Pydantic and explicit .NET DTOs with compatible schema tests. Contract version identifies wire shape; model and feature-set versions identify semantics. Breaking changes require a new contract version. Numeric invariants and tolerances are defined in [domain invariants](../domain/invariants.md).

Initial baseline compute can be synchronous with a bounded timeout, orchestrated by a durable .NET job. Retries use the same run/snapshot/model request; .NET enforces one successful result per run. A late identical result can be recognized by digest, while a conflicting result is recorded as an operational error and cannot overwrite success. Do not promise exactly-once network delivery.

Use explicit error codes for incompatible version, missing model, invalid features, insufficient data and transient runtime failure. Retry only classified transient failures. Timeouts do not produce fabricated probabilities. Store sanitized failure metadata in the run. If a computation finishes after kickoff, do not classify it as a valid pre-match prediction merely because its inputs had an earlier cutoff.

## Training and artifact access

Training and long backtests begin as explicit Python batch jobs/CLI invoked through a documented operator workflow. Design an asynchronous service job API only if duration/operations justify it. Training datasets are exported by .NET as immutable manifests and bounded files; Python cannot query mutable business tables directly. Python writes model artifacts to a restricted artifact area, with code/configuration/environment and dataset digests; .NET registers validated model versions before application inference can use them.

MLflow, Optuna, boosting and SHAP enter when experiments warrant them; v0.1 does not deploy unused Python tooling. Python's runtime is separate when activated, not an in-process .NET library. Object references are allowlisted and scoped, never arbitrary user-controlled download URLs or local filesystem paths. Use private networking and service authentication with least-privilege object access. Pin compatible dependencies during implementation.
