Feature: Eventos de Domínio
  Como sistema de vendas
  Eu preciso emitir eventos de domínio
  Para rastrear mudanças de estado

  Scenario: CompraEfetuada deve ser emitido ao criar venda
    Given que não existem vendas cadastradas
    When eu criar uma venda com sucesso
    Then o evento CompraEfetuada deve ser emitido
    And deve ser logado em formato JSON
    And deve conter VendaId, NumeroVenda e ValorTotal

  Scenario: CompraAlterada deve ser emitido ao atualizar venda
    Given que existe uma venda cadastrada
    When eu atualizar a venda
    Then o evento CompraAlterada deve ser emitido
    And deve ser logado em formato JSON
    And deve conter VendaId, NumeroVenda e ValorTotal

  Scenario: CompraCancelada deve ser emitido ao cancelar venda
    Given que existe uma venda cadastrada
    When eu cancelar a venda
    Then o evento CompraCancelada deve ser emitido
    And deve ser logado em formato JSON
    And deve conter VendaId e NumeroVenda

  Scenario: ItemCancelado deve ser emitido ao remover item
    Given que existe uma venda com itens cadastrados
    When eu remover um item da venda
    Then o evento ItemCancelado deve ser emitido
    And deve ser logado em formato JSON
    And deve conter VendaId, ItemId e ProdutoNome

