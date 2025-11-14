import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ApiService } from '../../../../services/api.service';
import { ApiRequest, ApiResponse, ApiHistoryItem, ApiEndpoint } from '../../../../models/api.model';

@Component({
  selector: 'app-api-simulator-tab',
  templateUrl: './api-simulator-tab.component.html',
  styleUrls: ['./api-simulator-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ApiSimulatorTabComponent implements OnInit {
  selectedMethod: 'GET' | 'POST' | 'PUT' | 'DELETE' = 'GET';
  selectedEndpoint: string = '';
  requestBody: string = '{}';
  response: ApiResponse | null = null;
  history: ApiHistoryItem[] = [];
  loading: boolean = false;

  endpoints: ApiEndpoint[] = [
    { method: 'GET', path: '/api/venda', description: 'Listar todas as vendas' },
    { method: 'GET', path: '/api/venda/{id}', description: 'Obter venda por ID', exampleRequest: { id: 'guid-aqui' } },
    { method: 'POST', path: '/api/venda', description: 'Criar nova venda', exampleRequest: {
      numeroVenda: 'V001',
      clienteId: 'guid',
      clienteNome: 'Cliente Teste',
      filialId: 'guid',
      filialNome: 'Filial Teste',
      itens: [{ produtoId: 'guid', produtoNome: 'Produto', quantidade: 5, valorUnitario: 100 }]
    }},
    { method: 'PUT', path: '/api/venda/{id}', description: 'Atualizar venda', exampleRequest: {
      itensParaAdicionar: [],
      itensParaRemover: [],
      itensParaAtualizar: []
    }},
    { method: 'DELETE', path: '/api/venda/{id}', description: 'Cancelar venda' },
    { method: 'GET', path: '/health', description: 'Health check' }
  ];

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadHistory();
  }

  onEndpointSelect(): void {
    const endpoint = this.endpoints.find(e => e.path === this.selectedEndpoint);
    if (endpoint) {
      this.selectedMethod = endpoint.method;
      if (endpoint.exampleRequest) {
        this.requestBody = JSON.stringify(endpoint.exampleRequest, null, 2);
      } else {
        this.requestBody = '{}';
      }
    }
  }

  executeRequest(): void {
    if (!this.selectedEndpoint) {
      alert('Selecione um endpoint');
      return;
    }

    this.loading = true;
    let url = this.selectedEndpoint;
    
    if (url.includes('{id}')) {
      const id = prompt('Digite o ID:');
      if (!id) {
        this.loading = false;
        return;
      }
      url = url.replace('{id}', id);
    }

    let body: any = null;
    try {
      if (this.requestBody && this.requestBody.trim() !== '{}') {
        body = JSON.parse(this.requestBody);
      }
    } catch (e) {
      alert('JSON inválido no body');
      this.loading = false;
      return;
    }

    const request: ApiRequest = {
      method: this.selectedMethod,
      url: url,
      body: body,
      headers: {
        'Content-Type': 'application/json'
      }
    };

    this.apiService.executeRequest(request).subscribe({
      next: (response) => {
        this.response = response;
        this.addToHistory(request, response);
        this.loading = false;
      },
      error: (error) => {
        console.error(error);
        this.loading = false;
      }
    });
  }

  addToHistory(request: ApiRequest, response: ApiResponse): void {
    const item: ApiHistoryItem = {
      id: Date.now().toString(),
      request,
      response,
      timestamp: new Date()
    };
    this.history.unshift(item);
    if (this.history.length > 50) {
      this.history = this.history.slice(0, 50);
    }
    this.saveHistory();
  }

  loadHistory(): void {
    const saved = localStorage.getItem('api-history');
    if (saved) {
      try {
        this.history = JSON.parse(saved);
      } catch (e) {
        this.history = [];
      }
    }
  }

  saveHistory(): void {
    localStorage.setItem('api-history', JSON.stringify(this.history));
  }

  clearHistory(): void {
    this.history = [];
    this.saveHistory();
  }

  formatJson(obj: any): string {
    return JSON.stringify(obj, null, 2);
  }

  getStatusClass(status: number): string {
    if (status >= 200 && status < 300) return 'status-success';
    if (status >= 400 && status < 500) return 'status-client-error';
    if (status >= 500) return 'status-server-error';
    return 'status-info';
  }

  loadHistoryItem(item: ApiHistoryItem): void {
    this.selectedMethod = item.request.method;
    this.selectedEndpoint = item.request.url;
    this.requestBody = item.request.body ? JSON.stringify(item.request.body, null, 2) : '{}';
    this.response = item.response;
  }

  trackByHistoryId(index: number, item: ApiHistoryItem): string {
    return item.id;
  }
}

