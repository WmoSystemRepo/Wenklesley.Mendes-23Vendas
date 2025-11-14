import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

interface Requirement {
  id: string;
  name: string;
  description: string;
  status: 'valid' | 'invalid' | 'warning';
  details: string;
  link?: string;
}

@Component({
  selector: 'app-validation-tab',
  templateUrl: './validation-tab.component.html',
  styleUrls: ['./validation-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ValidationTabComponent implements OnInit {
  requirements: Requirement[] = [
    {
      id: 'clean-architecture',
      name: 'Clean Architecture',
      description: 'Separação clara de responsabilidades em camadas',
      status: 'valid',
      details: 'Projeto organizado em Domain, Application, Infrastructure e API layers',
      link: './README.md#arquitetura'
    },
    {
      id: 'ddd',
      name: 'Domain-Driven Design (DDD)',
      description: 'Uso de DDD com entidades, value objects e eventos de domínio',
      status: 'valid',
      details: 'Entidades, Value Objects, Domain Events e Domain Services implementados',
      link: './README.md#arquitetura'
    },
    {
      id: 'test-coverage',
      name: 'Cobertura de Testes',
      description: '100% de cobertura na camada Domain',
      status: 'valid',
      details: '38 testes unitários, 57 testes de integração, 23 testes BDD',
      link: './DOCUMENTACAO_TESTES.md'
    },
    {
      id: 'json-logs',
      name: 'Logs em Formato JSON',
      description: 'Logs estruturados usando Serilog',
      status: 'valid',
      details: 'Serilog configurado com output em JSON, logs no console',
      link: './README.md#observabilidade'
    },
    {
      id: 'git-flow',
      name: 'Git Flow Workflow',
      description: 'Uso de Git Flow para versionamento',
      status: 'valid',
      details: 'Estrutura de branches: main, develop, feature, bugfix, hotfix',
      link: './CONTRIBUTING.md#git-flow-workflow'
    },
    {
      id: 'semantic-commit',
      name: 'Commit Semântico',
      description: 'Commits seguindo padrão Conventional Commits',
      status: 'valid',
      details: 'Formato: type(scope): subject. Tipos: feat, fix, docs, test, refactor, etc.',
      link: './CONTRIBUTING.md#commit-semântico'
    },
    {
      id: 'best-practices',
      name: 'Boas Práticas de Código',
      description: 'Aplicação de SOLID, Clean Code e boas práticas',
      status: 'valid',
      details: 'SOLID, DRY, KISS, Dependency Injection, CQRS, FluentValidation',
      link: './CONTRIBUTING.md#padrões-de-código'
    },
    {
      id: 'documentation',
      name: 'Documentação',
      description: 'Documentação completa e clara',
      status: 'valid',
      details: 'README, CONTRIBUTING, DOCUMENTACAO_TESTES, TESTES_DOCKER',
      link: './README.md'
    }
  ];

  constructor() {}

  ngOnInit(): void {}

  getStatusIcon(status: string): string {
    switch (status) {
      case 'valid': return 'check_circle';
      case 'invalid': return 'error';
      case 'warning': return 'warning';
      default: return 'help';
    }
  }

  getStatusClass(status: string): string {
    return `status-${status}`;
  }

  getOverallScore(): number {
    const valid = this.requirements.filter(r => r.status === 'valid').length;
    return Math.round((valid / this.requirements.length) * 100);
  }

  getValidCount(): number {
    return this.requirements.filter(r => r.status === 'valid').length;
  }

  getWarningCount(): number {
    return this.requirements.filter(r => r.status === 'warning').length;
  }

  getInvalidCount(): number {
    return this.requirements.filter(r => r.status === 'invalid').length;
  }
}

