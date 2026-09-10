# ADR-002: PostgreSQL as system of record

Date: 2026-09-08. Status: Accepted. Authority: AGENTS.md sections 4.2 and 4.4.

## Context and alternatives

Football identities, match relationships and immutable prediction references need relational integrity and transactional writes. A document-first store or embeddings as primary storage would weaken these relationships. Multiple business databases add unnecessary coordination initially.

## Decision

Use one PostgreSQL database with module-owned schemas and one migration stream. Use EF Core/Npgsql by default, explicit DTOs for APIs, and selective Dapper only for demonstrated analytical read complexity. Use JSONB for variable provider bodies; keep stable business values relational. Store large artifacts in object storage when needed; pgvector requires a real semantic retrieval use case.

## Consequences and validation

Relational constraints and short local transactions support safe ingestion. Module boundaries still need code enforcement. PostgreSQL behavior is tested with Testcontainers, including empty-schema migration, mapping uniqueness, rollback and observation references. Profile actual queries before extra indexes/cache.

Reconsider storage additions only for measured workload or artifact needs; do not replace relational queries with embeddings.
