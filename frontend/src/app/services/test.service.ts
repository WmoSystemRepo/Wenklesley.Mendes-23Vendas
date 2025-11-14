import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, interval, of } from 'rxjs';
import { map, startWith, switchMap, catchError, tap } from 'rxjs/operators';
import { TestResult, TestStats, TestDetails } from '../models/test.model';
import { TEST_SCENARIOS_MAP } from '../data/test-scenarios.data';

@Injectable({
  providedIn: 'root'
})
export class TestService {
  private readonly baseUrl = '/api/dashboard';

  constructor(private http: HttpClient) {}

  getTests(): Observable<TestResult[]> {
    return this.http.get<TestResult[]>(`${this.baseUrl}/tests`);
  }

  getTestStats(): Observable<TestStats> {
    return this.http.get<TestStats>(`${this.baseUrl}/tests/stats`);
  }

  getTestsStream(): Observable<TestResult[]> {
    return interval(2000).pipe(
      startWith(0),
      switchMap(() => this.getTests())
    );
  }

  runTests(): Observable<any> {
    return this.http.post(`${this.baseUrl}/tests/run`, {});
  }

  clearTests(): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/tests`);
  }

  getTestScenarios(testName: string): Observable<TestDetails> {
    const staticData = TEST_SCENARIOS_MAP[testName];
    
    if (staticData) {
      return of(staticData).pipe(
        map((data: TestDetails) => data)
      );
    }
    
    return this.http.get<TestDetails>(`${this.baseUrl}/tests/${encodeURIComponent(testName)}/scenarios`).pipe(
      map((response: any) => {
        return {
          testName: response.testName || testName,
          type: response.type || 'unknown',
          description: response.description || 'Cenários não disponíveis',
          scenarios: response.scenarios || [],
          file: response.file || 'N/A',
          relatedTests: response.relatedTests
        } as TestDetails;
      }),
      catchError((error: any) => {
        console.warn('Erro ao buscar do backend, tentando fallback:', error);
        const fallbackData = Object.keys(TEST_SCENARIOS_MAP).find(key => 
          key.toLowerCase().includes(testName.toLowerCase()) || 
          testName.toLowerCase().includes(key.toLowerCase())
        );
        
        if (fallbackData) {
          return of(TEST_SCENARIOS_MAP[fallbackData]);
        }
        
        return of({
          testName: testName,
          type: 'unknown',
          description: 'Cenários não disponíveis para este teste',
          scenarios: [],
          file: 'N/A'
        });
      })
    );
  }
}

