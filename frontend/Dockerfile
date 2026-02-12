FROM node:22.12-alpine

WORKDIR /app

# Instalamos Bun para mantener la velocidad
RUN npm install -g bun

# Copiamos dependencias
COPY package.json bun.lockb* ./
RUN bun install

# Copiamos el resto del código
COPY . .

# Exponemos el puerto de Angular
EXPOSE 4200

# Usamos el comando base. El host 0.0.0.0 es obligatorio para Docker.
CMD ["bun", "run", "ng", "serve", "--host", "0.0.0.0"]