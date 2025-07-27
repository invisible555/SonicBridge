# Etap 1: Budowanie aplikacji
FROM node:20-alpine AS build

WORKDIR /app

# Kopiuj pliki konfiguracyjne i zależności
COPY package.json package-lock.json* ./
COPY vite.config.* ./
COPY tailwind.config.* ./
COPY tsconfig.* ./
COPY index.html ./
COPY public ./public
COPY src ./src
# Instalacja zależności
RUN npm install

# Budowanie aplikacji (Vite)
RUN npm run build

# Etap 2: Serwowanie przez nginx
FROM nginx:alpine

# Usuwamy domyślne pliki nginx
RUN rm -rf /usr/share/nginx/html/*

# Kopiujemy statyczne pliki z builda
COPY --from=build /app/dist /usr/share/nginx/html

# (opcjonalnie) Skopiuj własny nginx.conf jeśli masz
# COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
