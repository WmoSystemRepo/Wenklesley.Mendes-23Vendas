Feature: Observabilidade para Identificar Gargalos
  Como desenvolvedor/operador
  Eu preciso de observabilidade na aplicação
  Para identificar gargalos de performance

  Scenario: Logs devem estar em formato JSON
    Given que a aplicação está rodando
    When eu fizer uma requisição para a API
    Then os logs devem ser gerados em formato JSON
    And devem usar Serilog
    And devem aparecer no console

  Scenario: PerformanceMiddleware deve medir tempo de resposta
    Given que a aplicação está rodando
    When eu fizer uma requisição para POST /api/venda
    Then o middleware deve medir o tempo de resposta
    And deve logar o tempo em milissegundos
    And deve incluir método, path e status code

  Scenario: Correlation ID deve ser gerado e retornado
    Given que a aplicação está rodando
    When eu fizer uma requisição para a API
    Then deve gerar um Correlation ID
    And deve retornar no header X-Correlation-Id
    And deve estar presente nos logs

  Scenario: Requisições lentas devem ser logadas como Warning
    Given que a aplicação está rodando
    When eu fizer uma requisição que demora mais de 1000ms
    Then deve logar como Warning
    And deve incluir mensagem "Request lento detectado"
    And deve incluir Correlation ID

  Scenario: Health check deve retornar métricas
    Given que a aplicação está rodando
    When eu acessar GET /health
    Then deve retornar status 200 OK
    And deve retornar status "healthy"
    And deve retornar timestamp
    And deve retornar uptime
    And deve retornar environment
    And deve retornar version

