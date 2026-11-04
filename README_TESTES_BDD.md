# EcoWater | Desafio de Testes Automatizados • FIAP 2026

> **Projeto:** Cidades ESG Inteligentes / EcoWater API  
> **Tecnologias:** ASP.NET Core 8, C#, SQL Server 2022, Docker, Gherkin, Reqnroll, xUnit, NJsonSchema.  
> **Escopo:** API real da atividade DevOps anterior, preservada com uma suíte adicional de testes de comportamento e de contrato.

## 1. Introdução

A EcoWater API monitora sensores de consumo de água e identifica suspeitas de vazamento quando uma leitura ultrapassa o limite configurado. O desafio utiliza **BDD com Gherkin**, executando as solicitações HTTP contra a aplicação em contêiner e validando status, conteúdo e JSON Schema. O domínio atende ao pilar **Ambiental (E)** do ESG.

## 2. O que é testado

**20 cenários**, com 125 linhas de passos Gherkin e cobertura das **13 operações HTTP dos cinco controllers**, além do health check. Os cenários contemplam:

- autenticação JWT e rejeição de token ausente/inválido;
- criação, atualização, exclusão, consulta e paginação de sensores;
- cadastro, consulta e validação de leituras de consumo;
- ausência de alerta para leituras inferiores ou iguais ao limite;
- criação de alerta `High` para consumo superior ao limite, seguida de resolução e consulta do estado;
- relatórios de impacto ambiental e economia de água;
- entradas inválidas, 404 para IDs inexistentes, 400 de validação e 401 de autorização;
- verificação dos contratos com **10 JSON Schemas**.

Os contratos usam `additionalProperties: true` intencionalmente, permitindo compatibilidade com a inclusão de campos; os objetos de listas exigem os campos essenciais esperados no projeto.

### Matriz de APIs

| API | Método | Respostas avaliadas |
|---|---|---|
| `/api/auth/login` | POST | 200, token JWT com 3 segmentos e JSON Schema |
| `/api/sensors` | GET / POST | 200, 201, 400, 401; paginação e contrato |
| `/api/sensors/{id}` | GET / PUT / DELETE | 200, 404, 401; leitura, atualização e exclusão |
| `/api/consumptions` | GET / POST | 200, 201, 400, 401; paginação e contrato |
| `/api/consumptions/{id}` | GET | 200, 404 e contrato |
| `/api/alerts` | GET | 200, paginação, severidade e estado |
| `/api/alerts/{id}/resolve` | PUT | 200, 401, 404; confirmação de resolução |
| `/api/reports/environmental-impact` | GET | 200 e contrato do relatório paginado |
| `/api/reports/water-saving` | GET | 200, indicadores e contrato |
| `/health` | GET | 200, conteúdo `Healthy` (texto simples) |

## 3. Estrutura de arquivos

```text
.github/workflows/bdd-tests.yml                Pipeline BDD independente
scripts/run-bdd.sh                              Execução local e geração de TRX
tests/EcoWater.BddTests/
  EcoWater.BddTests.csproj                      Dependências C#/.NET 8
  Features/EcoWater.feature                    20 cenários Gherkin
  Steps/ApiSteps.cs                            Implementações HTTP e asserts
  Schemas/*.json                               10 contratos JSON Schema
  reqnroll.json                                Cultura e idioma
evidencias/testes/VALIDACAO_ESTATICA_BDD.md     Resultado da auditoria estática
```

## 4. Pré-requisitos

- .NET SDK **8** instalado (`dotnet --info`);
- Docker e Docker Compose (`docker compose version`);
- Bash, curl e pelo menos 4 GB de RAM disponibilizados ao Docker para o SQL Server;
- porta 8080 disponível;
- acesso à Internet durante a primeira restauração de pacotes NuGet/imagens.

Não executar a suíte contra banco real ou de produção. O comando `docker compose down -v` **apaga o volume de banco** da stack selecionada.

## 5. Como executar localmente

Na raiz do repositório:

```bash
cp .env.example .env
# Revise DB_PASSWORD, JWT_KEY e API_PORT no arquivo .env.
# Não versionar nem compartilhar este arquivo com segredos reais.

docker compose up -d --build --wait
docker compose ps
curl -i http://localhost:8080/health

dotnet restore tests/EcoWater.BddTests/EcoWater.BddTests.csproj
bash scripts/run-bdd.sh
```

O script executa `dotnet test`, grava o relatório **TRX** em `TestResults/Bdd/ecowater-bdd.trx` e falha se a API não estiver saudável ou um teste falhar. Guarde o log de terminal e o TRX como evidências. Ao fim, **apenas na stack descartável**:

```bash
docker compose logs --no-color > bdd-container-logs.txt
docker compose down -v
```

Para mudar o destino de teste, use `ECOWATER_BASE_URL=http://localhost:8080` ao chamar o script. Os cenários são elaborados para banco isolado, sem dados prévios. O login de demonstração em `/api/auth/login` emite token sem credenciais; isso é uma **limitação da aplicação de origem**, não um fluxo seguro de autenticação de usuários reais.

## 6. Execução no GitHub Actions

O workflow em `.github/workflows/bdd-tests.yml` dispara por `push` em `main/develop`, `pull_request` para `main` ou manualmente em **Actions → Testes BDD e contratos - EcoWater → Run workflow**. Ele:

1. restaura o SDK .NET 8 e sobe API + SQL Server com Compose;
2. restaura os pacotes e executa os 20 cenários contra `http://localhost:8080`;
3. salva TRX e logs dos contêineres como artefatos, inclusive se a execução falhar;
4. remove a stack descartável ao finalizar.

O fluxo original de DevOps permanece no repositório e **não é substituído**. Os screenshots do pipeline antigo **não comprovam os novos testes BDD**.

## 7. Evidências para a entrega

Antes de enviar a atividade, anexar à documentação final:

- imagem do workflow BDD com status de conclusão, identificador da execução e data;
- imagem do log `dotnet test` informando total, aprovados e falhos;
- `TestResults/Bdd/ecowater-bdd.trx`, ou o TRX obtido dos artefatos do Actions;
- imagem de `docker compose ps` com a stack (API e SQL Server) saudável;
- se houver falhas, apresentar causas e correções, não apenas prints isolados.

**Estado de verificação:** o desenvolvimento e a revisão estática foram realizados em um ambiente sem .NET SDK e sem Docker. A consistência dos arquivos, a sintaxe JSON Schema/YAML e os mapeamentos textuais do Gherkin foram auditados; **a execução dos testes em C# ainda não foi comprovada**. Não informar aprovação em CI sem resultado verdadeiro do workflow.

## 8. Rastreabilidade

- Código: controllers em `src/EcoWaterApi/Controllers`.
- Comportamentos: `Features/EcoWater.feature`.
- Step definitions: `Steps/ApiSteps.cs`.
- Contratos: `Schemas/*.json`.
- Auditoria estática: `evidencias/testes/VALIDACAO_ESTATICA_BDD.md`.
- Evidência dinâmica quando produzida: `TestResults/Bdd/ecowater-bdd.trx` e artefatos de Actions.

## 9. Limitações e melhorias posteriores

A autenticação atual emite JWT sem validar usuário e senha; a suíte verifica o contrato do token e a proteção dos endpoints, sem afirmar que a autenticação de identidade foi implementada. O health check existente verifica o processo, mas não executa consultas explícitas de dependência para o SQL Server. Em um projeto produtivo, seria recomendável acrescentar autenticação real, autorização granular, health checks de dependências e testes de carga. Essas melhorias não foram introduzidas no código da atividade original para evitar alteração indevida de escopo.

## 10. Revisão adicional de 04/11/2026

Foram adicionados dois cenários de regressão que exercitam o relatório de impacto ambiental **com dados reais criados pelo cenário**, o resumo de desperdício e os metadados de paginação. Assim, a validação dos contratos de relatório deixa de se apoiar apenas em coleções vazias. O workflow agora compila explicitamente a suíte em Release antes de testá-la.

A revisão estática foi realizada, mas não substitui a execução com .NET SDK, SQL Server e Docker. Não há evidência de build C# nem de aprovação da nova suíte neste pacote. O arquivo `evidencias/testes/AUDITORIA_FINAL.md` contém os critérios de aceite e as pendências.
