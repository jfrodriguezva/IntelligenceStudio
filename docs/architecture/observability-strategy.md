# Observability strategy

Use structured Serilog logs in .NET and OpenTelemetry instrumentation across HTTP, database operations and jobs. Carry trace/correlation context into queued SyncRun metadata and ML requests. Python emits compatible trace and structured error metadata when introduced.

## Signals

| Area | Logs/traces | Metrics |
|---|---|---|
| API | Route template, status, latency, trace ID; sanitized validation/error code | Request count, failure rate, duration distribution |
| Ingestion | Provider, SyncRunId, JobId, page, parser version, attempt, result counts, FixtureId where relevant | Run outcomes, request/retry counts, quota remaining if supplied, sync lag, queue age |
| Database | Slow operation/query name, timeout and lock conflict | Connection pressure, query duration, transaction failures |
| Predictions | PredictionRunId, ModelVersion, snapshot digest, error code | Run latency/failures, missing-feature rate, model disagreement; evaluation metrics by version/cohort |
| AI, later | AIExecutionId, model, prompt version, usage/cost if supplied | Failure/latency/token and cost totals; never prompt text by default |

Entity IDs belong in logs/traces, not unbounded metric labels. Metric dimensions should be bounded: provider code, route template, outcome and controlled model/version dimensions with lifecycle limits. Never log secrets, cookies, authorization headers, full provider payloads, sensitive notes or full prompts.

## Health and diagnosis

Liveness reports the process is running. Readiness checks required database access/schema compatibility; a provider outage should not remove readiness for already-stored fixture reads. ML availability becomes a prediction capability status, not an excuse to make the Match Center unavailable. Return minimal public probe bodies; detailed diagnostics require operational access.

A failed sync retains its audit record and error classification. The UI distinguishes last successful observation from last failed refresh. Define stale thresholds per fixture state; v0.1 claims no live SLA. Alert on prolonged queued/running jobs, repeated auth failures, exhausted quota, database failure, and missed expected freshness once cadence is configured. Thresholds start as documented configuration and are tuned from measurements.

## Initial operator runbook

1. Start with traceId or SyncRunId and inspect state/heartbeat, scope, attempts and sanitized errors.
2. For auth/coverage errors, correct configuration and stop automatic retries; never expose the key in diagnostics.
3. For throttling, respect provider retry time and reduce cadence/budget before retrying.
4. For parser errors, quarantine affected payloads/records and fix the adapter using permitted examples.
5. For worker failure, recover only expired leases and requeue through the durable run mechanism; never manually delete canonical facts to force a retry.
6. Verify last successful observation and page/record counts after recovery. Preserve incident history and link any corrective import.
