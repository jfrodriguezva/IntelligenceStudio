import { EvidenceWorkspace } from "./evidence-workspace";

type Fixture = { id: string; homeTeam: string; awayTeam: string; kickoffUtc: string | null };
type FixturePage = { items: Fixture[] };

export default async function EvidencePage() {
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const response = await fetch(`${origin}/api/v1/fixtures?page=1&pageSize=20`, { cache: "no-store" });
  const result: FixturePage = response.ok ? await response.json() : { items: [] };
  return <main><p className="eyebrow">Centro de evidencia</p><h1>Clips, imágenes y notas</h1><p>Reúne referencias de edición para localizarlas por minuto, jugada y contexto táctico.</p><EvidenceWorkspace fixtures={result.items} /></main>;
}
