# ADR-003: Separate Python ML runtime

Date: 2026-09-08. Status: Accepted. Authority: AGENTS.md sections 3 and 4.3.

## Context and alternatives

Python has a distinct modeling ecosystem and model lifecycle. In-process embedding would entangle runtimes, while moving application ownership into Python would duplicate business orchestration. A separate service introduces a contract and operational cost that must remain narrow.

## Decision

Python/FastAPI owns feature transformations, training, inference, simulation, calibration, backtesting and explanations. .NET owns the system of record and submits immutable evidence/snapshots. Python receives no business database credentials. Define versioned private contracts now and implement/deploy the runtime with v0.3, when it has actual computation to perform.

## Consequences and validation

Network failures and version mismatches must be handled explicitly. .NET persists durable run state and validates successful outputs; it never accepts unregistered artifacts. Contract tests cover IDs/digests, error handling and numeric invariants; pytest covers temporal correctness and reproducibility. Long training can begin as batch work rather than a new asynchronous service framework.

Reconsider transport/worker topology only when execution duration or capacity makes the bounded initial contract insufficient. The separate-runtime direction remains authoritative.
