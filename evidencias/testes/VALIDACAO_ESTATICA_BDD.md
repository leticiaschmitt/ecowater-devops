# Auditoria estática dos artefatos | EcoWater BDD

Data de elaboração: 04/11/2026

| Verificação | Resultado |
|---|---|
| Cenários Gherkin | 18 identificados |
| Passos Gherkin | 125 identificados |
| Bindings C# associados aos passos | 31 declarações; 0 passos sem correspondência textual |
| JSON Schemas | 10 válidos segundo Draft 2020-12 |
| Referências aos schemas no Gherkin | 10 nomes resolvidos |
| Workflows YAML | 3 analisados |
| Endpoints de controllers | 13 operações de API + 1 health check identificados |
| Script local de execução | Presente |

## Escopo exato da validação

Esta auditoria **não é uma execução C#/.NET e não valida funcionalidade HTTP**. Confere a estrutura dos arquivos, a coerência de nomenclatura dos cenários, os contratos JSON Schema, a sintaxe YAML e as declarações de rotas presentes no projeto.

## Pendente para validação dinâmica

Executar `dotnet restore`, `dotnet build`, subir SQL Server e API com Docker e rodar `bash scripts/run-bdd.sh` no ambiente integrado. Guardar o TRX e prints da execução do workflow, sem reaproveitar os screenshots anteriores de DevOps como evidências de BDD.
