# Evidências do deploy em production

Fonte: resumo do job "Deploy - Produção" da execução #1 do workflow **CI/CD - EcoWater API**
(commit `c96b736`, branch `main`) - https://github.com/leticiaschmitt/ecowater-devops/actions

- Imagem: `ghcr.io/leticiaschmitt/ecowater-devops:c96b736`
- ASPNETCORE_ENVIRONMENT: `Production`
- URL no runner: http://localhost:8080/swagger
- Print: [docs/prints/05-deploy-producao.png](../../docs/prints/)

Os arquivos completos (respostas de cada endpoint, api.log e compose-ps.txt) são gerados a cada execução
e ficam disponíveis no artefato `evidencias-production` da execução no GitHub Actions.
