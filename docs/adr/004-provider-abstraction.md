# ADR-004: Football provider abstraction

Date: 2026-09-08. Status: Accepted. Authority: AGENTS.md section 6.2.

## Context and alternatives

API-Football is the initial provider, but its identifiers, payloads and coverage must not define the domain. Directly exposing its DTOs is fast initially but spreads coupling. A universal abstraction for every hypothetical provider would overgeneralize.

## Decision

Place `IFootballDataProvider` and the API-Football adapter in Data Acquisition, scoped initially to team/competition/season/fixture imports. Normalize into Football-owned commands. Maintain typed provider mappings to canonical UUIDs, capture permitted raw evidence and audit metadata, and explicitly map statuses/score periods. Verify current official provider contracts during implementation.

## Consequences and validation

An adapter adds mapping work but isolates provider changes and enables deterministic tests. Capability gaps remain visible rather than filled with invented fields. Test pagination, retries, rate limits, unknown statuses, malformed records, duplicates and partial imports. Public DTOs contain canonical IDs, never provider entities.

Add operations/providers only for a concrete use case. Provider rights/quota/retention decisions gate account-backed imports.
