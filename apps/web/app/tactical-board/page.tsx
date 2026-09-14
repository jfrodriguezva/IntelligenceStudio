import { LineupBuilder } from "./lineup-builder";

type Fixture = { id: string; homeTeam: string; homeTeamId: string; awayTeam: string; awayTeamId: string };
type FixturePage = { items: Fixture[] };
type Player = { id: string; name: string; position: string };

export default async function TacticalBoardPage() {
  const origin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";
  const fixtureResponse = await fetch(`${origin}/api/v1/fixtures?page=1&pageSize=20`, { cache: "no-store" });
  const fixtures: FixturePage = fixtureResponse.ok ? await fixtureResponse.json() : { items: [] };
  const fixture = fixtures.items.find((item) => item.homeTeam === "Real Madrid" || item.awayTeam === "Real Madrid");
  const teamId = fixture?.homeTeam === "Real Madrid" ? fixture.homeTeamId : fixture?.awayTeamId;
  const squadResponse = teamId ? await fetch(`${origin}/api/v1/teams/${teamId}/squad`, { cache: "no-store" }) : null;
  const squad: Player[] = squadResponse?.ok ? await squadResponse.json() : [];
  return <main><p className="eyebrow">Pizarra táctica</p><h1>Once inicial</h1>{squad.length === 0 || !fixture ? <p>Actualiza los datos para cargar la plantilla.</p> : <LineupBuilder fixtureId={fixture.id} squad={squad} />}</main>;
}
