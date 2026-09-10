# ADR-008: Ingestion history and local transactions

Date: 2026-09-08. Status: Accepted. Basis: AGENTS.md provenance, simplicity and temporal-integrity requirements.

## Context and alternatives

Updating fixture rows alone loses source revisions needed for future as-of analysis. Saving only raw responses makes interpretation dependent on changing parsers and retention rights. Event sourcing is unnecessary. Separate module databases would complicate atomic mapping/fact writes.

## Decision

Keep mutable current Football projections and immutable accepted normalized observations in Acquisition, linked to raw payload metadata. Use one scoped EF context/migration stream with module-owned configurations and facades. Commit mapping, canonical record and accepted observation atomically in a short local transaction. Persist raw capture and SyncRun audit independently so failures remain inspectable. Jobs claim durable requests with leases; unique keys and fixture-level concurrency protection make replay safe.

## Consequences and validation

A deliberate cross-module infrastructure transaction exists; business access still flows Acquisition → Football facade. The current-observation reference is provenance, not a Football dependency on Acquisition types. Handle the fixture/observation insertion cycle transactionally and restrict FK deletion. Tests must cover duplicate/concurrent imports, failed commits, older responses, source corrections and crash/replay. Normalized observations are not an event-sourced application log.

Reconsider shared context only if boundary enforcement or independent persistence needs justify a more complex consistency design. Do not sacrifice immutable evidence to simplify a migration.
