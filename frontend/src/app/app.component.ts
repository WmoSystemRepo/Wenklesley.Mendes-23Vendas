import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: '<app-dashboard></app-dashboard>',
  styles: [':host { display: block; height: 100%; }']
})
export class AppComponent {
  title = '123Vendas Dashboard';
}

