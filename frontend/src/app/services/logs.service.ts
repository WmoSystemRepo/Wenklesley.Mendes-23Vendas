import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, interval } from 'rxjs';
import { startWith, switchMap } from 'rxjs/operators';
import { LogEntry, LogLevel } from '../models/log.model';

@Injectable({
  providedIn: 'root'
})
export class LogsService {
  private readonly baseUrl = '/api/dashboard';

  constructor(private http: HttpClient) {}

  getLogs(level?: LogLevel, limit: number = 100): Observable<LogEntry[]> {
    let url = `${this.baseUrl}/logs?limit=${limit}`;
    if (level) {
      url += `&level=${level}`;
    }
    return this.http.get<LogEntry[]>(url);
  }

  getLogsStream(level?: LogLevel): Observable<LogEntry[]> {
    return interval(1000).pipe(
      startWith(0),
      switchMap(() => this.getLogs(level, 50))
    );
  }

  clearLogs(): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/logs`);
  }
}

