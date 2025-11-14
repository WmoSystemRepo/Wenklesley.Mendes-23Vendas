import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { GitService } from '../../../../services/git.service';
import { GitBranch, GitFlowStep, GitCommit } from '../../../../models/git.model';

@Component({
  selector: 'app-git-flow-tab',
  templateUrl: './git-flow-tab.component.html',
  styleUrls: ['./git-flow-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GitFlowTabComponent implements OnInit {
  branches: GitBranch[] = [];
  flowSteps: GitFlowStep[] = [];
  commitMessage: string = '';
  commitValidation: GitCommit | null = null;

  constructor(private gitService: GitService) {}

  ngOnInit(): void {
    this.loadBranches();
    this.loadFlowSteps();
  }

  loadBranches(): void {
    this.gitService.getBranches().subscribe(branches => {
      this.branches = branches;
    });
  }

  loadFlowSteps(): void {
    this.gitService.getGitFlowSteps().subscribe(steps => {
      this.flowSteps = steps;
    });
  }

  validateCommit(): void {
    if (!this.commitMessage.trim()) {
      this.commitValidation = null;
      return;
    }

    this.gitService.validateCommit(this.commitMessage).subscribe(validation => {
      this.commitValidation = validation;
    });
  }

  getBranchIcon(type: string): string {
    switch (type) {
      case 'main': return 'star';
      case 'develop': return 'code';
      case 'feature': return 'add_circle';
      case 'bugfix': return 'bug_report';
      case 'hotfix': return 'local_fire_department';
      case 'release': return 'rocket_launch';
      default: return 'folder';
    }
  }

  getCommitTypeIcon(type: string): string {
    switch (type) {
      case 'feat': return 'add_circle';
      case 'fix': return 'bug_report';
      case 'docs': return 'description';
      case 'style': return 'palette';
      case 'refactor': return 'build';
      case 'test': return 'check_circle';
      case 'chore': return 'settings';
      default: return 'help';
    }
  }

  commitExamples = [
    { message: 'feat: adicionar endpoint de criação de venda', description: 'Nova funcionalidade' },
    { message: 'fix: corrigir cálculo de desconto para 5 itens', description: 'Correção de bug' },
    { message: 'docs: atualizar README com instruções Docker', description: 'Documentação' },
    { message: 'test: adicionar testes para VendaService', description: 'Testes' },
    { message: 'refactor: extrair lógica de desconto para serviço', description: 'Refatoração' }
  ];
}

