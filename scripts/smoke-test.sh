#!/usr/bin/env bash
# Smoke tests executados após cada deploy (staging e produção).
# Uso: ./scripts/smoke-test.sh <base_url> <pasta_de_evidencias>
set -euo pipefail

BASE_URL="${1:-http://localhost:8080}"
OUT="${2:-evidencias}"
mkdir -p "$OUT"

echo "==> Aguardando a API ficar saudável em $BASE_URL/health"
for i in $(seq 1 40); do
  if curl -fs "$BASE_URL/health" > /dev/null; then echo "API saudável (tentativa $i)"; break; fi
  if [ "$i" -eq 40 ]; then echo "API não respondeu a tempo"; exit 1; fi
  sleep 5
done

check() { # check <descrição> <arquivo> <status esperado> <curl args...>
  local desc="$1" file="$2" expected="$3"; shift 3
  local status
  status=$(curl -s -o "$OUT/$file" -w "%{http_code}" "$@")
  echo "[$status] $desc" | tee -a "$OUT/smoke-test.log"
  [ "$status" = "$expected" ] || { echo "Falhou: esperado $expected"; cat "$OUT/$file"; exit 1; }
}

: > "$OUT/smoke-test.log"
check "GET  /health"                          health.txt          200 "$BASE_URL/health"
check "GET  /swagger/v1/swagger.json"         swagger.json        200 "$BASE_URL/swagger/v1/swagger.json"
check "GET  /api/sensors?page=1&pageSize=5"   sensors.json        200 "$BASE_URL/api/sensors?page=1&pageSize=5"
check "POST /api/sensors sem token (401)"     unauthorized.json   401 -X POST "$BASE_URL/api/sensors" -H "Content-Type: application/json" -d '{}'
check "POST /api/auth/login"                  login.json          200 -X POST "$BASE_URL/api/auth/login"

TOKEN=$(sed -E 's/.*"token":"([^"]+)".*/\1/' "$OUT/login.json")

check "POST /api/sensors (com JWT)"           sensor-created.json 201 -X POST "$BASE_URL/api/sensors" \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"name":"Sensor Smoke Test","location":"Pipeline CI/CD","maxConsumptionLimit":500,"isActive":true}'
check "GET  /api/consumptions"                consumptions.json   200 "$BASE_URL/api/consumptions"
check "GET  /api/alerts"                      alerts.json         200 "$BASE_URL/api/alerts"
check "GET  /api/reports/environmental-impact" report-impact.json 200 "$BASE_URL/api/reports/environmental-impact"
check "GET  /api/reports/water-saving"        report-saving.json  200 "$BASE_URL/api/reports/water-saving"

echo "==> Smoke tests OK"
