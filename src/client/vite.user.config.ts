import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  root: 'user',
  plugins: [vue()],
  resolve: {
    alias: {
      '@user': fileURLToPath(new URL('./user', import.meta.url)),
      '@shared': fileURLToPath(new URL('./src/shared', import.meta.url)),
    },
  },
  build: {
    outDir: '../dist/user',
    emptyOutDir: true,
  },
})
