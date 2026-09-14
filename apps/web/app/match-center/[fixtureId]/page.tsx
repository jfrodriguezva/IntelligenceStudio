import Link from "next/link";
import { notFound } from "next/navigation";
import { PredictionPanel } from "./prediction-panel";

type Fixture = { id: string; competition: string; season: string; homeTeamId: string; homeTeam: string; awayTeamId: string; awayTeam: string; kickoffUtc: string | null; status: string; homeGoals: number | null; awayGoals: number | null };
type TeamForm = { team: string; played: number; won: number; drawn: number; lost: number; goalsFor: number; goalsAgainst: number; points: number };
type TeamStatistic = { team: string; possessionPercent: number | null; shots: number | null; shotsOnTarget: number | null; corners: number | null };
type SquadPlayer = { id: string; name: string; position: string };
type MatchEvent = { type: string; minute: number; player: string | null; note: string | null };
type Prediction = { modelVersion: string; dataCutoff: string; homeWin: number; draw: number; awayWin: number; generatedAt: string };

async function getFixture(fixtureId: string): Promise<Fixture | null> {
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const response = await fetch(`${origin}/api/v1/fixtures/${fixtureId}`, { cache: "no-store" });
  return response.ok ? response.json() : null;
}

export default async function FixtureDetailPage({ params }: { params: Promise<{ fixtureId: string }> }) {
  const fixtureId = (await params).fixtureId;
  const fixture = await getFixture(fixtureId);
  if (!fixture) notFound();
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const madridTeamId = fixture.homeTeam === "Real Madrid" ? fixture.homeTeamId : fixture.awayTeamId;
  const [formResponse, statisticsResponse, squadResponse, eventsResponse, predictionResponse] = await Promise.all([fetch(`${origin}/api/v1/fixtures/${fixtureId}/form`, { cache: "no-store" }), fetch(`${origin}/api/v1/fixtures/${fixtureId}/statistics`, { cache: "no-store" }), fetch(`${origin}/api/v1/teams/${madridTeamId}/squad`, { cache: "no-store" }), fetch(`${origin}/api/v1/fixtures/${fixtureId}/events`, { cache: "no-store" }), fetch(`${origin}/api/v1/fixtures/${fixtureId}/predictions/latest`, { cache: "no-store" })]);
  const form: TeamForm[] = formResponse.ok ? await formResponse.json() : [];
  const statistics: TeamStatistic[] = statisticsResponse.ok ? await statisticsResponse.json() : [];
  const squad: SquadPlayer[] = squadResponse.ok ? await squadResponse.json() : [];
  const events: MatchEvent[] = eventsResponse.ok ? await eventsResponse.json() : [];
  const prediction: Prediction | null = predictionResponse.ok ? await predictionResponse.json() : null;
  return <main>
    <p><Link href="/match-center">← Match Center</Link></p>
    <p className="eyebrow">{fixture.competition} · Temporada {fixture.season}</p>
    <h1>{fixture.homeTeam} {fixture.homeGoals ?? ""} — {fixture.awayGoals ?? ""} {fixture.awayTeam}</h1>
    <p>{fixture.kickoffUtc ? new Intl.DateTimeFormat("es-MX", { dateStyle: "full", timeStyle: "short" }).format(new Date(fixture.kickoffUtc)) : "Fecha pendiente"}</p>
    <p>Estado: {fixture.status}</p>
    <h2>Predicho vs. real</h2>{prediction ? <section><p><strong>{prediction.modelVersion}</strong> · generado {new Intl.DateTimeFormat("es-MX", { dateStyle: "short", timeStyle: "short" }).format(new Date(prediction.generatedAt))}</p><ul><li>Victoria {fixture.homeTeam}: {(prediction.homeWin * 100).toFixed(1)}%</li><li>Empate: {(prediction.draw * 100).toFixed(1)}%</li><li>Victoria {fixture.awayTeam}: {(prediction.awayWin * 100).toFixed(1)}%</li></ul>{fixture.homeGoals !== null && fixture.awayGoals !== null ? <p>Resultado real: <strong>{fixture.homeTeam} {fixture.homeGoals} — {fixture.awayGoals} {fixture.awayTeam}</strong></p> : <p>El resultado real se mostrará cuando el partido finalice.</p>}</section> : <><p>Aún no existe una predicción guardada para este partido.</p><PredictionPanel fixtureId={fixtureId} /></>}
    <h2>Forma reciente</h2><ul>{form.map((team) => <li key={team.team}><strong>{team.team}</strong>: {team.played} partidos · {team.won}G {team.drawn}E {team.lost}P · {team.goalsFor}-{team.goalsAgainst} · {team.points} puntos</li>)}</ul>
    <h2>Estadísticas del partido</h2><ul>{statistics.map((team) => <li key={team.team}><strong>{team.team}</strong>: posesión {team.possessionPercent ?? "—"}% · tiros {team.shots ?? "—"} · a puerta {team.shotsOnTarget ?? "—"} · córners {team.corners ?? "—"}</li>)}</ul>
    <h2>Línea de tiempo</h2>{events.length === 0 ? <p>Sin eventos importados todavía.</p> : <ul>{events.map((event, index) => <li key={`${event.minute}-${event.type}-${index}`}><strong>{event.minute}' · {event.type}</strong>{event.player ? ` · ${event.player}` : ""}{event.note ? ` · ${event.note}` : ""}</li>)}</ul>}
    <h2>Plantilla disponible</h2>{squad.length === 0 ? <p>La plantilla aún no está disponible en el proveedor.</p> : <ul>{squad.map((player) => <li key={player.id}>{player.name} · {player.position}</li>)}</ul>}
  </main>;
}
