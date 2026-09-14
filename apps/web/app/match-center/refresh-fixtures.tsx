"use client";

import { useState } from "react";

export function RefreshFixtures() {
  const [key, setKey] = useState("");
  const [message, setMessage] = useState<string>();

  async function refresh() {
    setMessage("Actualizando…");
    const response = await fetch("/api/v1/acquisition/fixtures/refresh", { method: "POST", headers: { "X-MIS-Admin-Key": key } });
    setMessage(response.ok ? "Actualización terminada. Recarga la página para ver los datos." : "No se pudo actualizar. Verifica la clave administrativa.");
    setKey("");
  }

  return <section className="refresh"><h2>Actualizar datos</h2><p>Usa la clave administrativa local para importar fixtures recientes y próximos.</p><input aria-label="Clave administrativa" type="password" value={key} onChange={(event) => setKey(event.target.value)} /><button type="button" onClick={refresh} disabled={!key}>Actualizar</button>{message && <p>{message}</p>}</section>;
}
