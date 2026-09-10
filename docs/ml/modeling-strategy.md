# Modeling strategy

Build interpretable baselines before complex models. Focus on honest probabilities and evidence quality, not a headline accuracy percentage. Real Madrid may be the product focus, but rating/training data must include an adequate connected population of opponents and competitions; a handful of Madrid matches is insufficient for reliable model fitting.

| Stage | Model and output | Required validation before use |
|---|---|---|
| v0.3 | Elo strength plus explicitly fitted/documented three-way outcome mapping | Naive baseline comparison; chronological backtest; draw modeling; Brier/Log Loss and basic calibration |
| v0.4 | Poisson followed by Dixon-Coles if justified; expected goal counts and score distribution | Held-out likelihood, goal/score diagnostics, low-score dependence check, tail accounting |
| Later | Recent-form alternative and gradient boosting with scikit-learn/XGBoost or LightGBM | Incremental out-of-time improvement with ablations and coverage checks |
| Later | Ensemble | Preserve component outputs/weights; train weights on earlier validation periods; independent final evaluation |

Elo does not directly supply a calibrated home/draw/away distribution or expected goals. Specify the draw/home-advantage mapping and fit parameters using only earlier training data. Expected goal fields remain explicitly unsupported/null for Elo until a goal model exists. Never substitute observed xG for a model's goal expectation.

## Evaluation protocol

Order examples by prediction knowledge cutoff, split training → validation/calibration → final test chronologically, and use rolling-origin backtests. Fit imputation, scaling, feature selection, hyperparameters, calibration and ensemble weights inside training/validation windows. Keep final test untouched. Record dataset manifest, cutoff, competition coverage, seed, metric definitions and model version.

Use multiclass Log Loss (natural log, documented numerical clipping epsilon) and multiclass Brier Score (sum of squared error over three classes, then average across fixtures; no division by three unless clearly labeled). Report reliability plots/bin counts, sample sizes, predicted-vs-observed frequencies, competition and home/away breakdowns. Compare against a simple historical-prior baseline learned from training data. Promotion thresholds are set before held-out evaluation; do not retroactively choose them to favor a model.

v0.5 adds the user dashboard, historical comparisons and drift monitoring. Numerical evaluation and leakage protection are release requirements starting in v0.3. A confidence diagnostic explains sample size, model disagreement, feature coverage and freshness; it is not a probability of being correct. Insufficient evidence can yield no prediction.

## Simulation

Use exact score enumeration where practical or seeded Monte Carlo. Record the algorithm/version, distribution assumptions, seed, sample count or truncation threshold and residual tail mass. Derive home/draw/away, exact scores, over/under, both-to-score and goal difference from consistent regulation-time semantics. Test distribution mass, finite values, expected goals and reproducibility tolerance. Present simulation uncertainty; additional decimal places do not imply certainty.

## Explainability and bias

Show component disagreement and data-quality limitations alongside output. Introduce SHAP only for a model that benefits from it. Keep tactical judgments and analyst predictions separate from learned probabilities. Odds comparisons do not retrain or overwrite historical outputs. Retraining produces a new model version and new predictions; old versions/results remain addressable.
