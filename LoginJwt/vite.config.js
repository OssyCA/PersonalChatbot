import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/api": {
        target: "https://localhost:7289",
        changeOrigin: true,
        secure: false,
      },
    },
    cors: {
      origin: "https://localhost:7289",
      credentials: true,
    },
  },
});
