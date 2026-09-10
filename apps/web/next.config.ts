import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  async rewrites() {
    const apiOrigin = process.env.MIS_API_ORIGIN ?? "http://localhost:5080";

    return [{ source: "/api/:path*", destination: `${apiOrigin}/api/:path*` }];
  },
};

export default nextConfig;
