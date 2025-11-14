Feature: Regras de Desconto por Quantidade
  Como sistema de vendas
  Eu preciso aplicar descontos automáticos baseados na quantidade
  Para incentivar compras maiores

  Scenario Outline: Aplicar desconto baseado na quantidade
    Given que estou criando uma venda
    When eu adicionar <quantidade> itens com valor unitário de R$ 100,00
    Then o desconto aplicado deve ser <desconto>%
    And o valor total do item deve ser R$ <valor_total>

    Examples:
      | quantidade | desconto | valor_total |
      | 1          | 0        | 100.00      |
      | 2          | 0        | 200.00      |
      | 3          | 0        | 300.00      |
      | 4          | 0        | 400.00      |
      | 5          | 10       | 450.00      |
      | 9          | 10       | 810.00      |
      | 10         | 20       | 800.00      |
      | 15         | 20       | 1200.00     |
      | 20         | 20       | 1600.00     |

  Scenario: Não permitir venda acima de 20 itens
    Given que estou criando uma venda
    When eu tentar adicionar 21 itens
    Then a venda não deve ser criada
    And deve retornar erro de validação
    And a mensagem deve indicar que quantidade > 20 é proibida

  Scenario: Não permitir desconto para menos de 4 itens
    Given que estou criando uma venda
    When eu adicionar 3 itens
    Then não deve aplicar nenhum desconto
    And o valor total deve ser o valor bruto

