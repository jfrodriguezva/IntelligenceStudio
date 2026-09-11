from fastapi import FastAPI
from pydantic import BaseModel, Field

app = FastAPI(title="MIS ML Runtime", version="0.1.0")


class MatchFeatures(BaseModel):
    home_strength: float = Field(ge=0)
    away_strength: float = Field(ge=0)


class MatchProbabilities(BaseModel):
    home_win: float
    draw: float
    away_win: float
    model: str


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok", "service": "mis-ml"}


@app.post("/v1/predictions/baseline", response_model=MatchProbabilities)
def baseline_prediction(features: MatchFeatures) -> MatchProbabilities:
    total = features.home_strength + features.away_strength
    if total == 0:
        return MatchProbabilities(home_win=0.35, draw=0.30, away_win=0.35, model="strength-baseline-v1")
    decisive_mass = 0.70
    return MatchProbabilities(home_win=round(decisive_mass * features.home_strength / total, 4), draw=0.30, away_win=round(decisive_mass * features.away_strength / total, 4), model="strength-baseline-v1")
