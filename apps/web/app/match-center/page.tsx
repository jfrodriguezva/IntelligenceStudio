import Link from "next/link";
import { RefreshFixtures } from "./refresh-fixtures";

type Fixture = { id: string; competition: string; season: string; homeTeamId: string; homeTeam: string; awayTeamId: string; awayTeam: string; kickoffUtc: string | null; status: string; homeGoals: number | null; awayGoals: number | null };
type SyncRun = { id: string; provider: string; status: string; requestedAt: string; completedAt: string | null; importedFixtureCount: number; errorCode: string | null };

async function getFixtures(): Promise<Fixture[]> {
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const response = await fetch(`${origin}/api/v1/fixtures`, { cache: "no-store" });
  return response.ok ? response.json() : [];
}

async function getSyncRuns(): Promise<SyncRun[]> {
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const response = await fetch(`${origin}/api/v1/acquisition/sync-runs?take=5`, { cache: "no-store" });
  return response.ok ? response.json() : [];
}

export default async function MatchCenterPage({ searchParams }: { searchParams: Promise<{ status?: string; competition?: string }> }) {
  const [fixtures, syncRuns, filters] = await Promise.all([getFixtures(), getSyncRuns(), searchParams]);
  const competitions = [...new Set(fixtures.map((fixture) => fixture.competition))].sort();
  const filtered = fixtures.filter((fixture) =>
    (!filters.status || fixture.status === filters.status) && (!filters.competition || fixture.competition === filters.competition));

  return <main>
    <p className="eyebrow">Match Center</p>
    <h1>Partidos</h1>
    <nav className="filters"><Link href="/match-center">Todos</Link>{["Finished", "Scheduled", "Live"].map((status) => <Link key={status} href={`/match-center?status=${status}`}>{status}</Link>)}</nav>
    <nav className="filters">{competitions.map((competition) => <Link key={competition} href={`/match-center?competition=${encodeURIComponent(competition)}`}>{competition}</Link>)}</nav>
    <p>{filtered.length} partidos disponibles.</p>
    <RefreshFixtures />
    <section aria-labelledby="sync-history"><h2 id="sync-history">Historial de sincronizaciones</h2>{syncRuns.length === 0 ? <p>Aún no hay sincronizaciones registradas.</p> : <ul>{syncRuns.map((run) => <li key={run.id}><strong>{run.status}</strong> · {run.importedFixtureCount} fixtures · {new Intl.DateTimeFormat("es-MX", { dateStyle: "medium", timeStyle: "short" }).format(new Date(run.requestedAt))}{run.errorCode ? ` · Error: ${run.errorCode}` : ""}</li>)}</ul>}</section>
    <ul>{filtered.map((fixture) => <li key={fixture.id}><Link href={`/match-center/${fixture.id}`}><strong>{fixture.homeTeam} {fixture.homeGoals ?? ""} — {fixture.awayGoals ?? ""} {fixture.awayTeam}</strong><br />{fixture.competition} · {fixture.kickoffUtc ? new Intl.DateTimeFormat("es-MX", { dateStyle: "medium", timeStyle: "short" }).format(new Date(fixture.kickoffUtc)) : "Fecha pendiente"} · {fixture.status}</Link></li>)}</ul>
  </main>;
}
