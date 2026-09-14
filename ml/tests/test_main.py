from fastapi.testclient import TestClient

from app.main import app


client = TestClient(app)


def test_health_contract() -> None:
    response = client.get("/health")

    assert response.status_code == 200
    assert response.json() == {"status": "ok", "service": "mis-ml"}


def test_baseline_prediction_is_a_three_way_distribution() -> None:
    response = client.post("/v1/predictions/baseline", json={"home_strength": 2.0, "away_strength": 1.0})

    assert response.status_code == 200
    payload = response.json()
    assert payload["model"] == "strength-baseline-v1"
    assert payload["home_win"] > payload["away_win"]
    assert abs(payload["home_win"] + payload["draw"] + payload["away_win"] - 1) < 0.0001


def test_baseline_rejects_negative_strength() -> None:
    response = client.post("/v1/predictions/baseline", json={"home_strength": -1, "away_strength": 1})

    assert response.status_code == 422
