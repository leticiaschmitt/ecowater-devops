# =============================================================
# EcoWater API - Dockerfile multi-stage (build -> runtime)
# =============================================================

# ---------- Etapa 1: build/publish com o SDK do .NET 8 ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas o .csproj primeiro para aproveitar o cache de camadas no restore
COPY src/EcoWaterApi/EcoWaterApi.csproj src/EcoWaterApi/
RUN dotnet restore src/EcoWaterApi/EcoWaterApi.csproj

# Copia o restante do código e publica em modo Release
COPY src/EcoWaterApi/ src/EcoWaterApi/
RUN dotnet publish src/EcoWaterApi/EcoWaterApi.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Etapa 2: runtime enxuto (somente ASP.NET Core) ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# curl é usado pelo HEALTHCHECK
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DisableHttpsRedirection=true

COPY --from=build /app/publish .

# Executa sem privilégios de root (usuário "app" já existe na imagem oficial)
USER app

EXPOSE 8080

HEALTHCHECK --interval=15s --timeout=5s --start-period=40s --retries=5 \
    CMD curl -fs http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "EcoWaterApi.dll"]
