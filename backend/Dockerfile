# STAGE 1: Build y Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

# Copiar solo los archivos de proyecto (.csproj) para aprovechar el caché de capas de Docker
COPY ["src/API/API.csproj", "src/API/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "src/API/API.csproj"

# Copiar el resto del código fuente
COPY . .

# Compilar y publicar la app en modo Release hacia la carpeta /publish
WORKDIR "/app/src/API"
RUN dotnet publish "API.csproj" -c Release -o /publish /p:UseAppHost=false

# STAGE 2: Runtime (Imagen Final Ligera)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Instalar dependencias necesarias para QuestPDF (Fuentes y librerías de dibujo)
# Alpine las necesita para generar los PDFs de stock bajo
RUN apk add --no-cache fontconfig ttf-dejavu

# Copiar solo los binarios compilados desde el stage anterior
COPY --from=build /publish .

# Exponer el puerto configurado en el contenedor
EXPOSE 8080

# Usuario no-root por seguridad
USER $APP_UID

ENTRYPOINT ["dotnet", "API.dll"]