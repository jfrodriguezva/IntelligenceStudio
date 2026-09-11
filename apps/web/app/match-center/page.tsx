import Link from "next/link";
import { RefreshFixtures } from "./refresh-fixtures";

type Fixture = { id: string; competition: string; season: string; homeTeamId: string; homeTeam: string; awayTeamId: string; awayTeam: string; kickoffUtc: string | null; status: string; homeGoals: number | null; awayGoals: number | null };
type FixturePage = { items: Fixture[]; totalCount: number; page: number; pageSize: number };
type SyncRun = { id: string; provider: string; status: string; requestedAt: string; completedAt: string | null; importedFixtureCount: number; errorCode: string | null };
type Filters = { competition?: string; season?: string; status?: string; from?: string; to?: string; page?: string };

async function getFixtures(filters: Filters): Promise<FixturePage> {
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const parameters = new URLSearchParams({ page: filters.page ?? "1", pageSize: "10" });
  for (const [key, value] of Object.entries(filters)) if (value && key !== "page") parameters.set(key, value);
  const response = await fetch(`${origin}/api/v1/fixtures?${parameters}`, { cache: "no-store" });
  return response.ok ? response.json() : { items: [], totalCount: 0, page: 1, pageSize: 10 };
}

async function getSyncRuns(): Promise<SyncRun[]> {
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const response = await fetch(`${origin}/api/v1/acquisition/sync-runs?take=5`, { cache: "no-store" });
  return response.ok ? response.json() : [];
}

function pageHref(filters: Filters, page: number) {
  const parameters = new URLSearchParams();
  for (const [key, value] of Object.entries({ ...filters, page: String(page) })) if (value) parameters.set(key, value);
  return `/match-center?${parameters}`;
}

export default async function MatchCenterPage({ searchParams }: { searchParams: Promise<Filters> }) {
  const filters = await searchParams;
  const [result, syncRuns] = await Promise.all([getFixtures(filters), getSyncRuns()]);
  const competitions = [...new Set(result.items.map((fixture) => fixture.competition))].sort();
  const seasons = [...new Set(result.items.map((fixture) => fixture.season))].sort().reverse();
  const totalPages = Math.max(1, Math.ceil(result.totalCount / result.pageSize));

  return <main>
    <p className="eyebrow">Match Center</p>
    <h1>Partidos</h1>
    <form className="filters" method="get"><select aria-label="Competición" name="competition" defaultValue={filters.competition ?? ""}><option value="">Todas las competiciones</option>{competitions.map((competition) => <option key={competition} value={competition}>{competition}</option>)}</select><select aria-label="Temporada" name="season" defaultValue={filters.season ?? ""}><option value="">Todas las temporadas</option>{seasons.map((season) => <option key={season} value={season}>{season}</option>)}</select><select aria-label="Estado" name="status" defaultValue={filters.status ?? ""}><option value="">Todos los estados</option>{["Finished", "Scheduled", "Live"].map((status) => <option key={status} value={status}>{status}</option>)}</select><input aria-label="Desde" name="from" type="date" defaultValue={filters.from} /><input aria-label="Hasta" name="to" type="date" defaultValue={filters.to} /><button type="submit">Filtrar</button><Link href="/match-center">Limpiar</Link></form>
    <p>{result.totalCount} partidos encontrados.</p>
    <RefreshFixtures />
    <section aria-labelledby="sync-history"><h2 id="sync-history">Historial de sincronizaciones</h2>{syncRuns.length === 0 ? <p>Aún no hay sincronizaciones registradas.</p> : <ul>{syncRuns.map((run) => <li key={run.id}><strong>{run.status}</strong> · {run.importedFixtureCount} fixtures · {new Intl.DateTimeFormat("es-MX", { dateStyle: "medium", timeStyle: "short" }).format(new Date(run.requestedAt))}{run.errorCode ? ` · Error: ${run.errorCode}` : ""}</li>)}</ul>}</section>
    {result.items.length === 0 ? <p>No hay partidos con esos filtros.</p> : <ul>{result.items.map((fixture) => <li key={fixture.id}><Link href={`/match-center/${fixture.id}`}><strong>{fixture.homeTeam} {fixture.homeGoals ?? ""} — {fixture.awayGoals ?? ""} {fixture.awayTeam}</strong><br />{fixture.competition} · {fixture.kickoffUtc ? new Intl.DateTimeFormat("es-MX", { dateStyle: "medium", timeStyle: "short" }).format(new Date(fixture.kickoffUtc)) : "Fecha pendiente"} · {fixture.status}</Link></li>)}</ul>}
    <nav className="filters" aria-label="Paginación">{result.page > 1 && <Link href={pageHref(filters, result.page - 1)}>← Anterior</Link>}<span>Página {result.page} de {totalPages}</span>{result.page < totalPages && <Link href={pageHref(filters, result.page + 1)}>Siguiente →</Link>}</nav>
  </main>;
}
