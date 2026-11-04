# Auditoria técnica adicional — EcoWater BDD (04/11/2026)

## Escopo e resultados COMPROVADOS nesta sessão
- 20 cenários Gherkin e 146 passos textuais (todos mapeados a definições C# por verificação de expressões).
- 10 JSON Schemas: sintaxe e estrutura validadas com jsonschema Draft 2020-12.
- 3 arquivos YAML de workflows: parse de sintaxe concluído.
- Shell scripts: bash -n concluído.
- Todos os arquivos .json do repositório: parse concluído (incluindo JSON com BOM UTF-8).
- O arquivo ZIP é gerado com verificação CRC integral.
- Confirmação externa: pacotes Reqnroll.xUnit 2.2.1 e NJsonSchema 11.4.0 existem no NuGet.

## Incrementos desta rodada
- Adicionado teste real da leitura no relatório de impacto ambiental, usando ID e consumo criados durante o cenário.
- Adicionado teste dos valores do relatório de desperdício, com invariantes (desperdício <= consumo total).
- Adicionado teste de paginação para sensores e alertas.
- A etapa de compilação Release foi explicitada no workflow BDD.

## O que NÃO foi validado e permanece bloqueante
- dotnet restore/build/test: indisponível no ambiente de auditoria (SDK .NET não instalado).
- Docker Compose e SQL Server: indisponíveis no ambiente de auditoria (Docker não instalado).
- Não foi possível obter TRX, logs de integração, captura de execução BDD ou aprovação efetiva no CI.
- A documentação anterior de DevOps contém prints e testes xUnit históricos, que NÃO atestam os 20 cenários desta atividade.
- O login da API original emite um JWT sem usuário/senha; os testes cobrem emissão e rejeição de token inválido, e não autenticação de identidade.

## Aceite antes de declarar 100% funcional
1. Em máquina/runner com .NET 8 e Docker, executar: `cp .env.example .env && docker compose up -d --build --wait`.
2. Executar `dotnet restore tests/EcoWater.BddTests/EcoWater.BddTests.csproj`.
3. Executar `dotnet build tests/EcoWater.BddTests/EcoWater.BddTests.csproj -c Release --no-restore`.
4. Executar `bash scripts/run-bdd.sh` e conferir 20/20 testes aprovados no TRX.
5. Guardar log de build, TRX, logs Docker e screenshot do workflow; anexar evidências reais ao PDF.
6. Registrar, corrigir e repetir qualquer cenário falho, antes do upload acadêmico.
