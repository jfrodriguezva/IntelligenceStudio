"use client";

import { ChangeEvent, FormEvent, useEffect, useState } from "react";

type Evidence = { id: string; minute: number; description: string; originalFileName: string; contentType: string; byteLength: number; capturedAt: string };

export function EvidenceQueue({ fixtureId }: { fixtureId: string }) {
  const [items, setItems] = useState<Evidence[]>([]);
  const [key, setKey] = useState("");
  const [minute, setMinute] = useState("");
  const [description, setDescription] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState("");
  const [loading, setLoading] = useState(false);
  const origin = process.env.NEXT_PUBLIC_MIS_API_ORIGIN ?? "http://localhost:5080";

  useEffect(() => { setItems([]); setStatus(""); setFile(null); }, [fixtureId]);

  async function load() {
    if (!key) { setStatus("Escribe la clave administrativa para consultar la evidencia."); return; }
    setLoading(true); setStatus("");
    try {
      const response = await fetch(`${origin}/api/v1/fixtures/${fixtureId}/evidence`, { headers: { "X-MIS-Admin-Key": key } });
      if (!response.ok) { setStatus(response.status === 401 ? "La clave administrativa no es válida." : "No fue posible consultar la evidencia."); return; }
      setItems(await response.json());
    } catch { setStatus("No se pudo conectar con el Centro de evidencia."); }
    finally { setLoading(false); }
  }

  function selectFile(event: ChangeEvent<HTMLInputElement>) { setFile(event.target.files?.[0] ?? null); }

  async function attach(event: FormEvent) {
    event.preventDefault();
    if (!key || !file || !description.trim()) { setStatus("Selecciona un archivo, escribe su descripción y proporciona la clave administrativa."); return; }
    const payload = new FormData();
    payload.append("file", file); payload.append("minute", minute || "0"); payload.append("description", description.trim());
    setLoading(true); setStatus("");
    try {
      const response = await fetch(`${origin}/api/v1/fixtures/${fixtureId}/evidence`, { method: "POST", headers: { "X-MIS-Admin-Key": key }, body: payload });
      if (!response.ok) { setStatus(response.status === 401 ? "La clave administrativa no es válida." : "El archivo fue rechazado. Usa una imagen o video permitido de hasta 100 MB."); return; }
      const created: Evidence = await response.json(); setItems((current) => [...current, created].sort((left, right) => left.minute - right.minute)); setMinute(""); setDescription(""); setFile(null); setStatus("Evidencia guardada en el repositorio privado local.");
    } catch { setStatus("No se pudo conectar con el Centro de evidencia."); }
    finally { setLoading(false); }
  }

  return <section><h2>Cola de evidencia</h2><p>Asocia una imagen o clip con el minuto y el contexto que quieres recuperar durante la edición. Se aceptan JPEG, PNG, WebP, MP4, WebM y MOV, hasta 100 MB.</p><div className="filters"><input aria-label="Clave administrativa" type="password" placeholder="Clave administrativa" value={key} onChange={(event) => setKey(event.target.value)} /><button type="button" onClick={load} disabled={!key || loading}>{loading ? "Consultando…" : "Cargar evidencia"}</button></div><form className="filters" onSubmit={attach}><input aria-label="Minuto asociado" inputMode="numeric" min="0" max="130" placeholder="Minuto" type="number" value={minute} onChange={(event) => setMinute(event.target.value)} /><input aria-label="Nota de evidencia" placeholder="Qué debe encontrarse" value={description} onChange={(event) => setDescription(event.target.value)} /><input aria-label="Archivo de evidencia" accept="image/jpeg,image/png,image/webp,video/mp4,video/webm,video/quicktime" type="file" onChange={selectFile} /><button type="submit" disabled={!key || !file || !description.trim() || loading}>Guardar evidencia</button></form>{status && <p role="status">{status}</p>}{items.length === 0 ? <p>No hay evidencia cargada para este partido.</p> : <ul>{items.map((item) => <li key={item.id}><strong>{item.minute}' · {item.originalFileName}</strong> · {item.contentType} · {Math.ceil(item.byteLength / 1024)} KB{item.description ? ` · ${item.description}` : ""}</li>)}</ul>}</section>;
}
