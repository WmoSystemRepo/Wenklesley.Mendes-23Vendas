import { Component, OnInit, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { LogsService } from '../../../../services/logs.service';
import { LogEntry, LogLevel } from '../../../../models/log.model';

@Component({
  selector: 'app-logs-tab',
  templateUrl: './logs-tab.component.html',
  styleUrls: ['./logs-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LogsTabComponent implements OnInit, OnDestroy {
  logs: LogEntry[] = [];
  filteredLogs: LogEntry[] = [];
  selectedLevel: LogLevel | 'all' = 'all';
  searchTerm: string = '';
  autoScroll: boolean = true;
  maxLogs: number = 500;
  private destroy$ = new Subject<void>();

  constructor(private logsService: LogsService) {}

  ngOnInit(): void {
    this.loadLogs();
    
    this.logsService.getLogsStream()
      .pipe(takeUntil(this.destroy$))
      .subscribe(logs => {
        this.logs = logs.slice(-this.maxLogs);
        this.applyFilters();
        if (this.autoScroll) {
          setTimeout(() => this.scrollToBottom(), 100);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadLogs(): void {
    this.logsService.getLogs()
      .pipe(takeUntil(this.destroy$))
      .subscribe(logs => {
        this.logs = logs.slice(-this.maxLogs);
        this.applyFilters();
      });
  }

  applyFilters(): void {
    let filtered = [...this.logs];
    
    if (this.selectedLevel !== 'all') {
      filtered = filtered.filter(log => log.level === this.selectedLevel);
    }
    
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(log => 
        log.message.toLowerCase().includes(term) ||
        JSON.stringify(log.properties || {}).toLowerCase().includes(term)
      );
    }
    
    this.filteredLogs = filtered;
  }

  onLevelChange(): void {
    this.applyFilters();
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  clearLogs(): void {
    this.logsService.clearLogs()
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.logs = [];
        this.filteredLogs = [];
      });
  }

  scrollToBottom(): void {
    const container = document.querySelector('.logs-container');
    if (container) {
      container.scrollTop = container.scrollHeight;
    }
  }

  getLevelClass(level: LogLevel): string {
    return `log-level-${level.toLowerCase()}`;
  }

  formatJson(obj: any): string {
    return JSON.stringify(obj, null, 2);
  }

  trackByLogId(index: number, log: LogEntry): string {
    return `${log.timestamp}-${index}`;
  }
}

