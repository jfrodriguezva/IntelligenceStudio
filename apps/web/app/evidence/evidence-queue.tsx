"use client";

import { ChangeEvent, useState } from "react";

type Evidence = { name: string; type: string; minute: string; note: string };

export function EvidenceQueue() {
  const [items, setItems] = useState<Evidence[]>([]);
  const [minute, setMinute] = useState("");
  const [note, setNote] = useState("");
  function attach(event: ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0];
    if (!file) return;
    setItems((current) => [...current, { name: file.name, type: file.type || "Archivo", minute: minute || "—", note: note.trim() }]);
    event.target.value = "";
  }
  return <section><h2>Cola de evidencia</h2><p>Asocia una imagen o un clip con el minuto y la observación que quieres encontrar durante la edición.</p><div className="filters"><input aria-label="Minuto asociado" inputMode="numeric" placeholder="Minuto" type="number" value={minute} onChange={(event) => setMinute(event.target.value)} /><input aria-label="Nota de evidencia" placeholder="Qué debe encontrarse" value={note} onChange={(event) => setNote(event.target.value)} /><input aria-label="Archivo de evidencia" accept="image/*,video/*" type="file" onChange={attach} /></div>{items.length === 0 ? <p>No hay evidencia en esta sesión.</p> : <ul>{items.map((item, index) => <li key={`${item.name}-${index}`}><strong>{item.minute}' · {item.name}</strong> · {item.type}{item.note ? ` · ${item.note}` : ""}</li>)}</ul>}</section>;
}
