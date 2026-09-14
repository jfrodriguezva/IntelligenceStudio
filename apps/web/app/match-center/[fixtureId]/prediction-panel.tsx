"use client";

import { useState } from "react";

export function PredictionPanel({ fixtureId }: { fixtureId: string }) {
  const [key, setKey] = useState("");
  const [message, setMessage] = useState("");
  async function generate() {
    setMessage("Generando predicción…");
    const response = await fetch(`/api/v1/fixtures/${fixtureId}/predictions/baseline`, { method: "POST", headers: { "X-MIS-Admin-Key": key } });
    setMessage(response.ok ? "Predicción guardada. Recarga la página para verla." : response.status === 503 ? "El runtime de ML no está disponible." : "No fue posible generar la predicción.");
    setKey("");
  }
  return <section className="refresh"><h3>Generar predicción base</h3><p>Usa la forma disponible antes del partido. El resultado queda guardado como una versión inmutable.</p><input aria-label="Clave administrativa para predicción" type="password" value={key} onChange={(event) => setKey(event.target.value)} /><button type="button" disabled={!key} onClick={generate}>Generar</button>{message && <p role="status">{message}</p>}</section>;
}
