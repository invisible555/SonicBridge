import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig(({ command }) => {
  const config: any = {
    plugins: [react(), tailwindcss()],
    server: {
      port: 5173,
      host: 'localhost'
    }
  };

  if (command === 'serve') {
    // HTTPS tylko w trybie deweloperskim!
    config.server.https = {
      key: fs.readFileSync(path.resolve(__dirname, 'certs/cert.key')),
      cert: fs.readFileSync(path.resolve(__dirname, 'certs/cert.crt'))
    };
  }

  return config;
});
