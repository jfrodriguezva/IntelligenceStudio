# API boundaries

Design contract for v0.1, to become OpenAPI during implementation. ASP.NET Core owns REST business endpoints under `/api/v1`. The Next.js same-origin forwarding layer does not duplicate domain rules. EF entities and provider DTOs never form public contracts.

## First-slice endpoints

| Method and route | Request | Response and access |
|---|---|---|
| `GET /api/v1/teams` | `page`, `pageSize`, optional `search` | Canonical ID/name summaries; authenticated read roles |
| `GET /api/v1/competitions` | Pagination | Canonical competition summaries; read roles |
| `GET /api/v1/competitions/{id}/seasons` | Pagination | Season IDs/labels; read roles |
| `GET /api/v1/fixtures` | Optional `teamId`, `seasonId`, `from`, `to`, `status`; pagination; `sort=kickoffAsc` or `kickoffDesc` | Fixture summaries and total count; read roles |
| `GET /api/v1/fixtures/{fixtureId}` | Canonical UUID | Basic Match Center DTO; read roles |
| `POST /api/v1/admin/sync-runs` | Idempotency-Key header; configured provider/focus selection, season IDs, UTC date range | `202 Accepted`, run ID and Location; Admin only |
| `GET /api/v1/admin/sync-runs/{runId}` | Canonical UUID | State, timestamps, counts, sanitized error summaries; Admin only |
| `GET /health/live` | None | Minimal process liveness; platform access |
| `GET /health/ready` | None | Minimal readiness; platform access |

Lists use page ≥ 1 and pageSize 1–100, default 25. Sort by kickoff then canonical ID; null kickoffs sort last in both directions. `from` is inclusive and `to` exclusive, UTC ISO 8601. Reject inverted/excessive ranges and unknown status/sort values. Offset pagination suffices for the initial bounded dataset; concurrent imports may shift pages, which the UI handles by refreshing. Queries must remain indexed and bounded. Future analytics endpoints return explicit read models.

## DTO sketch

```json
{
  "id": "canonical-fixture-uuid",
  "competition": { "id": "canonical-competition-uuid", "name": "Competition" },
  "season": { "id": "canonical-season-uuid", "label": "Configured season" },
  "homeTeam": { "id": "canonical-home-uuid", "name": "Home team" },
  "awayTeam": { "id": "canonical-away-uuid", "name": "Away team" },
  "kickoffUtc": null,
  "status": "Scheduled",
  "score": { "regulation": null, "extraTime": null, "penalties": null },
  "freshness": { "lastObservedAt": "2026-09-08T12:00:00Z", "state": "Current" },
  "warnings": []
}
```

IDs above are illustrative placeholders. Score periods, when available, each contain `home` and `away` nonnegative integers. UI labels the period rather than inventing a combined total. Freshness states are Current, Stale and Unknown, based on an explicit configured policy appropriate to status. The DTO lastObservedAt is the last successful source confirmation, including an unchanged record; it is not the immutable observation available_at used for historical features. Admin run data reports unsuccessful attempts independently. Provider attribution can be shown through a separate source summary, with no raw payload or credentials.

List envelope: `items`, `page`, `pageSize`, `totalCount`. Use consistent Problem Details with stable machine code, traceId and field errors: 400 invalid input, 401 unauthenticated, 403 forbidden, 404 missing resource, 409 conflicting idempotency/scope, 429 application request limit, 503 unable to durably accept work. Provider errors after acceptance belong to the run outcome, not an HTTP response already sent.

The same idempotency key and body return the same run location; a different body with that key is 409. Scope conflicts can return the existing active run reference. Store key/body fingerprint durably with run metadata; define cleanup only after the maximum retry window is documented. No synchronous long provider fetch in the request handler.

## Future boundaries

Predictions have create-run and read-result endpoints; there is no update-result endpoint. Betting accepts odds snapshots and creates assessments referencing immutable IDs. Notebook corrections append revisions. Content exposes draft/review/version actions. ML endpoints remain private and are specified in the [ML contract](../ml/service-contract.md). Do not implement these routes in v0.1.

Generate TypeScript types/client from backend OpenAPI where practical and check schema drift in CI. Apply authorization to every route, request size limits, cancellation, UTC handling, and correlation IDs. OpenAPI and operational details must not expose private internals on a public deployment.

