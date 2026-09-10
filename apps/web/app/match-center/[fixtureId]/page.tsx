import Link from "next/link";
import { notFound } from "next/navigation";

type Fixture = { id: string; competition: string; season: string; homeTeam: string; awayTeam: string; kickoffUtc: string | null; status: string; homeGoals: number | null; awayGoals: number | null };

async function getFixture(fixtureId: string): Promise<Fixture | null> {
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const response = await fetch(`${origin}/api/v1/fixtures/${fixtureId}`, { cache: "no-store" });
  return response.ok ? response.json() : null;
}

export default async function FixtureDetailPage({ params }: { params: Promise<{ fixtureId: string }> }) {
  const fixture = await getFixture((await params).fixtureId);
  if (!fixture) notFound();
  return <main>
    <p><Link href="/match-center">← Match Center</Link></p>
    <p className="eyebrow">{fixture.competition} · Temporada {fixture.season}</p>
    <h1>{fixture.homeTeam} {fixture.homeGoals ?? ""} — {fixture.awayGoals ?? ""} {fixture.awayTeam}</h1>
    <p>{fixture.kickoffUtc ? new Intl.DateTimeFormat("es-MX", { dateStyle: "full", timeStyle: "short" }).format(new Date(fixture.kickoffUtc)) : "Fecha pendiente"}</p>
    <p>Estado: {fixture.status}</p>
  </main>;
}
