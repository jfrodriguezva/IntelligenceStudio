"use client";

import { FormEvent, useState } from "react";

const tags = ["PRESSING", "BUILDUP", "ATTACK", "DEFENSE", "TRANSITION", "SET_PIECE", "INDIVIDUAL", "ERROR", "TACTICAL", "GOAL", "CHANCE", "CLIP"];
type Note = { minute: string; tag: string; text: string };

export function MatchNotebook() {
  const [notes, setNotes] = useState<Note[]>([]);
  const [minute, setMinute] = useState("");
  const [tag, setTag] = useState(tags[0]);
  const [text, setText] = useState("");
  function add(event: FormEvent) {
    event.preventDefault();
    if (!text.trim()) return;
    setNotes((current) => [...current, { minute: minute || "—", tag, text: text.trim() }]);
    setMinute(""); setText("");
  }
  return <section><h2>Libreta de partido</h2><p>Registra una observación mientras ves el partido. Después podrá asociarse con evidencia y clips.</p><form className="filters" onSubmit={add}><input aria-label="Minuto" inputMode="numeric" max="130" min="0" placeholder="Minuto" type="number" value={minute} onChange={(event) => setMinute(event.target.value)} /><select aria-label="Etiqueta" value={tag} onChange={(event) => setTag(event.target.value)}>{tags.map((item) => <option key={item}>{item}</option>)}</select><input aria-label="Observación" placeholder="Observación táctica" value={text} onChange={(event) => setText(event.target.value)} /><button type="submit">Agregar</button></form>{notes.length === 0 ? <p>Aún no hay observaciones en esta sesión.</p> : <ul>{notes.map((note, index) => <li key={`${note.minute}-${index}`}><strong>{note.minute}' · {note.tag}</strong> · {note.text}</li>)}</ul>}</section>;
}
