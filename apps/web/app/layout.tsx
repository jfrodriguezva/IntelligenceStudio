import type { Metadata } from "next";
import type { ReactNode } from "react";
import Link from "next/link";
import "./styles.css";

export const metadata: Metadata = {
  title: "Madrid Intelligence Studio",
  description: "Football intelligence for Madrid, hagámoslo real.",
};

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {
  return (
    <html lang="es-MX">
      <body><header><Link className="brand" href="/">MIS</Link><nav><Link href="/match-center">Match Center</Link><Link href="/tactical-board">Pizarra</Link><Link href="/analysis">Análisis</Link><Link href="/evidence">Evidencia</Link><Link href="/tactical-library">Biblioteca</Link></nav></header>{children}</body>
    </html>
  );
}
