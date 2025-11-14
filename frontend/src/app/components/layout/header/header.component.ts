import { Component } from '@angular/core';

@Component({
  selector: 'app-header',
  template: `
    <mat-toolbar color="primary">
      <mat-icon>dashboard</mat-icon>
      <span class="title">123Vendas Dashboard</span>
      <span class="spacer"></span>
      <span class="subtitle">Sistema de Gerenciamento de Vendas</span>
    </mat-toolbar>
  `,
  styles: [`
    mat-toolbar {
      position: sticky;
      top: 0;
      z-index: 1000;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    
    .title {
      margin-left: 8px;
      font-size: 20px;
      font-weight: 500;
    }
    
    .spacer {
      flex: 1 1 auto;
    }
    
    .subtitle {
      font-size: 14px;
      opacity: 0.9;
    }
    
    mat-icon {
      margin-right: 8px;
    }
  `]
})
export class HeaderComponent {}

