# ADR-005: Immutable prediction results

Date: 2026-09-08. Status: Accepted. Authority: AGENTS.md sections 2.3, 6.4 and 9.

## Context and alternatives

Replacing an old prediction with a newer model result destroys honest predicted-vs-actual evaluation. Keeping only the latest probability or only a model name cannot explain what the analyst saw before kickoff.

## Decision

Separate mutable PredictionRun lifecycle from immutable successful MatchPrediction. Each result references a frozen feature snapshot, explicit model/feature versions, cutoff, generation time and result-period semantics. Preserve ensemble components. Regeneration creates a new run/result; evaluation references a specific actual observation and may append corrected evaluations.

## Consequences and validation

Historical storage grows and the UI must distinguish prediction versions. Deny update/delete on historical result tables to the runtime role; use a separate migration identity. Enforce one success per run and restrict historical FK deletions. Tests cover attempted mutation, duplicate callbacks, model changes and corrected actuals. Unsupported expected goals are null with reason, not invented for Elo.

Reconsider retention only through an explicit rights/operational decision that records consequences for reproducibility. Silent overwrites remain forbidden.
