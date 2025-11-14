Feature: Criar Venda
  Como um usuário do sistema
  Eu quero criar uma nova venda
  Para registrar uma transação comercial

  Scenario: Criar venda com sucesso
    Given que não existem vendas cadastradas
    When eu criar uma venda com os seguintes dados:
      | NumeroVenda | ClienteNome | FilialNome | Quantidade | ValorUnitario |
      | V001        | João Silva  | Filial SP  | 5          | 100.00        |
    Then a venda deve ser criada com sucesso
    And o valor total deve ser calculado corretamente
    And deve aplicar desconto de 10% para 5 itens
    And o evento CompraEfetuada deve ser emitido

  Scenario: Criar venda com quantidade acima do permitido
    Given que não existem vendas cadastradas
    When eu tentar criar uma venda com 21 itens
    Then a venda não deve ser criada
    And deve retornar erro de validação

  Scenario: Criar venda com todos os campos obrigatórios
    Given que não existem vendas cadastradas
    When eu criar uma venda com todos os campos preenchidos
    Then a venda deve conter número da venda
    And deve conter data da venda
    And deve conter cliente (ID e nome)
    And deve conter filial (ID e nome)
    And deve conter valor total
    And deve conter status NaoCancelado

