# Feature governance and temporal integrity

## Two clocks and eligibility

Event time describes when a match/event occurred (or is scheduled). `available_at` describes when a validated observation became available to MIS; store `fetched_at` separately and use the conservative acceptance time for eligibility. Provider publication/update times, if trustworthy, are supplementary evidence, never an invented timestamp. For a prediction cutoff T, every input observation must have available_at ≤ T. Completed-match performance features additionally require event completion before T and must exclude the target fixture. Known future fixture schedules are eligible as schedules, never as future results.

A pre-match run requires InputDataCutoff ≤ GeneratedAt < kickoff as known in its frozen evidence. Store kickoff-as-known, fixture observation ID, and prediction market period. Unknown kickoff prevents pre-match eligibility. Later schedule corrections cannot rewrite a prediction: append an eligibility assessment, and exclude clearly invalid timing from reported pre-match performance with a recorded reason. Generation after kickoff is a late run, even with an earlier input cutoff.

## Historical import limitation

A final historical result first imported today was not known to this system before its historical kickoff. Never backdate available_at to fabricate prospective evidence. Initial backtests may use historical reconstructed datasets with explicit assumed publication lags and source-revision limitations; label them `HistoricalReconstruction`. Results from archived, time-qualified evidence are `AsOfReplay`; predictions generated and saved before kickoff are `Prospective`. Do not blend these cohorts silently in model performance reports.

Current canonical tables are projections, not valid point-in-time training sources. Retain accepted fixture observations from v0.1; when statistics, injuries and lineups are introduced, add equivalent observation history before using them predictively. Snapshot normalized inputs even if raw provider payload retention is limited by licensing. If needed evidence cannot be retained, record the reproducibility limitation and disable unsupported replay claims.

## Published definitions and frozen inputs

A FeatureSetVersion contains feature names/types/units, missing-value rules, source/producer, transformation code digest, lookback windows, competition filters and temporal rules. Descriptive metrics are produced once by Match Intelligence under a formula version; predictive transformations belong to Python. Changing any semantic rule publishes a new version.

FeatureSnapshot freezes fixture ID, cutoff, kickoff-as-known, version, typed feature values, missing reasons, immutable observation/metric references, evidence manifest/hash, transformation digest and creation time. Persist it before inference. Published definitions, snapshots and successful results are insert-only for the application role; migrations use a separate privileged identity. Administrative repair is audited and cannot silently replace historical model evidence.

## Examples and required tests

| Case | Expected behavior |
|---|---|
| A result occurred yesterday but arrived after T | Excluded from strict as-of features |
| Injury update after T with an earlier effective date | Excluded; previous known injury state remains eligible |
| Target match score appears in an imported dataset | Excluded from target pre-match features |
| Schedule for next week was known at T | Eligible for rest/congestion features using the then-known schedule |
| Final score corrected after a prediction | New observation; frozen features unchanged |
| Rolling window contains postponed/unfinished fixtures | Exclude from completed-performance averages under definition rules |
| Training scaler or calibration fitted with test rows | Fail temporal dataset validation |
| Evidence timestamp exactly T | Eligible if otherwise valid; generation must still precede kickoff |
| Timezone conversion crosses midnight or DST | UTC cutoff ordering preserved |
| Same artifact, snapshot and seed replayed | Equal output or documented numerical tolerance |

A dataset manifest records row/target identity, evidence cohort, cutoff policy, source revisions, train/validation/test windows and digests. Training metadata must distinguish when artifacts were actually built from the historical cutoff simulated by a backtest. Actual-data revisions used for evaluation are immutable references; re-evaluation creates another evaluation record, never changes the prediction.
