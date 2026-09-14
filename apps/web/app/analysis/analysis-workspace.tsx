"use client";

import { useState } from "react";
import { MatchNotebook } from "./match-notebook";

type Fixture = { id: string; homeTeam: string; awayTeam: string; kickoffUtc: string | null };

export function AnalysisWorkspace({ fixtures }: { fixtures: Fixture[] }) {
  const [fixtureId, setFixtureId] = useState(fixtures[0]?.id ?? "");
  const activeFixture = fixtures.find((fixture) => fixture.id === fixtureId);

  if (fixtures.length === 0) return <section><h2>Sin partidos disponibles</h2><p>Actualiza los datos desde Match Center para comenzar el análisis.</p></section>;

  return <section>
    <label>Partido activo
      <select aria-label="Partido activo" value={fixtureId} onChange={(event) => setFixtureId(event.target.value)}>
        {fixtures.map((fixture) => <option key={fixture.id} value={fixture.id}>{fixture.homeTeam} — {fixture.awayTeam}{fixture.kickoffUtc ? ` · ${new Date(fixture.kickoffUtc).toLocaleDateString("es-MX")}` : ""}</option>)}
      </select>
    </label>
    <p className="muted">Analizando: <strong>{activeFixture?.homeTeam} — {activeFixture?.awayTeam}</strong></p>
    <MatchNotebook fixtureId={fixtureId} />
  </section>;
}
