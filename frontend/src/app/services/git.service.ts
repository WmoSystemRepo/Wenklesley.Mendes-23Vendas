import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GitCommit, GitBranch, GitFlowStep } from '../models/git.model';

@Injectable({
  providedIn: 'root'
})
export class GitService {
  private readonly baseUrl = '/api/dashboard';

  constructor(private http: HttpClient) {}

  getGitInfo(): Observable<any> {
    return this.http.get(`${this.baseUrl}/git-info`);
  }

  validateCommit(message: string): Observable<GitCommit> {
    return this.http.post<GitCommit>(`${this.baseUrl}/git/validate-commit`, { message });
  }

  getBranches(): Observable<GitBranch[]> {
    return this.http.get<GitBranch[]>(`${this.baseUrl}/git/branches`);
  }

  getGitFlowSteps(): Observable<GitFlowStep[]> {
    return this.http.get<GitFlowStep[]>(`${this.baseUrl}/git/flow-steps`);
  }
}

