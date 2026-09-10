# AGENTS.md — Madrid Intelligence Studio

## 0. Purpose

This repository contains **Madrid Intelligence Studio (MIS)**, a production-oriented football intelligence platform focused initially on Real Madrid.

This file is the primary persistent instruction set for AI coding agents working in this repository, especially Codex.

Treat this repository as a serious software product, not a tutorial, demo, landing page, hackathon, or throwaway CRUD application.

The initial content property supported by the platform is:

**“Madrid, hagámoslo real”**

The product should eventually become the analyst's operating system for:

- football data ingestion;
- match analysis;
- predictive modeling;
- probability estimation;
- score simulation;
- betting market analysis;
- tactical analysis;
- live match note-taking;
- historical prediction evaluation;
- AI-assisted insight generation;
- podcast/content production.

The primary user is currently one football analyst/content creator, but the system should be designed so additional users, analysts and editors can be added later.

---

# 1. Product Vision

Madrid Intelligence Studio must help answer:

1. What happened?
2. Why did it happen?
3. What is likely to happen?
4. How confident are we?
5. What did our model predict?
6. Was the model right?
7. Does the betting market disagree with the model?
8. What tactical pattern matters?
9. What should the analyst talk about?
10. What evidence supports that conclusion?

Every feature should contribute to one or more of those questions, or improve maintainability, reliability, testability, security, traceability or developer experience.

---

# 2. Core Product Workflows

## 2.1 Pre-match workflow

Target workflow:

Fixture selected
→ sync relevant football data
→ calculate current form
→ build immutable feature snapshot
→ run prediction models
→ run score simulations
→ analyze likely lineups
→ generate match intelligence
→ optionally import or enter bookmaker odds
→ calculate betting value
→ generate pre-match podcast/content outline

The pre-match experience should eventually include:

- expected lineup;
- recent form;
- player availability;
- team strength;
- tactical keys;
- statistical keys;
- player matchups;
- model probabilities;
- expected goals;
- likely scorelines;
- confidence;
- data-quality warnings;
- analyst prediction;
- market comparison;
- betting value.

## 2.2 Live match workflow

Provide a fast Match Notebook for capturing notes while watching.

Example:

- 23:14 — Valverde triggers press
- 28:32 — Mbappé drags center-back
- 31:10 — Vinícius loses dangerous possession
- 44:20 — fullback positioning too high

The notebook must support rapid keyboard-driven entry and tags such as:

- PRESSING
- BUILDUP
- ATTACK
- DEFENSE
- TRANSITION
- SET_PIECE
- INDIVIDUAL
- ERROR
- TACTICAL
- GOAL
- CHANCE
- CLIP

## 2.3 Post-match workflow

Final whistle
→ sync final match data
→ sync team/player statistics
→ compare prediction vs reality
→ analyze Match Notebook
→ associate tactical scenes
→ generate post-match insights
→ generate podcast outline

A dedicated feature must exist for:

**PREDICTED VS ACTUAL**

Example:

PRE-MATCH | ACTUAL
---|---
RM win 62% | RM won
Expected xG 2.04 | actual xG 2.31
Opponent xG 0.91 | actual xG 0.74
Most likely score 2-0 | actual 3-1

Historical pre-match predictions must remain exactly as they existed before kickoff.

---

# 3. Architecture Principles

Use:

**Modular Monolith + Pragmatic DDD + Vertical Slice Architecture**

Do not introduce microservices by default.

The one intentional separate service from the beginning is the Machine Learning runtime, implemented in Python, because its ecosystem and lifecycle differ from the application backend.

Do not introduce the following without an ADR proving a real need:

- Kubernetes;
- Kafka;
- event sourcing;
- distributed sagas;
- service mesh;
- unnecessary CQRS frameworks;
- generic enterprise abstractions;
- premature microservices.

Prefer one coherent vertical slice over ten incomplete modules.

Prefer explicit code over framework magic.

Prefer boring, understandable infrastructure over fashionable complexity.

---

# 4. Target Technology Stack

## 4.1 Frontend

Use:

- Next.js 16
- React
- TypeScript
- App Router
- Tailwind CSS
- shadcn/ui
- TanStack Query
- React Hook Form
- Zod
- Apache ECharts
- Konva.js for tactical-board functionality

Use Server Components when appropriate.

Use Client Components only when browser-side interactivity requires them.

Avoid unnecessary global state.

Prefer:

1. URL state;
2. server state;
3. local component state;

before introducing a global store.

## 4.2 Core Backend

Use:

- .NET 10 LTS
- ASP.NET Core
- C#
- Entity Framework Core
- Npgsql
- PostgreSQL
- Dapper selectively for complex/read-heavy analytics
- OpenAPI

The .NET application is the primary orchestration layer and system of record.

Do not expose EF entities directly through API contracts.

Use explicit request/response DTOs.

## 4.3 Machine Learning Service

Use a separate Python service with:

- FastAPI
- Pydantic
- Polars
- NumPy
- SciPy
- scikit-learn
- XGBoost and/or LightGBM
- Optuna
- MLflow
- SHAP
- pytest

Its responsibilities include:

- feature engineering;
- training dataset construction;
- training;
- inference;
- backtesting;
- calibration;
- simulation;
- explainability;
- model evaluation.

It must NOT own:

- users;
- authentication;
- football CRUD;
- content workflows;
- authorization;
- podcast management;
- application orchestration.

## 4.4 Database

Use PostgreSQL as the primary relational system of record.

Use relational tables for football, prediction and business data.

Use JSONB only when schema variability is genuinely valuable, especially for raw provider payloads.

Use pgvector only for legitimate semantic-retrieval use cases.

Do not use embeddings as a replacement for relational queries.

## 4.5 Cache

Use Redis where it provides measurable value, such as:

- hot fixtures;
- standings;
- provider response caching;
- distributed locks;
- temporary calculations;
- reference data.

Cache invalidation must be explicit.

Do not cache live-match data for durations that make the UI misleading.

## 4.6 Object Storage

Local development:

- MinIO

Production:

- Azure Blob Storage or S3-compatible storage

Potential stored objects:

- charts;
- images;
- tactical exports;
- podcast assets;
- raw datasets;
- model artifacts;
- exports.

Avoid storing large binaries directly in PostgreSQL unless justified.

## 4.7 Background Jobs

Use Quartz.NET initially.

Potential jobs:

- SyncUpcomingFixturesJob
- SyncTeamsJob
- SyncPlayersJob
- SyncExpectedLineupsJob
- SyncOddsJob
- SyncMatchEventsJob
- SyncLiveStatisticsJob
- SyncFinalStatisticsJob
- GenerateFeatureSnapshotJob
- RunPredictionJob
- PostMatchProcessingJob

Jobs should be:

- observable;
- retryable where appropriate;
- idempotent where practical;
- safe against duplicate execution.

## 4.8 Observability

Use:

- OpenTelemetry
- structured logging
- Serilog
- correlation IDs
- metrics
- health checks
- tracing where useful

Useful dimensions include:

- FixtureId
- Provider
- SyncRunId
- PredictionRunId
- ModelVersion
- JobId

Never log secrets.

## 4.9 CI/CD

Use GitHub Actions.

Expected pipeline stages:

- restore/install;
- lint;
- format checks;
- backend build;
- frontend build;
- Python checks;
- unit tests;
- integration tests;
- frontend tests;
- Docker build.

Broken quality gates should block merge/deployment.

---

# 5. Repository Structure

Target monorepo:

```text
madrid-intelligence/
│
├── AGENTS.md
├── README.md
│
├── apps/
│   └── web/
│
├── services/
│   ├── api/
│   └── ml/
│
├── infrastructure/
│   ├── docker/
│   └── azure/
│
├── docs/
│   ├── product/
│   ├── architecture/
│   ├── domain/
│   ├── adr/
│   ├── api/
│   ├── ml/
│   └── security/
│
├── scripts/
│
├── tests/
│
└── docker-compose.yml
```

Adapt only if a clearer structure is justified.

Document important deviations.

---

# 6. Bounded Contexts / Modules

The initial domain is divided into the following business modules.

## 6.1 Football

Owns objective football entities and match facts.

Candidate concepts:

- Competition
- Season
- Team
- Player
- Coach
- Venue
- Fixture
- Lineup
- Formation
- MatchEvent
- TeamMatchStatistics
- PlayerMatchStatistics
- Standing
- Injury
- Suspension

Fixture lifecycle may include:

- Scheduled
- PreMatch
- Live
- HalfTime
- Finished
- Postponed
- Cancelled

Provider-specific state strings must be mapped into canonical domain values.

## 6.2 Data Acquisition

Owns integration with external football-data providers.

Candidate concepts:

- Provider
- ProviderTeamMapping
- ProviderPlayerMapping
- ProviderFixtureMapping
- ProviderCompetitionMapping
- SyncJob
- SyncRun
- RawPayload
- ImportError
- RateLimitState

Initial provider:

**API-Football / API-Sports**

The domain MUST NOT depend directly on API-Football DTOs.

Use an abstraction such as:

```csharp
IFootballDataProvider
```

Potential future providers:

- API-Football
- StatsBomb
- Sportradar
- Opta
- CSV/imported datasets
- proprietary datasets

Preserve provenance.

For important imported data, be able to determine:

- provider;
- provider identifier;
- fetch timestamp;
- resource/endpoint;
- sync run;
- parsing result;
- relevant errors.

## 6.3 Match Intelligence

Owns deterministic or derived analytical information based on football facts.

Candidate concepts:

- MatchAnalysis
- TeamFormSnapshot
- PlayerFormSnapshot
- ExpectedLineup
- TacticalProfile
- MatchFeature
- StrengthRating
- WeaknessRating
- HeadToHeadSnapshot
- MatchInsight

Maintain a strict distinction between:

### FACT
A value received from a trusted source.

### DERIVED METRIC
A deterministic calculation from source data.

### AI INTERPRETATION
Language generated from facts/derived metrics.

Never persist or present them as equivalent.

Example:

FACT:
Real Madrid completed 17 progressive passes.

DERIVED:
That value is 18% above the rolling team average.

AI INTERPRETATION:
Real Madrid progressed the ball more aggressively than usual.

## 6.4 Predictions

Prediction is a first-class business domain.

Candidate concepts:

- PredictionModel
- ModelVersion
- TrainingRun
- FeatureDefinition
- FeatureSetVersion
- FeatureSnapshot
- PredictionRun
- MatchPrediction
- ScorePrediction
- SimulationRun
- ModelMetric
- CalibrationMetric
- BacktestRun

A prediction must be immutable after creation.

Do not overwrite historical predictions because a new model version exists.

Every prediction should record at least:

- FixtureId
- ModelId
- ModelVersion
- FeatureSetVersion
- GeneratedAt
- InputDataCutoff
- HomeWinProbability
- DrawProbability
- AwayWinProbability
- ExpectedGoalsHome
- ExpectedGoalsAway
- Confidence
- DataQuality

For ensemble models, preserve component outputs.

Example:

- Elo: 0.60
- Poisson: 0.64
- ML: 0.63
- Ensemble: 0.62

Never discard component predictions.

## 6.5 Betting Intelligence

Prediction and betting must remain separate concepts.

Candidate concepts:

- Bookmaker
- Market
- MarketSelection
- OddsSnapshot
- ImpliedProbability
- NormalizedProbability
- ModelProbability
- Edge
- ValueAssessment
- BetScenario
- BetSlip
- Settlement

The application does NOT automatically place bets.

Odds may initially be:

- manually entered;
- imported from a supported provider;
- loaded through permitted integrations.

Do not implement fragile or prohibited scraping just to obtain bookmaker prices.

**Value does not mean “Madrid will win.”**

Value means the model's estimated probability is favorable relative to the offered price, subject to uncertainty and data quality.

Track:

- model probability;
- market implied probability;
- normalized market probability;
- probability edge;
- odds edge;
- confidence;
- model agreement;
- data quality;
- timestamp.

## 6.6 Tactical Studio

Candidate concepts:

- TacticalBoard
- TacticalScene
- PlayerMarker
- BallMarker
- Zone
- Arrow
- Run
- Pass
- Press
- DefensiveLine
- Annotation
- Spotlight
- ClipMarker

Required capabilities eventually include:

- drag players;
- change formations;
- draw arrows;
- draw passing lanes;
- draw movement;
- shade zones;
- highlight players;
- spotlight ball;
- highlight defensive lines;
- save scenes;
- associate a scene with a fixture or content segment.

Use normalized pitch coordinates, not fixed screen pixels.

Persist tactical scene state in a stable, versioned format.

## 6.7 Match Notebook

Candidate fields:

- FixtureId
- MatchMinute
- MatchSecond
- Timestamp
- Text
- Tags
- Players
- Team
- Category
- Importance
- ClipCandidate
- CreatedAt

Optimize for speed.

AI may group notes into patterns but source notes remain immutable and traceable.

Example AI cluster:

“Valverde pressing pattern”

based on original notes at 23:14, 38:02, 55:31.

## 6.8 Content Studio

Candidate concepts:

- PodcastEpisode
- EpisodeSegment
- TalkingPoint
- Script
- Article
- SocialPost
- ThumbnailBrief
- Chart
- ContentAsset
- PromptTemplate
- AIExecution

Initial content property:

**Madrid, hagámoslo real**

Primary workflows:

- PRE-MATCH
- POST-MATCH

The human analyst remains responsible for final editorial decisions.

## 6.9 AI

Do not couple application logic directly to a single AI provider.

Potential interfaces:

```csharp
ITextGenerationProvider
IEmbeddingProvider
```

Application-level services might include:

- MatchInsightGenerator
- MatchNotesAnalyzer
- PodcastOutlineGenerator
- PostMatchSummaryGenerator

Track AI execution metadata when useful:

- model;
- prompt template version;
- execution time;
- input references;
- output;
- token usage;
- cost if available.

Prompt templates must be versioned.

Do not silently mutate historical AI-generated content when prompts change.

---

# 7. Machine Learning Strategy

Start with interpretable baselines before sophisticated models.

Candidate models:

1. Elo
2. Poisson
3. Dixon-Coles
4. Recent-form model
5. Gradient boosting model
6. Ensemble

Do not immediately build a complicated neural model.

Establish trustworthy baselines first.

Evaluate probability models using:

- Log Loss
- Brier Score
- calibration curves
- multiclass calibration
- predicted-vs-observed analysis
- competition-level performance
- home/away performance
- model drift

Accuracy alone is NOT sufficient.

## 7.1 Feature Engineering

Features must be computable strictly using information available before the prediction timestamp.

Potential features:

- Elo
- rolling goals scored
- rolling goals conceded
- rolling xG
- rolling xGA
- shots
- shots on target
- big chances
- possession
- progressive metrics if available
- recent form
- home form
- away form
- rest days
- schedule congestion
- opponent strength
- competition strength
- player availability
- expected lineup strength
- injuries
- suspensions
- manager tenure
- recent formation
- head-to-head with low weighting

Temporal leakage is a critical defect.

Add explicit tests protecting against it.

Historical feature snapshots used for predictions must be immutable.

## 7.2 Simulation

Support Monte Carlo or equivalent score simulation.

Expose probabilities such as:

- home win;
- draw;
- away win;
- score distribution;
- over/under;
- both teams to score;
- goal difference;
- win by 2+.

Do not present simulated precision as certainty.

---

# 8. Bias Control

The system should actively reduce narrative bias.

Potential warnings:

- MODEL DISAGREEMENT
- LOW DATA QUALITY
- LARGE MARKET DISAGREEMENT
- INSUFFICIENT SAMPLE
- RECENT MANAGER CHANGE
- KEY PLAYER AVAILABILITY UNCERTAIN
- UNUSUAL LINEUP
- MARKET MOVEMENT

Human analysis should remain separate from quantitative model output.

The analyst may write:

“My football interpretation favors Real Madrid.”

That must NOT automatically alter the numerical model probability.

---

# 9. Critical Domain Invariants

Treat these as mandatory unless an ADR intentionally changes one.

1. Historical predictions are immutable.
2. Model versions are identifiable.
3. Feature sets are versioned.
4. Every pre-match prediction knows its data cutoff.
5. Pre-match features cannot contain future information.
6. Provider IDs are not domain IDs.
7. Provider DTOs do not leak into domain entities.
8. AI interpretation is not a factual statistic.
9. Model probability and analyst opinion are separate.
10. Prediction and betting value are separate.
11. Odds are timestamped snapshots.
12. Market prices may change without altering historical evaluation.
13. Raw data and derived data must remain distinguishable.
14. Predictions should be reproducible where practical.
15. Important analytics should retain data provenance.

---

# 10. Security

Use secure-by-default practices.

Requirements:

- never commit secrets;
- use environment variables locally;
- use proper secret management in production;
- validate external inputs;
- validate provider payloads;
- validate file uploads;
- apply server-side authorization;
- protect administrative routes;
- use security headers;
- use CSRF protections appropriate to the auth model;
- avoid arbitrary file execution;
- never log secrets;
- keep dependencies patched.

Initial roles:

- Admin
- Analyst
- Editor
- Viewer

Prefer secure HTTP-only browser cookies where appropriate.

Do not rely only on hidden UI controls for authorization.

---

# 11. Testing

## Backend

Use:

- xUnit
- Testcontainers
- integration tests
- architecture/boundary tests

## Frontend

Use:

- Vitest
- Testing Library
- Playwright

## Python

Use:

- pytest

## ML-specific tests

Must include validation for:

- feature calculations;
- time boundaries;
- temporal leakage;
- deterministic transformations;
- training dataset construction;
- prediction contracts;
- model serialization;
- reproducibility where practical.

Do not mock PostgreSQL when actual PostgreSQL behavior is material.

Use Testcontainers.

Critical workflows require integration tests.

Important user workflows require E2E tests.

---

# 12. API Rules

Use REST initially.

Expose OpenAPI documentation.

Use explicit request/response contracts.

Do not expose persistence entities directly.

Support pagination for lists.

Use filtering and sorting intentionally.

For expensive analytics endpoints, create explicit read models rather than giant generic payloads.

Maintain frontend/backend schema consistency through generated clients/types where practical.

---

# 13. Performance

Do not prematurely optimize.

However:

- prevent N+1 query patterns;
- paginate large collections;
- index real query paths;
- avoid loading huge historical datasets into memory;
- cache appropriate reference data;
- profile before major optimization;
- use Dapper/read models when analytical queries become cumbersome in EF Core.

---

# 14. Local Development

The developer experience should eventually support:

```bash
docker compose up
```

for required local infrastructure.

Expected infrastructure:

- PostgreSQL
- Redis
- MinIO

Application processes may run locally or in containers depending on developer-experience tradeoffs.

No undocumented manual setup.

---

# 15. Production Reference Architecture

Initial production target:

Azure.

Candidate services:

- Azure Container Apps
- Azure Database for PostgreSQL
- Azure Cache for Redis
- Azure Blob Storage
- Azure Key Vault
- Application Insights

Do NOT introduce AKS/Kubernetes initially.

Use Infrastructure as Code when deployment work begins.

---

# 16. Documentation Requirements

Maintain documentation under `/docs`.

Expected documents include:

```text
docs/product/product-vision.md
docs/architecture/system-context.md
docs/architecture/container-view.md
docs/architecture/module-boundaries.md
docs/domain/domain-model.md
docs/domain/ubiquitous-language.md
docs/ml/modeling-strategy.md
docs/ml/feature-governance.md
docs/security/security-model.md
```

Use Mermaid diagrams where useful.

Create ADRs for consequential decisions.

Initial ADR candidates:

- ADR-001 Modular Monolith
- ADR-002 PostgreSQL
- ADR-003 Separate Python ML Runtime
- ADR-004 Football Provider Abstraction
- ADR-005 Prediction Immutability
- ADR-006 ML Feature Versioning
- ADR-007 AI Provider Abstraction

---

# 17. Code Quality Rules

Use clear domain terminology.

Avoid vague names such as:

- Manager
- Helper
- Utils
- CommonService
- DataProcessor

unless the responsibility is actually precise.

Prefer names such as:

- PredictionCalibrator
- FixtureImporter
- OddsNormalizer
- FeatureSnapshotBuilder
- MatchNotesAnalyzer

Avoid:

- god objects;
- massive service classes;
- generic repositories without a real need;
- interfaces for every class purely for ceremony;
- static global state;
- dead code;
- unexplained TODO comments.

Comments should explain WHY, not restate WHAT code already says.

---

# 18. Agent Operating Procedure

Before making significant changes:

1. inspect the repository;
2. inspect this `AGENTS.md`;
3. inspect relevant `/docs`;
4. understand existing module boundaries;
5. identify architectural consequences;
6. propose the smallest coherent implementation;
7. implement;
8. build;
9. run relevant tests;
10. review the diff;
11. fix failures;
12. update documentation if needed.

Do not blindly generate large codebases.

When requirements are ambiguous but reversible:

- choose a reasonable production-quality assumption;
- document it;
- keep the decision reversible;
- continue working.

Ask for clarification only when the decision would materially affect:

- security;
- data integrity;
- product behavior;
- irreversible architecture;
- cost/external integration risk.

---

# 19. Definition of Done

A task is not complete merely because code was written.

Before declaring completion:

- affected projects build successfully;
- relevant tests run;
- lint/format checks run;
- database migrations are validated where relevant;
- errors/logs are reviewed;
- architecture boundaries are respected;
- security implications are checked;
- temporal/data-integrity implications are checked;
- documentation is updated when necessary;
- dead/debug code is removed;
- the final diff is reviewed.

Report:

- what changed;
- why;
- tests executed;
- important decisions;
- known limitations;
- recommended next step.

---

# 20. MVP Roadmap

Do NOT attempt to build the whole platform at once.

## v0.1 — Match Center Foundation

Implement enough foundation to:

1. run locally;
2. persist football data;
3. connect API-Football through an abstraction;
4. import Real Madrid;
5. import required competitions/seasons;
6. import upcoming fixtures;
7. import recent completed fixtures;
8. display fixtures in the web UI;
9. select a fixture;
10. display a basic Match Center;
11. prove backend/frontend integration;
12. establish testing and architecture conventions.

Do NOT implement full betting, AI content generation, tactical board or sophisticated ML yet.

## v0.2 — Match Statistics and Form

Add:

- team match statistics;
- recent form;
- rolling metrics;
- basic Match Intelligence.

## v0.3 — Elo Baseline

Add:

- Elo ratings;
- historical backtest;
- first win/draw/loss prediction.

## v0.4 — Poisson / Dixon-Coles

Add:

- expected goals model;
- score probabilities;
- score distribution.

## v0.5 — Prediction Evaluation

Add:

- prediction dashboard;
- Brier Score;
- Log Loss;
- calibration;
- historical performance.

## v0.6 — Betting Intelligence

Add:

- odds snapshots;
- implied probability;
- normalized market probability;
- edge/value calculations;
- model-vs-market view.

## v0.7 — Match Notebook

Add:

- fast live note capture;
- tags;
- player associations;
- clip candidates;
- AI note grouping.

## v0.8 — Content Studio

Add:

- pre-match outline;
- post-match outline;
- predicted-vs-actual block;
- talking points;
- script versions.

## v0.9 — Tactical Studio

Add:

- pitch canvas;
- formations;
- player markers;
- arrows;
- zones;
- tactical scenes.

## v1.0 — Integrated Analyst Workflow

Combine:

- football data;
- predictions;
- simulations;
- betting intelligence;
- notes;
- tactics;
- AI;
- podcast production.

---

# 21. Phase Zero — First Task for Codex

If this repository is empty or mostly empty, DO NOT immediately scaffold the whole application.

First perform architecture and product initialization.

Create or refine:

1. system architecture;
2. bounded contexts/modules;
3. dependency rules;
4. initial domain model;
5. initial ER model;
6. major business invariants;
7. API boundaries;
8. provider integration design;
9. ML boundary;
10. local development architecture;
11. testing strategy;
12. deployment strategy;
13. security considerations;
14. observability strategy;
15. risks and assumptions;
16. ADR backlog;
17. phased implementation backlog.

Represent important architecture with Mermaid.

Create appropriate files under `/docs`.

Create/update `README.md`.

Do not generate large amounts of production code during Phase Zero.

After documenting Phase Zero, perform a self-review.

Explicitly ask:

- Is anything over-engineered?
- Are bounded contexts meaningful?
- Are responsibilities duplicated?
- Can the MVP be simpler?
- Is ML isolated correctly?
- Is provider coupling controlled?
- Can historical predictions be reproduced?
- Is temporal leakage prevented conceptually?
- Are AI-generated claims clearly separated from facts?
- Is the first vertical slice small enough to finish?

Correct problems before starting implementation.

---

# 22. Phase One — First Vertical Slice

After Phase Zero is coherent, implement:

**Football Data → Provider → PostgreSQL → .NET API → Next.js UI**

The first slice should prove:

- API-Football integration;
- canonical domain mapping;
- persistence;
- backend contract;
- frontend consumption;
- logging/observability;
- testing.

Prefer finishing this slice completely over scaffolding future modules.

---

# 23. Product-Specific Notes

This project is initially focused on Real Madrid, but avoid hardcoding Real Madrid into generic football-domain types.

Real Madrid can be the first configured team or default focus, but domain models should support other teams and competitions.

The product is intended to support analytical content rather than transfer rumors, gossip or shout-driven sports commentary.

The desired editorial style is:

- calm;
- tactical;
- evidence-driven;
- respectful;
- analytical;
- focused on movements, spaces, pressing, transitions, positioning and measurable performance.

The system should support analysis of:

- movements without the ball;
- central defenders being dragged;
- spaces created;
- line-breaking passes;
- pressing triggers;
- player duels;
- turnovers;
- tactical errors;
- formation effects.

The podcast production workflow should support concise pre-match and post-match episodes.

---

# 24. Final Engineering Rule

This repository must become easier to understand as it grows.

Every new abstraction, dependency, service and module must justify its existence.

If there is a conflict between:

- a fashionable architecture pattern;
- and a simpler design that satisfies the real requirements;

choose the simpler design unless there is a documented reason not to.

When working in this repository, act as a senior engineering team building a product intended to evolve for years.

Start with **Phase Zero** unless the repository already clearly indicates that Phase Zero has been completed.
