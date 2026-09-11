"use client";

import { useState } from "react";

type Player = { id: string; name: string; position: string };
type Slot = "Goalkeeper" | "Defender" | "Midfielder" | "Forward";

export function LineupBuilder({ squad }: { squad: Player[] }) {
  const [selected, setSelected] = useState<Record<string, Slot>>({});
  function toggle(player: Player) {
    setSelected((current) => {
      if (current[player.id]) { const next = { ...current }; delete next[player.id]; return next; }
      if (Object.keys(current).length === 11) return current;
      return { ...current, [player.id]: player.position as Slot };
    });
  }
  const chosen = squad.filter((player) => selected[player.id]);
  return <section><h2>Once previsto ({chosen.length}/11)</h2><p>Selecciona hasta once jugadores y ajusta su rol. La alerta indica una posición distinta a la natural registrada.</p><ul>{squad.map((player) => <li key={player.id}><label><input type="checkbox" checked={Boolean(selected[player.id])} onChange={() => toggle(player)} disabled={!selected[player.id] && chosen.length === 11} /> {player.name}</label>{selected[player.id] && <><select aria-label={`Posición de ${player.name}`} value={selected[player.id]} onChange={(event) => setSelected((current) => ({ ...current, [player.id]: event.target.value as Slot }))}>{(["Goalkeeper", "Defender", "Midfielder", "Forward"] as Slot[]).map((position) => <option key={position}>{position}</option>)}</select>{selected[player.id] !== player.position && <strong> · Fuera de posición: natural {player.position}</strong>}</>}</li>)}</ul></section>;
}
