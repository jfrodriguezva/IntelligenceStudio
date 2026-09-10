# ADR-001: Modular monolith with vertical slices

Date: 2026-09-08. Status: Accepted. Authority: AGENTS.md sections 3, 18, 22.

## Context and alternatives

One analyst needs an evolving product spanning football data and content. Microservices would introduce deployment/network consistency work before scale warrants it. A generic layered CRUD scaffold would also fail to express the important domain boundaries.

## Decision

Use a .NET modular monolith with pragmatic domain modeling and feature-oriented application slices. Activate Football and Data Acquisition first; keep documented future boundaries without empty implementations. Use module facades, one composition root and architecture tests. Python is the deliberate separate runtime described in ADR-003.

## Consequences and validation

Local database transactions remain available and deployment stays small. Namespace boundaries require discipline and tests; modularity is not guaranteed merely by folder names. Do not add a generic repository/CQRS framework or distributed messaging system without an evidenced need. Verify allowed dependency directions and finish provider-to-UI behavior before expanding modules.

Reconsider a deployment split only with measured scaling, isolation or ownership needs and a new ADR.
