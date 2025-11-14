import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit {
  selectedTab = 0;

  tabs = [
    { label: 'Testes', icon: 'check_circle' },
    { label: 'Logs', icon: 'description' },
    { label: 'Git Flow', icon: 'account_tree' },
    { label: 'API Simulator', icon: 'api' },
    { label: 'Validação', icon: 'verified' }
  ];

  constructor() {}

  ngOnInit(): void {}

  onTabChange(index: number): void {
    this.selectedTab = index;
  }
}

