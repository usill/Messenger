import { defineConfig } from "vite";
import tailwindcss from '@tailwindcss/vite'
import path from 'path';

export default defineConfig({
    plugins: [
        tailwindcss(),
    ],
    build: {
        rollupOptions: {
            input: {
                'index': path.resolve(__dirname, "pages/index.html"),
                'login': path.resolve(__dirname, "pages/login.html"),
                'registration': path.resolve(__dirname, "pages/registration.html"),
            }
        }
    }
});