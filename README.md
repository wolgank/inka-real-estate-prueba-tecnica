# 🏠 Inka Real Estate - Gestión de Inventarios

Prueba técnica desarrollada con una arquitectura robusta, utilizando **.NET 8** para el Backend y **Angular** para el Frontend, todo orquestado con **Docker**.

## 🚀 Requisitos Previos

* [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y en ejecución.
* Node.js y Bun (opcional, si deseas correr el frontend localmente).

## 🛠️ Configuración del Entorno

1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/wolgank/inka-real-estate-prueba-tecnica.git](https://github.com/wolgank/inka-real-estate-prueba-tecnica.git)
    cd inka-real-estate-prueba-tecnica
    ```

2.  **Variables de Entorno:**
    Copia el archivo de ejemplo y configura tus credenciales (especialmente las de SMTP para las notificaciones de stock bajo).
    ```bash
    cp .env.example .env
    ```

## 🐳 Despliegue con Docker (Recomendado)

He preparado diferentes perfiles de Docker Compose para facilitar las pruebas:

### A. Levantar todo el ecosistema (DB + Backend + Frontend)
Ideal para una revisión rápida de la funcionalidad completa.
```bash
docker-compose up -d
