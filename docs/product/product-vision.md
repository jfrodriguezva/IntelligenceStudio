# Product vision

MIS supports a football analyst who creates calm, tactical, evidence-based coverage for **Madrid, hagámoslo real**. Real Madrid is the first configured team, never a special case in generic football entities. Other teams and competitions must fit the same model. Admin, Analyst, Editor, and Viewer are intended roles; multi-user collaboration is future scope.

## Outcomes

The analyst should understand what happened, why it happened, what may happen next, and the uncertainty around those conclusions. They should be able to inspect supporting evidence, compare a frozen prediction with the eventual result, identify disagreement with a market, capture tactical patterns, and turn findings into concise pre-match and post-match episodes.

## Workflows

| Moment | Flow | Evidence retained |
|---|---|---|
| Pre-match | Choose fixture → sync → form metrics → feature snapshot → prediction/simulation → analysis → optional odds comparison → content outline | Source observations, cutoff, model/features versions, odds timestamps, generated-content references |
| Live | Capture timestamped notes with keyboard shortcuts → tag players/patterns/clip candidates | Original notes and subsequent explicit revisions |
| Post-match | Sync final facts → predicted vs actual → inspect notes/scenes → insights → episode outline | Original prediction, selected actual-data revision, notes, tactical scenes, script versions |

Facts, derived metrics, AI interpretations, and analyst opinions must have distinguishable labels and provenance. Missing data must remain missing, not become a zero or a fabricated statistic. The application does not place bets automatically. Editorial release remains a human decision.

## v0.1 scope

One configured focus team; explicitly selected competitions and seasons; import team and opponent identities, competitions, seasons, upcoming fixtures, and recent completed fixtures. Display a paginated fixture list and basic Match Center with participants, competition, kickoff, canonical status, available score, last successful observation, and freshness/error states. Admin can request a bounded sync and inspect its outcome.

Import dates are explicit UTC ranges, capped by configured limits. UI may offer a configurable recent/upcoming window. No implicit all-history crawl or automatic paid integration expansion. Competition selection and provider identity are validated configuration, not guessed numeric IDs.

Success means a real permitted provider response can reach the UI, a repeated import creates no duplicates, provider failure leaves prior data readable with truthful freshness, and an analyst can open a fixture without understanding internal provider details. Deterministic fixtures demonstrate this in CI; an opt-in provider smoke check proves account integration.

## Deferred scope

Statistics/form start in v0.2; Elo in v0.3; score models in v0.4; evaluation dashboard in v0.5; betting in v0.6; notebook in v0.7; content in v0.8; tactics in v0.9; integrated workflow in v1.0. Baseline evaluation and temporal tests start with the first model, not with the later dashboard. No live polling promise, rich player ingestion, generated content, odds feed, or ML runtime deployment is needed to finish v0.1.

## Product decisions still required

Before account-backed imports, establish allowed competitions/seasons, date window, available subscription coverage, storage/redistribution rights, and quota budget. Before shared hosting, choose identity provider and exposure model. These are tracked with implementation gates in the [risk register](risks-and-assumptions.md).
