type Fixture = { id: string; competition: string; season: string; homeTeam: string; awayTeam: string; kickoffUtc: string | null; status: string; homeGoals: number | null; awayGoals: number | null };

async function getFixtures(): Promise<Fixture[]> {
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const response = await fetch(`${origin}/api/v1/fixtures`, { cache: "no-store" });
  return response.ok ? response.json() : [];
}

export default async function HomePage() {
  const fixtures = await getFixtures();
  return (
    <main>
      <p className="eyebrow">Madrid, hagámoslo real</p>
      <h1>Madrid Intelligence Studio</h1>
      <p>Partidos recientes y próximos del Match Center.</p>
      {fixtures.length === 0 ? <p>No hay fixtures disponibles todavía.</p> : <ul>{fixtures.map((fixture) => <li key={fixture.id}><strong>{fixture.homeTeam} {fixture.homeGoals ?? ""} — {fixture.awayGoals ?? ""} {fixture.awayTeam}</strong><br />{fixture.competition} · {fixture.kickoffUtc ? new Intl.DateTimeFormat("es-MX", { dateStyle: "medium", timeStyle: "short" }).format(new Date(fixture.kickoffUtc)) : "Fecha pendiente"} · {fixture.status}</li>)}</ul>}
    </main>
  );
}
