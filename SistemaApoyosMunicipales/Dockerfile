# Esta fase se usa cuando se ejecuta desde VS en modo rápido
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER root
WORKDIR /app

# Instalar dependencias nativas para SkiaSharp (actualizado para Ubuntu 24.04 en .NET 10)
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    libfontconfig1 \
    libfreetype6 \
    libpng-dev \
    libjpeg-dev \
    libgif-dev \
    libexif-dev \
    libx11-dev \
    libxext-dev \
    libxrender-dev \
    libxkbcommon-x11-0 \
    libgl1 \
    libglib2.0-0 \
    && rm -rf /var/lib/apt/lists/*

# Volver al usuario no-root para seguridad
USER $APP_UID

EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:8080;https://*:8081

# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Copiar los archivos .csproj de la API y de las capas de Clean Architecture
COPY ["SistemaApoyosMunicipales/SistemaApoyosMunicipales.API.csproj", "SistemaApoyosMunicipales/"]
COPY ["SistemaApoyosMunicipales.Application/SistemaApoyosMunicipales.Application.csproj", "SistemaApoyosMunicipales.Application/"]
COPY ["SistemaApoyosMunicipales.Domain/SistemaApoyosMunicipales.Domain.csproj", "SistemaApoyosMunicipales.Domain/"]
COPY ["SistemaApoyosMunicipales.Infrastructure/SistemaApoyosMunicipales.Infrastructure.csproj", "SistemaApoyosMunicipales.Infrastructure/"]

# 2. Restaurar dependencias apuntando al proyecto principal (.API)
RUN dotnet restore "SistemaApoyosMunicipales/SistemaApoyosMunicipales.API.csproj"

# 3. Copiar el resto del código fuente
COPY . .

# 4. Compilar el proyecto apuntando al archivo .API.csproj
WORKDIR "/src/SistemaApoyosMunicipales"
RUN dotnet build "SistemaApoyosMunicipales.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SistemaApoyosMunicipales.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaApoyosMunicipales.API.dll"]