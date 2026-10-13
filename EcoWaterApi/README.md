# EcoWater API

## Monitoramento Inteligente de Consumo de Água e Preservação de Recursos Naturais

## Projeto desenvolvido em ASP.NET Core 8 para apoio a iniciativas ESG

### **Integrante - Letícia Schmitt Rocha**

---

# Visão Geral

A EcoWater API é uma solução desenvolvida para monitoramento inteligente do consumo de água, com foco na identificação de desperdícios, detecção de possíveis vazamentos e geração de informações estratégicas para tomada de decisão sustentável.

O projeto foi concebido considerando os princípios de ESG (Environmental, Social and Governance), especificamente o eixo de **Acesso à Água e Preservação de Recursos Naturais**, promovendo o uso consciente dos recursos hídricos por meio de monitoramento contínuo, automação de alertas e geração de indicadores ambientais.

A solução simula um cenário real de utilização em residências, condomínios, indústrias e organizações que necessitam acompanhar padrões de consumo e agir preventivamente diante de situações de desperdício.


# Problema de Negócio

O desperdício de água é um dos principais desafios ambientais enfrentados atualmente.

Em muitos cenários, vazamentos ou consumos anormais são percebidos apenas após a análise da conta de água, quando o prejuízo financeiro e ambiental já ocorreu.

A ausência de monitoramento em tempo real dificulta:

* Identificação rápida de vazamentos;
* Controle eficiente de consumo;
* Implementação de políticas de sustentabilidade;
* Geração de indicadores ambientais;
* Planejamento de ações preventivas.

A EcoWater API busca resolver esse problema fornecendo uma plataforma capaz de registrar leituras, gerar alertas automáticos e disponibilizar relatórios consolidados para análise.


# Objetivos do Projeto

* Monitorar o consumo de água por sensores inteligentes;
* Registrar leituras periódicas de consumo;
* Detectar padrões de consumo acima do limite permitido;
* Gerar alertas automáticos para possíveis desperdícios;
* Disponibilizar relatórios ambientais;
* Aplicar autenticação e autorização em operações críticas;
* Demonstrar boas práticas de desenvolvimento utilizando ASP.NET Core 8.


# Arquitetura da Solução

O projeto foi estruturado utilizando o padrão arquitetural MVVM (Model-View-ViewModel), promovendo desacoplamento, organização e manutenção simplificada do código.

## Estrutura de Camadas

### Models

Responsáveis pela representação das entidades persistidas no banco de dados.

Entidades implementadas:

* Sensor
* ConsumptionReading
* Alert
* EnvironmentalReport
---

### ViewModels

Responsáveis pela comunicação entre API e cliente.

ViewModels implementados:

* SensorViewModel
* ConsumptionViewModel
* AlertViewModel
---

### Services

Camada responsável pelas regras de negócio.

Serviços implementados:

* SensorService
* ConsumptionService
* AlertService
* ReportService
---

### Controllers

Camada responsável pela exposição dos endpoints REST.

Controllers implementados:

* AuthController
* SensorController
* ConsumptionController
* AlertController
* ReportController
---

### Data

Camada responsável pela persistência de dados utilizando Entity Framework Core.

* AppDbContext
* Migrations


# Tecnologias Utilizadas

## Backend

* ASP.NET Core 8
* C#
* Entity Framework Core
* LINQ
---

## Banco de Dados

* Microsoft SQL Server 2022
* Entity Framework Migrations
---

## Segurança

* JWT Authentication
* Authorization Attributes
* Token Validation
---

## Documentação

* Swagger / OpenAPI
---

## Testes

* xUnit
* Microsoft.AspNetCore.Mvc.Testing
---

## Containerização

* Docker



# Funcionalidades Implementadas

## 1. Gestão de Sensores

Permite o cadastro e gerenciamento dos sensores responsáveis pelo monitoramento de consumo.

### Endpoints
```
GET /api/sensors

GET /api/sensors/{id}

POST /api/sensors

PUT /api/sensors/{id}

DELETE /api/sensors/{id}
```
---

### Recursos

* Paginação
* CRUD completo
* Validação de dados
* Proteção por JWT em operações críticas



## 2. Registro de Consumo

Permite registrar leituras de consumo realizadas pelos sensores.

### Endpoints
```
GET /api/consumptions

GET /api/consumptions/{id}

POST /api/consumptions
```
---

### Recursos

* Registro de consumo em litros
* Associação com sensores
* Detecção automática de consumo acima do limite configurado
* Geração automática de alertas



## 3. Gestão de Alertas

Responsável por notificar situações potencialmente críticas.

### Endpoints
```
GET /api/alerts

PUT /api/alerts/{id}/resolve
```
---

### Recursos

* Alertas automáticos
* Controle de resolução
* Histórico de ocorrências



## 4. Relatórios Ambientais

Disponibiliza indicadores para análise do impacto ambiental.

### Endpoints
```
GET /api/reports/environmental-impact

GET /api/reports/water-saving
```
---

### Recursos

* Indicadores consolidados
* Consumo total monitorado
* Estimativa de desperdício
* Recomendações de sustentabilidade


# Segurança

Para garantir a proteção dos recursos da aplicação, foi implementado mecanismo de autenticação baseado em JWT (JSON Web Token).

Operações de criação, atualização e exclusão exigem autenticação prévia.

Fluxo:

1. Solicitar token através de:
```
POST /api/auth/login
```

2. Receber token JWT.

3. Informar o token no header Authorization:

Bearer {token}

4. Executar operações protegidas.


# Paginação

Os endpoints de listagem foram desenvolvidos com suporte à paginação para otimizar consultas e garantir escalabilidade.

Parâmetros disponíveis:

* page
* pageSize

Exemplo:
```
GET /api/sensors?page=1&pageSize=10
```

Benefícios:

* Redução de carga no banco de dados;
* Melhor desempenho;
* Escalabilidade para grandes volumes de dados.



# Banco de Dados

A aplicação utiliza Microsoft SQL Server integrado ao Entity Framework Core.

### Recursos Implementados

* Migrations
* Criação automática de schema
* Relacionamentos entre entidades
* Controle de versão da estrutura do banco

Comandos utilizados:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```


# Testes Automatizados

Foram implementados testes utilizando xUnit para validação dos endpoints REST.

Testes desenvolvidos:

* SensorControllerTests
* ConsumptionControllerTests
* AlertControllerTests
* ReportControllerTests

Todos os testes validam o retorno HTTP 200 conforme solicitado no desafio.

Execução:

```bash
dotnet test
```



# Containerização

A aplicação pode ser executada através do Docker.

Build da imagem:

```bash
docker build -t ecowater-api .
```

Execução:

```bash
docker run -p 8080:8080 ecowater-api
```



# Requisitos Atendidos do Desafio

✅ ASP.NET Core 8

✅ Arquitetura MVVM

✅ Banco de Dados SQL Server

✅ Entity Framework Core

✅ Migrations

✅ Endpoints RESTful

✅ Paginação

✅ Autenticação JWT

✅ Swagger

✅ Testes xUnit

✅ Dockerfile

✅ Integração com banco de dados

✅ Tema ESG

---

# Considerações Finais

A EcoWater API demonstra a aplicação prática dos conceitos de desenvolvimento de APIs modernas utilizando ASP.NET Core 8, contemplando aspectos de arquitetura, segurança, persistência de dados, testes automatizados e boas práticas de engenharia de software.

Além do atendimento integral aos requisitos técnicos propostos no desafio, a solução contribui para iniciativas de sustentabilidade por meio do monitoramento inteligente do consumo de água, reforçando a importância da tecnologia como ferramenta de preservação ambiental.