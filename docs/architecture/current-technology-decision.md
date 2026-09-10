# Active technology decision

Date: 2026-09-10

Madrid Intelligence Studio uses Next.js for the portal, .NET 10 / ASP.NET Core for the backend, SQL Server on localhost for development data, API-Football as the first data provider, and Python only when model training and inference start.

The portal never connects directly to SQL Server or API-Football. Next.js calls the .NET API. The backend protects provider credentials, validates external data, applies business rules and writes canonical records to SQL Server.

Data refresh starts manually from a portal button in v0.1. The backend records each import attempt and returns its outcome. Quartz or another scheduler is deferred until manual refresh creates a real operational need.

Local secrets use .NET user-secrets. The API-Football token and SQL Server credentials must not be committed, sent in chat, placed in frontend environment variables or exposed to browser code.

The older PostgreSQL-oriented Phase Zero artifacts are historical design material. This decision supersedes their database choice; they will be reconciled as each affected implementation slice is built.
