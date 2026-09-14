# ADR-017: SQL Server system of record and manual data refresh

**Status:** Accepted  
**Date:** 2026-09-14

## Context

The initial Phase Zero material selected PostgreSQL and Quartz. The product owner selected SQL Server on localhost for the initial system of record and a user-triggered refresh workflow. The active implementation already uses EF Core SQL Server migrations, schema ownership through `football` and `acquisition`, API-Football credentials held by the API, and an audited `SyncRun` for every manual request.

## Decision

MIS uses SQL Server as its relational system of record. EF Core SQL Server migrations are the authoritative schema mechanism. The browser never connects to SQL Server directly; Next.js calls the .NET API, which applies domain rules and persists canonical records.

Fixture acquisition starts from the protected manual refresh endpoint. It is idempotent and records its outcome. Quartz is deferred until the manual workflow creates a measured operational need. Introducing scheduling requires ADR-015 to define cadence, quota budget, recovery, locking and hosted behavior.

## Consequences

- New persistence work uses SQL Server-compatible EF Core mappings, constraints and migrations.
- Local and integration validation must exercise SQL Server behavior where database semantics are material.
- A manual refresh is transparent to the analyst and avoids a background process before one is required.
- Existing PostgreSQL/Quartz references in Phase Zero documents are superseded by this record where they describe the active implementation.

## Validation and reconsideration

The decision is validated by applying migrations to local SQL Server, running the protected refresh path and preserving repeat-import safety. Reconsider the database only for a demonstrated SQL Server limitation. Reconsider scheduling only when the analyst needs unattended refreshes and the quota/recovery design has been reviewed.
