# Evidências do deploy em staging

Fonte: resumo do job "Deploy - Staging" da execução #1 do workflow **CI/CD - EcoWater API**
(commit `c96b736`, branch `main`) - https://github.com/leticiaschmitt/ecowater-devops/actions

- Imagem: `ghcr.io/leticiaschmitt/ecowater-devops:c96b736`
- ASPNETCORE_ENVIRONMENT: `Staging`
- URL no runner: http://localhost:8081/swagger
- Print: [docs/prints/04-deploy-staging.png](../../docs/prints/)

Os arquivos completos (respostas de cada endpoint, api.log e compose-ps.txt) são gerados a cada execução
e ficam disponíveis no artefato `evidencias-staging` da execução no GitHub Actions.
