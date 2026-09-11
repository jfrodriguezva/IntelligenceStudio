import { MatchNotebook } from "./match-notebook";

type Fixture = { id: string; homeTeam: string; awayTeam: string; kickoffUtc: string | null };
type FixturePage = { items: Fixture[] };

export default function AnalysisPage() {
  return <AnalysisContent />;
}

async function AnalysisContent() {
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const response = await fetch(`${origin}/api/v1/fixtures?page=1&pageSize=10`, { cache: "no-store" });
  const result: FixturePage = response.ok ? await response.json() : { items: [] };
  return <main><p className="eyebrow">Análisis</p><h1>Previa y post partido</h1><p>Registra el contexto que no aparece en la estadística: riesgos, zonas, impacto y decisiones tácticas.</p><label>Partido activo<select aria-label="Partido activo">{result.items.map((fixture) => <option key={fixture.id}>{fixture.homeTeam} — {fixture.awayTeam}</option>)}</select></label><MatchNotebook fixtureId={result.items[0]?.id} /></main>;
}
