"use client";

import { FormEvent, useEffect, useState } from "react";

const tags = ["PRESSING", "BUILDUP", "ATTACK", "DEFENSE", "TRANSITION", "SET_PIECE", "INDIVIDUAL", "ERROR", "TACTICAL", "GOAL", "CHANCE", "CLIP"];
type Note = { id: string; minute: number; tag: string; text: string; capturedAt: string };

export function MatchNotebook({ fixtureId }: { fixtureId?: string }) {
  const [notes, setNotes] = useState<Note[]>([]);
  const [key, setKey] = useState("");
  const [minute, setMinute] = useState("");
  const [tag, setTag] = useState(tags[0]);
  const [text, setText] = useState("");
  const [status, setStatus] = useState("");
  const [loading, setLoading] = useState(false);
  const origin = process.env.NEXT_PUBLIC_MIS_API_ORIGIN ?? "http://localhost:5080";

  useEffect(() => { setNotes([]); setStatus(""); }, [fixtureId]);

  async function loadNotes() {
    if (!fixtureId || !key) { setStatus("Escribe la clave administrativa para consultar la libreta."); return; }
    setLoading(true); setStatus("");
    try {
      const response = await fetch(`${origin}/api/v1/fixtures/${fixtureId}/notes`, { headers: { "X-MIS-Admin-Key": key } });
      if (!response.ok) { setStatus(response.status === 401 ? "La clave administrativa no es válida." : "No fue posible consultar las observaciones."); return; }
      setNotes(await response.json());
    } catch { setStatus("No se pudo conectar con el servicio de análisis."); }
    finally { setLoading(false); }
  }

  async function add(event: FormEvent) {
    event.preventDefault();
    if (!text.trim() || !fixtureId || !key) { setStatus("Completa la clave y la observación antes de guardar."); return; }
    setLoading(true); setStatus("");
    const response = await fetch(`${origin}/api/v1/fixtures/${fixtureId}/notes`, { method: "POST", headers: { "Content-Type": "application/json", "X-MIS-Admin-Key": key }, body: JSON.stringify({ minute: Number(minute || 0), tag, text }) });
    if (!response.ok) { setStatus(response.status === 401 ? "La clave administrativa no es válida." : "No fue posible guardar la observación."); setLoading(false); return; }
    const created: Note = await response.json();
    setNotes((current) => [...current, created].sort((left, right) => left.minute - right.minute));
    setMinute(""); setText("");
    setStatus("Observación guardada."); setLoading(false);
  }
  return <section><h2>Libreta de partido</h2><p>Registra una observación mientras ves el partido. Después podrá asociarse con evidencia y clips.</p><div className="filters"><input aria-label="Clave administrativa" type="password" placeholder="Clave administrativa" value={key} onChange={(event) => setKey(event.target.value)} /><button type="button" onClick={loadNotes} disabled={!fixtureId || !key || loading}>{loading ? "Consultando…" : "Cargar libreta"}</button></div><form className="filters" onSubmit={add}><input aria-label="Minuto" inputMode="numeric" max="130" min="0" placeholder="Minuto" type="number" value={minute} onChange={(event) => setMinute(event.target.value)} /><select aria-label="Etiqueta" value={tag} onChange={(event) => setTag(event.target.value)}>{tags.map((item) => <option key={item}>{item}</option>)}</select><input aria-label="Observación" placeholder="Observación táctica" value={text} onChange={(event) => setText(event.target.value)} /><button type="submit" disabled={!fixtureId || !key || loading}>Guardar</button></form>{status && <p role="status">{status}</p>}{notes.length === 0 ? <p>Aún no hay observaciones cargadas para este partido.</p> : <ul>{notes.map((note) => <li key={note.id}><strong>{note.minute}' · {note.tag}</strong> · {note.text}</li>)}</ul>}</section>;
}
