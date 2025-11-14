import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiRequest, ApiResponse } from '../models/api.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = '/api';

  constructor(private http: HttpClient) {}

  executeRequest(request: ApiRequest): Observable<ApiResponse> {
    const startTime = Date.now();
    const url = request.url.startsWith('/api') ? request.url : `${this.baseUrl}${request.url}`;
    
    let httpRequest: Observable<any>;
    const headers = new HttpHeaders(request.headers || {});

    switch (request.method) {
      case 'GET':
        httpRequest = this.http.get(url, { headers, observe: 'response' });
        break;
      case 'POST':
        httpRequest = this.http.post(url, request.body, { headers, observe: 'response' });
        break;
      case 'PUT':
        httpRequest = this.http.put(url, request.body, { headers, observe: 'response' });
        break;
      case 'DELETE':
        httpRequest = this.http.delete(url, { headers, observe: 'response' });
        break;
      default:
        throw new Error(`Unsupported method: ${request.method}`);
    }

    return new Observable(observer => {
      httpRequest.subscribe({
        next: (response: any) => {
          const duration = Date.now() - startTime;
          const apiResponse: ApiResponse = {
            status: response.status,
            statusText: response.statusText,
            headers: this.extractHeaders(response.headers),
            body: response.body,
            duration,
            timestamp: new Date()
          };
          observer.next(apiResponse);
          observer.complete();
        },
        error: (error: any) => {
          const duration = Date.now() - startTime;
          const apiResponse: ApiResponse = {
            status: error.status || 0,
            statusText: error.statusText || 'Error',
            headers: this.extractHeaders(error.headers),
            body: error.error || { message: error.message },
            duration,
            timestamp: new Date()
          };
          observer.next(apiResponse);
          observer.complete();
        }
      });
    });
  }

  private extractHeaders(headers: any): Record<string, string> {
    const result: Record<string, string> = {};
    if (headers) {
      headers.keys().forEach((key: string) => {
        result[key] = headers.get(key);
      });
    }
    return result;
  }
}

