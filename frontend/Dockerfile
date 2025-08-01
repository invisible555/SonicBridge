# Etap 1: Budowanie aplikacji
FROM node:20-alpine AS build

WORKDIR /app

COPY package.json package-lock.json* ./
COPY vite.config.* ./
COPY tailwind.config.* ./
COPY tsconfig.* ./
COPY index.html ./
COPY public ./public
COPY src ./src
COPY .env.docker .env

RUN npm install
RUN npm run build

# Etap 2: Serwowanie przez nginx
FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY --from=build /app/dist /usr/share/nginx/html

# 👇 tu zmieniam ścieżkę configu
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
