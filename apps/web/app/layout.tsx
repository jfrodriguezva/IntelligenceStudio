import type { Metadata } from "next";
import type { ReactNode } from "react";
import "./styles.css";

export const metadata: Metadata = {
  title: "Madrid Intelligence Studio",
  description: "Football intelligence for Madrid, hagámoslo real.",
};

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {
  return (
    <html lang="es-MX">
      <body>{children}</body>
    </html>
  );
}
