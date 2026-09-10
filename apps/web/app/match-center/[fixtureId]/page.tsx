import Link from "next/link";
import { notFound } from "next/navigation";

type Fixture = { id: string; competition: string; season: string; homeTeam: string; awayTeam: string; kickoffUtc: string | null; status: string; homeGoals: number | null; awayGoals: number | null };
type TeamForm = { team: string; played: number; won: number; drawn: number; lost: number; goalsFor: number; goalsAgainst: number; points: number };

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
  const formResponse = await fetch(`${origin}/api/v1/fixtures/${fixtureId}/form`, { cache: "no-store" });
  const form: TeamForm[] = formResponse.ok ? await formResponse.json() : [];
  return <main>
    <p><Link href="/match-center">← Match Center</Link></p>
    <p className="eyebrow">{fixture.competition} · Temporada {fixture.season}</p>
    <h1>{fixture.homeTeam} {fixture.homeGoals ?? ""} — {fixture.awayGoals ?? ""} {fixture.awayTeam}</h1>
    <p>{fixture.kickoffUtc ? new Intl.DateTimeFormat("es-MX", { dateStyle: "full", timeStyle: "short" }).format(new Date(fixture.kickoffUtc)) : "Fecha pendiente"}</p>
    <p>Estado: {fixture.status}</p>
    <h2>Forma reciente</h2><ul>{form.map((team) => <li key={team.team}><strong>{team.team}</strong>: {team.played} partidos · {team.won}G {team.drawn}E {team.lost}P · {team.goalsFor}-{team.goalsAgainst} · {team.points} puntos</li>)}</ul>
  </main>;
}
