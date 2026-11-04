#!/usr/bin/env bash
set -euo pipefail
export ECOWATER_BASE_URL="${ECOWATER_BASE_URL:-http://localhost:8080}"
mkdir -p TestResults/Bdd
# Resultado TRX persistido para rastreabilidade da execução corrente.
curl -fsS "${ECOWATER_BASE_URL}/health" >/dev/null
dotnet test tests/EcoWater.BddTests/EcoWater.BddTests.csproj \
  --configuration Release \
  --logger 'trx;LogFileName=ecowater-bdd.trx' \
  --results-directory TestResults/Bdd \
  --logger 'console;verbosity=normal'
