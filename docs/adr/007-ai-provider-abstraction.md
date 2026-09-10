# ADR-007: AI provider abstraction and provenance

Date: 2026-09-08. Status: Accepted. Authority: AGENTS.md sections 6.3, 6.8 and 6.9.

## Context and alternatives

Generated interpretation supports analysis and scripts but is not a football statistic. Direct model SDK calls throughout application logic would obscure provenance and make provider changes expensive. Building a generic autonomous agent platform is outside the initial need.

## Decision

Use an AI technical module with `ITextGenerationProvider`; introduce `IEmbeddingProvider` only for a concrete retrieval need. Caller use cases supply evidence and versioned prompts. Record AIExecution metadata and return interpretations with references. The calling module owns its insight/script; AI owns execution/template metadata. Preserve original notes and historical generated versions; human editorial decisions remain explicit.

## Consequences and validation

No AI module implementation is needed for v0.1. Later adapters must support timeout/error/cost handling, input minimization and untrusted-text isolation. Tests prevent AI output from updating fact/probability tables and verify source-reference continuity. Missing provider output cannot become fabricated analysis.

Reconsider provider/model choices through configuration/versioning; new automation privileges require separate product/security design.
