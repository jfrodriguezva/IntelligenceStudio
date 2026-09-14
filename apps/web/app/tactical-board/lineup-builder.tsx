"use client";

import { useEffect, useState } from "react";

type Player = { id: string; name: string; position: string };
type Slot = "Goalkeeper" | "Defender" | "Midfielder" | "Forward";
type PlayerMarker = { role: Slot; x: number; y: number };
type Scene = { id: string; title: string; stateJson: string; createdAt: string };
type SceneState = { schemaVersion: 1; players: Array<{ playerId: string; role: Slot; x: number; y: number }> };
const roles: Slot[] = ["Goalkeeper", "Defender", "Midfielder", "Forward"];

function initialMarker(role: Slot, current: Record<string, PlayerMarker>): PlayerMarker {
  const sameRole = Object.values(current).filter((marker) => marker.role === role).length;
  const coordinates: Record<Slot, Array<[number, number]>> = {
    Goalkeeper: [[50, 91]], Defender: [[18, 72], [39, 76], [61, 76], [82, 72]],
    Midfielder: [[24, 49], [42, 55], [58, 55], [76, 49]], Forward: [[29, 27], [50, 20], [71, 27]],
  };
  const [x, y] = coordinates[role][sameRole % coordinates[role].length];
  return { role, x: x / 100, y: y / 100 };
}

export function LineupBuilder({ fixtureId, squad }: { fixtureId: string; squad: Player[] }) {
  const [selected, setSelected] = useState<Record<string, PlayerMarker>>({});
  const [key, setKey] = useState("");
  const [title, setTitle] = useState("Once previsto");
  const [scenes, setScenes] = useState<Scene[]>([]);
  const [status, setStatus] = useState("");
  const [loading, setLoading] = useState(false);
  const chosen = squad.filter((player) => selected[player.id]);

  useEffect(() => { setScenes([]); setStatus(""); }, [fixtureId]);

  function toggle(player: Player) {
    setSelected((current) => {
      if (current[player.id]) { const next = { ...current }; delete next[player.id]; return next; }
      if (Object.keys(current).length === 11) return current;
      return { ...current, [player.id]: initialMarker(player.position as Slot, current) };
    });
  }

  function update(playerId: string, patch: Partial<PlayerMarker>) {
    setSelected((current) => ({ ...current, [playerId]: { ...current[playerId], ...patch } }));
  }

  async function loadScenes() {
    if (!key) { setStatus("Escribe la clave administrativa para consultar las escenas."); return; }
    setLoading(true); setStatus("");
    try {
      const response = await fetch(`/api/v1/fixtures/${fixtureId}/tactical-scenes`, { headers: { "X-MIS-Admin-Key": key } });
      if (!response.ok) { setStatus(response.status === 401 ? "La clave administrativa no es válida." : "No fue posible consultar las escenas."); return; }
      setScenes(await response.json());
    } catch { setStatus("No se pudo conectar con el servicio táctico."); }
    finally { setLoading(false); }
  }

  async function saveScene() {
    if (!key || !title.trim()) { setStatus("Escribe la clave administrativa y un nombre para la escena."); return; }
    const state: SceneState = { schemaVersion: 1, players: Object.entries(selected).map(([playerId, marker]) => ({ playerId, ...marker })) };
    setLoading(true); setStatus("");
    try {
      const response = await fetch(`/api/v1/fixtures/${fixtureId}/tactical-scenes`, { method: "POST", headers: { "Content-Type": "application/json", "X-MIS-Admin-Key": key }, body: JSON.stringify({ title: title.trim(), stateJson: JSON.stringify(state) }) });
      if (!response.ok) { setStatus(response.status === 401 ? "La clave administrativa no es válida." : "No fue posible guardar la escena."); return; }
      const created: Scene = await response.json(); setScenes((current) => [created, ...current]); setStatus("Escena guardada como versión independiente.");
    } catch { setStatus("No se pudo conectar con el servicio táctico."); }
    finally { setLoading(false); }
  }

  function openScene(scene: Scene) {
    try {
      const state = JSON.parse(scene.stateJson) as SceneState;
      const markers = Object.fromEntries(state.players.map((player) => [player.playerId, { role: player.role, x: player.x, y: player.y }]));
      setSelected(markers); setTitle(scene.title); setStatus(`Escena \"${scene.title}\" cargada.`);
    } catch { setStatus("La escena guardada no tiene un formato compatible."); }
  }

  return <section><h2>Once previsto ({chosen.length}/11)</h2><p>Selecciona hasta once jugadores, ajusta sus zonas y guarda una escena. Las coordenadas se conservan normalizadas para que el dibujo mantenga su posición en cualquier tamaño de pantalla.</p><div className="filters"><input aria-label="Clave administrativa" type="password" placeholder="Clave administrativa" value={key} onChange={(event) => setKey(event.target.value)} /><button type="button" onClick={loadScenes} disabled={!key || loading}>{loading ? "Consultando…" : "Cargar escenas"}</button><input aria-label="Nombre de escena" value={title} onChange={(event) => setTitle(event.target.value)} placeholder="Nombre de escena" /><button type="button" onClick={saveScene} disabled={!key || !title.trim() || loading}>Guardar escena</button></div>{status && <p role="status">{status}</p>}<div className="tactical-layout"><div className="pitch" aria-label="Campo táctico">{chosen.map((player) => { const marker = selected[player.id]; return <span className="player-marker" key={player.id} style={{ left: `${marker.x * 100}%`, top: `${marker.y * 100}%` }} title={`${player.name} · ${marker.role}`}>{player.name.split(" ").at(-1)}</span>; })}</div><div><h3>Jugadores y zonas</h3><ul className="compact-list">{squad.map((player) => { const marker = selected[player.id]; return <li key={player.id}><label><input type="checkbox" checked={Boolean(marker)} onChange={() => toggle(player)} disabled={!marker && chosen.length === 11} /> {player.name}</label>{marker && <div className="marker-controls"><select aria-label={`Posición de ${player.name}`} value={marker.role} onChange={(event) => update(player.id, { role: event.target.value as Slot })}>{roles.map((role) => <option key={role}>{role}</option>)}</select><label>X<input aria-label={`Coordenada horizontal de ${player.name}`} max="100" min="0" type="range" value={Math.round(marker.x * 100)} onChange={(event) => update(player.id, { x: Number(event.target.value) / 100 })} /></label><label>Y<input aria-label={`Coordenada vertical de ${player.name}`} max="100" min="0" type="range" value={Math.round(marker.y * 100)} onChange={(event) => update(player.id, { y: Number(event.target.value) / 100 })} /></label>{marker.role !== player.position && <strong>Fuera de posición: natural {player.position}</strong>}</div>}</li>; })}</ul></div></div>{scenes.length > 0 && <section><h3>Escenas guardadas</h3><ul>{scenes.map((scene) => <li key={scene.id}><button type="button" onClick={() => openScene(scene)}>{scene.title}</button> · {new Date(scene.createdAt).toLocaleString("es-MX")}</li>)}</ul></section>}</section>;
}
