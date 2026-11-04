Feature: EcoWater REST API - qualidade e sustentabilidade
  Como equipe responsável pelo monitoramento do consumo de água
  Quero validar os contratos e o comportamento da EcoWater API
  Para detectar desperdícios e evitar regressões.

  Scenario: Health check responde com sucesso
    When I send GET "/health" without authentication
    Then the response status must be 200
    And the response text must be "Healthy"

  Scenario: Autenticação retorna JWT válido na estrutura JSON
    When I send POST "/api/auth/login" without authentication
    Then the response status must be 200
    And the response matches schema "auth"
    And the JSON field "token" must be nonempty
    And the JWT must have three segments

  Scenario: Listagens e relatórios respondem com contrato esperado
    When I send GET "/api/sensors?page=1&pageSize=10" without authentication
    Then the response status must be 200
    And the response matches schema "page-sensors"
    When I send GET "/api/consumptions?page=1&pageSize=10" without authentication
    Then the response status must be 200
    And the response matches schema "page-consumptions"
    When I send GET "/api/alerts?page=1&pageSize=10" without authentication
    Then the response status must be 200
    And the response matches schema "page-alerts"
    When I send GET "/api/reports/environmental-impact?page=1&pageSize=10" without authentication
    Then the response status must be 200
    And the response matches schema "page-report"
    When I send GET "/api/reports/water-saving" without authentication
    Then the response status must be 200
    And the response matches schema "water-saving"

  Scenario: Criação e consulta de sensor com autenticação
    Given I have an access token
    When I create a valid sensor
    Then the response status must be 201
    And the response matches schema "sensor"
    And the JSON field "name" must equal "Sensor BDD"
    When I get the created sensor
    Then the response status must be 200
    And the response matches schema "sensor"

  Scenario: Atualização e exclusão do sensor
    Given I have an access token
    And I have created a sensor
    When I update the created sensor
    Then the response status must be 200
    And the response matches schema "message"
    When I get the created sensor
    Then the response status must be 200
    And the JSON field "name" must equal "Sensor BDD Atualizado"
    When I delete the created sensor
    Then the response status must be 200
    And the response matches schema "message"
    When I get the created sensor
    Then the response status must be 404
    And the response matches schema "message"

  Scenario: Cadastro de sensor exige autorização
    When I send POST "/api/sensors" without authentication using a valid sensor
    Then the response status must be 401

  Scenario: Cadastro de sensor rejeita limite inválido
    Given I have an access token
    When I send POST "/api/sensors" with invalid sensor
    Then the response status must be 400
    And the response matches schema "validation-error"

  Scenario: Consulta de sensor inexistente retorna 404
    When I send GET "/api/sensors/2147483647" without authentication
    Then the response status must be 404
    And the response matches schema "message"

  Scenario: Consumo dentro do limite não indica suspeita de vazamento
    Given I have an access token
    And I have created a sensor
    When I register a consumption of 50 liters
    Then the response status must be 201
    And the response matches schema "consumption"
    And the JSON field "hasLeakSuspicion" must be false
    And the consumption response belongs to the created sensor
    When I list alerts
    Then the response status must be 200
    And no alert must exist for the created sensor
    When I get the created consumption
    Then the response status must be 200
    And the response matches schema "consumption"

  Scenario: Consumo acima do limite gera alerta ambiental
    Given I have an access token
    And I have created a sensor
    When I register a consumption of 150 liters
    Then the response status must be 201
    And the response matches schema "consumption"
    And the JSON field "hasLeakSuspicion" must be true
    And the consumption response belongs to the created sensor
    When I list alerts
    Then the response status must be 200
    And the response matches schema "page-alerts"
    And there must be an alert for the created sensor
    When I resolve the alert
    Then the response status must be 200
    And the response matches schema "message"
    When I list alerts again
    Then the response status must be 200
    And the generated alert must be resolved

  Scenario: Consumo exige sensor existente
    Given I have an access token
    When I register consumption for a nonexistent sensor
    Then the response status must be 400
    And the response matches schema "message"

  Scenario: Consumo não pode ser criado sem autenticação
    When I send POST "/api/consumptions" without authentication using valid consumption
    Then the response status must be 401

  Scenario: Consumo de zero litros é inválido
    Given I have an access token
    And I have created a sensor
    When I register a consumption of 0 liters
    Then the response status must be 400
    And the response matches schema "validation-error"

  Scenario: Leitura inexistente retorna 404
    When I send GET "/api/consumptions/2147483647" without authentication
    Then the response status must be 404
    And the response matches schema "message"

  Scenario: Alteração e exclusão exigem autorização
    When I send PUT "/api/sensors/2147483647" without authentication using a valid sensor
    Then the response status must be 401
    When I send DELETE "/api/sensors/2147483647" without authentication
    Then the response status must be 401
    When I send PUT "/api/alerts/2147483647/resolve" without authentication
    Then the response status must be 401

  Scenario: Atualizar, excluir e resolver recursos inexistentes retorna 404
    Given I have an access token
    When I send PUT "/api/sensors/2147483647" with valid sensor
    Then the response status must be 404
    And the response matches schema "message"
    When I send DELETE "/api/sensors/2147483647" with authentication
    Then the response status must be 404
    And the response matches schema "message"
    When I send PUT "/api/alerts/2147483647/resolve" with authentication
    Then the response status must be 404
    And the response matches schema "message"

  Scenario: Consumo exatamente igual ao limite não produz alerta
    Given I have an access token
    And I have created a sensor
    When I register a consumption of 100 liters
    Then the response status must be 201
    And the response matches schema "consumption"
    And the JSON field "hasLeakSuspicion" must be false
    When I list alerts
    Then the response status must be 200
    And no alert must exist for the created sensor

  Scenario: Token inválido não concede permissão de cadastro
    When I send a request with an invalid bearer token to the sensors API
    Then the response status must be 401

  Scenario: Impacto ambiental reflete consumo registrado e alerta associado
    Given I have an access token
    And I have created a sensor
    When I register a consumption of 150 liters
    Then the response status must be 201
    And the JSON field "hasLeakSuspicion" must be true
    When I send GET "/api/reports/environmental-impact?page=1&pageSize=1000" without authentication
    Then the response status must be 200
    And the response matches schema "page-report"
    And the environmental impact report contains the created sensor reading
    When I send GET "/api/reports/water-saving" without authentication
    Then the response status must be 200
    And the response matches schema "water-saving"
    And the waste summary includes at least 150 liters of suspected waste

  Scenario: Paginação respeita página e tamanho solicitados
    When I send GET "/api/sensors?page=1&pageSize=2" without authentication
    Then the response status must be 200
    And the response matches schema "page-sensors"
    And the pagination must indicate page 1 and page size 2
    When I send GET "/api/alerts?page=1&pageSize=2" without authentication
    Then the response status must be 200
    And the response matches schema "page-alerts"
    And the pagination must indicate page 1 and page size 2
