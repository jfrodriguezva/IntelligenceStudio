# Ubiquitous language

| Term | Meaning |
|---|---|
| Fixture | Scheduled or played football match; identity survives schedule/status corrections |
| Competition season | A season belonging to one competition; display label need not be a single year |
| Focus team | Configured analysis target, initially Real Madrid |
| Canonical ID | MIS-generated UUID, independent of provider identifiers |
| Provider mapping | Typed relationship between a provider's external ID and a canonical entity |
| Observation | Immutable accepted view of a source record with when MIS received and accepted it |
| Raw payload | Retained provider response, subject to rights and retention limits |
| Fact | Attributed provider observation; may be corrected by a later observation |
| Derived metric | Deterministic calculation with input references and formula version |
| AI interpretation | Generated language grounded in references; never a factual statistic by itself |
| Analyst opinion | Human judgment recorded separately from quantitative output |
| Feature snapshot | Immutable model-ready values, definitions, evidence references, and cutoff |
| Input data cutoff | Latest permitted knowledge time for a prediction's inputs |
| Event time | When the represented football event occurred or is scheduled to occur |
| Available at | When an observation became available to MIS; not the historical event time |
| Prediction run | Mutable operational lifecycle for a computation request |
| Match prediction | Immutable successful numerical result linked to run, inputs, and model |
| Model version | Immutable identity of model code/configuration/artifact and training metadata |
| Confidence | Versioned diagnostic about evidence/model reliability; not the win probability |
| Data quality | Explicit completeness, freshness, coverage, and provenance diagnostics |
| Expected goals | A model's expected goal count; distinct from a provider's observed shot-based xG |
| Odds snapshot | Bookmaker prices for an identified market and period at a recorded time |
| Value assessment | Reproducible comparison of a specific prediction and odds snapshot |
| Predicted vs actual | Original prediction compared with a specific final-fact observation |
| Tactical scene | Versioned pitch state with geometry normalized to [0,1] |
| Match note | Original timestamped human observation, with append-only corrections if needed |

Default modeled result market is home/draw/away at regulation time including stoppage time, excluding extra time and penalties. Store period semantics explicitly; never infer them from a final score or bookmaker label.
