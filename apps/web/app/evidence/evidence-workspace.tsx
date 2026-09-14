"use client";

import { useState } from "react";
import { EvidenceQueue } from "./evidence-queue";

type Fixture = { id: string; homeTeam: string; awayTeam: string; kickoffUtc: string | null };

export function EvidenceWorkspace({ fixtures }: { fixtures: Fixture[] }) {
  const [fixtureId, setFixtureId] = useState(fixtures[0]?.id ?? "");
  if (fixtures.length === 0) return <section><h2>Sin partidos disponibles</h2><p>Actualiza los datos desde Match Center para registrar evidencia.</p></section>;
  return <section><label>Partido asociado<select aria-label="Partido asociado" value={fixtureId} onChange={(event) => setFixtureId(event.target.value)}>{fixtures.map((fixture) => <option key={fixture.id} value={fixture.id}>{fixture.homeTeam} — {fixture.awayTeam}{fixture.kickoffUtc ? ` · ${new Date(fixture.kickoffUtc).toLocaleDateString("es-MX")}` : ""}</option>)}</select></label><EvidenceQueue fixtureId={fixtureId} /></section>;
}
