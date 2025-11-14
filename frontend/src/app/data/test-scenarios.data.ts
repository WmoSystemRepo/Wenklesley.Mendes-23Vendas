import { TestDetails } from '../models/test.model';

export const TEST_SCENARIOS_MAP: Record<string, TestDetails> = {
  'CriarVenda_DeveCriarVendaComStatusNaoCancelado': {
    testName: 'CriarVenda_DeveCriarVendaComStatusNaoCancelado',
    type: 'unit',
    description: 'Verifica criação de venda com status inicial correto',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Criação de venda',
        expectedResult: 'Venda criada com status NaoCancelado'
      }
    ],
    file: 'tests/UnitTests/Domain/Entities/VendaTests.cs'
  },
  'AdicionarItem_DeveAdicionarItemEVincularAVenda': {
    testName: 'AdicionarItem_DeveAdicionarItemEVincularAVenda',
    type: 'unit',
    description: 'Verifica adição de item à venda',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Adicionar item à venda',
        expectedResult: 'Item adicionado e vinculado à venda'
      }
    ],
    file: 'tests/UnitTests/Domain/Entities/VendaTests.cs'
  },
  'AdicionarItem_DeveRecalcularValorTotal': {
    testName: 'AdicionarItem_DeveRecalcularValorTotal',
    type: 'unit',
    description: 'Verifica recálculo automático do valor total',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Adicionar item',
        expectedResult: 'Valor total recalculado automaticamente'
      }
    ],
    file: 'tests/UnitTests/Domain/Entities/VendaTests.cs'
  },
  'AdicionarItem_DeveEmitirEventoCompraEfetuada': {
    testName: 'AdicionarItem_DeveEmitirEventoCompraEfetuada',
    type: 'unit',
    description: 'Verifica emissão de evento de domínio',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Adicionar item',
        expectedResult: 'Evento CompraEfetuada emitido'
      }
    ],
    file: 'tests/UnitTests/Domain/Entities/VendaTests.cs'
  },
  'RemoverItem_DeveRemoverItemEVincularAVenda': {
    testName: 'RemoverItem_DeveRemoverItemEVincularAVenda',
    type: 'unit',
    description: 'Verifica remoção de item da venda',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Remover item da venda',
        expectedResult: 'Item removido da venda'
      }
    ],
    file: 'tests/UnitTests/Domain/Entities/VendaTests.cs'
  },
  'Cancelar_DeveAlterarStatusParaCancelado': {
    testName: 'Cancelar_DeveAlterarStatusParaCancelado',
    type: 'unit',
    description: 'Verifica cancelamento de venda',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Cancelar venda',
        expectedResult: 'Status alterado para Cancelado'
      }
    ],
    file: 'tests/UnitTests/Domain/Entities/VendaTests.cs'
  },
  'CriarItem_QuantidadeEntre5E9_DeveAplicar10PorcentoDesconto': {
    testName: 'CriarItem_QuantidadeEntre5E9_DeveAplicar10PorcentoDesconto',
    type: 'unit',
    description: 'Verifica desconto de 10% para 5-9 itens',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Criar item com quantidade entre 5 e 9',
        expectedResult: 'Desconto de 10% aplicado'
      }
    ],
    file: 'tests/UnitTests/Domain/Entities/VendaItemTests.cs'
  },
  'CriarItem_QuantidadeEntre10E20_DeveAplicar20PorcentoDesconto': {
    testName: 'CriarItem_QuantidadeEntre10E20_DeveAplicar20PorcentoDesconto',
    type: 'unit',
    description: 'Verifica desconto de 20% para 10-20 itens',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Criar item com quantidade entre 10 e 20',
        expectedResult: 'Desconto de 20% aplicado'
      }
    ],
    file: 'tests/UnitTests/Domain/Entities/VendaItemTests.cs'
  },
  'Handle_DeveCriarVendaComSucesso': {
    testName: 'Handle_DeveCriarVendaComSucesso',
    type: 'unit',
    description: 'Verifica criação de venda através do handler',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Criar venda via handler',
        expectedResult: 'Venda criada com sucesso'
      }
    ],
    file: 'tests/UnitTests/Application/Handlers/CreateVendaHandlerTests.cs'
  },
  'CriarVenda_DeveCriarVendaComSucesso': {
    testName: 'CriarVenda_DeveCriarVendaComSucesso',
    type: 'unit',
    description: 'Verifica criação de venda com sucesso',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Criar venda com dados válidos',
        expectedResult: 'Venda criada com sucesso e ID retornado'
      },
      {
        id: 'scenario-2',
        description: 'Validar que o repositório foi chamado',
        expectedResult: 'Método AddAsync foi invocado'
      },
      {
        id: 'scenario-3',
        description: 'Validar que as mudanças foram salvas',
        expectedResult: 'SaveChangesAsync foi chamado e retornou sucesso'
      }
    ],
    file: 'tests/UnitTests/Application/Handlers/CreateVendaHandlerTests.cs'
  },
  'Validar_NumeroVendaVazio_DeveFalhar': {
    testName: 'Validar_NumeroVendaVazio_DeveFalhar',
    type: 'unit',
    description: 'Verifica validação de número de venda obrigatório',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Tentar criar venda sem número',
        expectedResult: 'Validação deve falhar com erro'
      },
      {
        id: 'scenario-2',
        description: 'Verificar mensagem de erro retornada',
        expectedResult: 'Mensagem de erro deve indicar campo obrigatório'
      }
    ],
    file: 'tests/UnitTests/Application/Validators/CreateVendaCommandValidatorTests.cs'
  },
  'Post_CriarVenda_DeveRetornar201CreatedComId': {
    testName: 'Post_CriarVenda_DeveRetornar201CreatedComId',
    type: 'integration',
    description: 'Verifica criação com retorno de ID',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Criar venda via API POST',
        expectedResult: 'Status 201 Created com ID retornado'
      }
    ],
    file: 'tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs'
  },
  'Post_CriarVenda_DeveCalcularDescontosPorItem': {
    testName: 'Post_CriarVenda_DeveCalcularDescontosPorItem',
    type: 'integration',
    description: 'Verifica cálculo de descontos',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Criar venda com itens que recebem desconto',
        expectedResult: 'Descontos calculados corretamente por item'
      }
    ],
    file: 'tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs'
  },
  'Get_ObterVendaPorId_DeveRetornarVendaExistente': {
    testName: 'Get_ObterVendaPorId_DeveRetornarVendaExistente',
    type: 'integration',
    description: 'Verifica obtenção por ID',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Obter venda existente via GET',
        expectedResult: 'Venda retornada com sucesso'
      }
    ],
    file: 'tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs'
  },
  'Put_AtualizarVenda_DeveAtualizarItens': {
    testName: 'Put_AtualizarVenda_DeveAtualizarItens',
    type: 'integration',
    description: 'Verifica atualização de itens',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Atualizar venda via PUT',
        expectedResult: 'Itens atualizados com sucesso'
      }
    ],
    file: 'tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs'
  },
  'Delete_CancelarVenda_DeveAlterarStatusParaCancelado': {
    testName: 'Delete_CancelarVenda_DeveAlterarStatusParaCancelado',
    type: 'integration',
    description: 'Verifica cancelamento',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Cancelar venda via DELETE',
        expectedResult: 'Status alterado para Cancelado'
      }
    ],
    file: 'tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs'
  },
  'Post_Com5Itens_DeveAplicar10PorcentoDesconto': {
    testName: 'Post_Com5Itens_DeveAplicar10PorcentoDesconto',
    type: 'integration',
    description: 'Verifica desconto de 10% para 5 itens',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Criar venda com 5 itens',
        expectedResult: 'Desconto de 10% aplicado'
      }
    ],
    file: 'tests/IntegrationTests/Controllers/RegrasDeNegocioIntegrationTests.cs'
  },
  'Post_Com10Itens_DeveAplicar20PorcentoDesconto': {
    testName: 'Post_Com10Itens_DeveAplicar20PorcentoDesconto',
    type: 'integration',
    description: 'Verifica desconto de 20% para 10 itens',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Criar venda com 10 itens',
        expectedResult: 'Desconto de 20% aplicado'
      }
    ],
    file: 'tests/IntegrationTests/Controllers/RegrasDeNegocioIntegrationTests.cs'
  },
  'Criar venda com sucesso': {
    testName: 'Criar venda com sucesso',
    type: 'bdd',
    description: 'Cria venda com dados válidos e verifica sucesso',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Criar venda com dados válidos',
        expectedResult: 'Venda criada com sucesso'
      }
    ],
    file: 'tests/BddTests/Features/CriarVenda.feature'
  },
  'Criar venda com quantidade acima do permitido': {
    testName: 'Criar venda com quantidade acima do permitido',
    type: 'bdd',
    description: 'Tenta criar venda com 21 itens e verifica erro',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Tentar criar venda com 21 itens',
        expectedResult: 'Erro retornado (quantidade > 20 não permitida)'
      }
    ],
    file: 'tests/BddTests/Features/CriarVenda.feature'
  },
  'Aplicar desconto baseado na quantidade': {
    testName: 'Aplicar desconto baseado na quantidade',
    type: 'bdd',
    description: 'Testa descontos para quantidades de 1 a 20 itens',
    scenarios: [
      {
        id: 'scenario-1',
        description: 'Testar descontos para diferentes quantidades',
        expectedResult: 'Descontos aplicados corretamente: 0% (1-4), 10% (5-9), 20% (10-20)'
      }
    ],
    file: 'tests/BddTests/Features/RegrasDeDesconto.feature'
  }
};

