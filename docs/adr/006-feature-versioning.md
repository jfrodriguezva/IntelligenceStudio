# ADR-006: Versioned features and temporal evidence

Date: 2026-09-08. Status: Accepted. Authority: AGENTS.md sections 7.1 and 9.

## Context and alternatives

Training on current corrected tables can leak future knowledge. Storing only a feature name or event date cannot reconstruct what was available. Full event sourcing would be disproportionate; explicit observation history and frozen model inputs suffice.

## Decision

Publish immutable feature-set definitions including types, formulas, producers, missing rules and lookbacks. Freeze values, evidence references, cutoff and digests before inference. Use available-at timestamps as well as event time. Keep historical reconstruction, as-of replay and prospective evaluation cohorts distinct; never backdate newly imported history. Version fitting/calibration and dataset manifests with chronological partitions.

## Consequences and validation

Point-in-time datasets require retained evidence, not just current projections. Capture fixture history in v0.1 and equivalent histories before later data becomes predictive. Tests cover late-arriving records, target leakage, corrected facts, cutoff boundaries, train/test preprocessing and replay. Retention rights may limit exact raw replay and must be disclosed.

Reconsider feature semantics by publishing a new version, never editing a definition referenced by a historical prediction.
