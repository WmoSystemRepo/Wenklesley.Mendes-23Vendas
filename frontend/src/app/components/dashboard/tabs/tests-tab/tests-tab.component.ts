import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { tap } from 'rxjs/operators';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TestService } from '../../../../services/test.service';
import { TestResult, TestStats, TestDetails } from '../../../../models/test.model';
import { TestDetailsModalComponent } from './test-details-modal.component';
import { TestScenariosListModalComponent, TestScenariosListData } from './test-scenarios-list-modal.component';
import { TutorialService } from '../../../../services/tutorial.service';
import { TutorialOverlayComponent } from './tutorial-overlay.component';

@Component({
  selector: 'app-tests-tab',
  templateUrl: './tests-tab.component.html',
  styleUrls: ['./tests-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestsTabComponent implements OnInit, OnDestroy {
  tests: TestResult[] = [];
  stats: TestStats = {
    total: 0,
    passed: 0,
    failed: 0,
    running: 0,
    pending: 0,
    successRate: 0
  };
  selectedFilter: 'all' | 'unit' | 'integration' | 'bdd' = 'all';
  private destroy$ = new Subject<void>();

  constructor(
    private testService: TestService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
    private tutorialService: TutorialService
  ) {}

  ngOnInit(): void {
    this.loadTests();
    this.loadStats();
    
    this.testService.getTestsStream()
      .pipe(takeUntil(this.destroy$))
      .subscribe(tests => {
        this.tests = tests;
        this.updateStats();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadTests(): void {
    this.testService.getTests()
      .pipe(takeUntil(this.destroy$))
      .subscribe(tests => {
        this.tests = tests;
        this.updateStats();
      });
  }

  loadStats(): void {
    this.testService.getTestStats()
      .pipe(takeUntil(this.destroy$))
      .subscribe(stats => {
        this.stats = stats;
      });
  }

  updateStats(): void {
    const filtered = this.getFilteredTests();
    this.stats = {
      total: filtered.length,
      passed: filtered.filter(t => t.status === 'passed').length,
      failed: filtered.filter(t => t.status === 'failed').length,
      running: filtered.filter(t => t.status === 'running').length,
      pending: filtered.filter(t => t.status === 'pending').length,
      successRate: filtered.length > 0 
        ? Math.round((filtered.filter(t => t.status === 'passed').length / filtered.length) * 100)
        : 0
    };
  }

  getFilteredTests(): TestResult[] {
    if (this.selectedFilter === 'all') {
      return this.tests;
    }
    return this.tests.filter(t => t.type === this.selectedFilter);
  }

  runTests(): void {
    this.testService.runTests()
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.loadTests();
      });
  }

  getStatusClass(status: string): string {
    return `status-${status}`;
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'passed': return 'check_circle';
      case 'failed': return 'error';
      case 'running': return 'sync';
      case 'pending': return 'schedule';
      default: return 'help';
    }
  }

  trackByTestId(index: number, test: TestResult): string {
    return test.id;
  }

  clearTests(): void {
    this.tests = [];
    
    this.stats = {
      total: 0,
      passed: 0,
      failed: 0,
      running: 0,
      pending: 0,
      successRate: 0
    };
    
    this.testService.clearTests()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Testes limpos com sucesso', 'Fechar', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.cdr.markForCheck();
        },
        error: () => {
          this.snackBar.open('Erro ao limpar testes', 'Fechar', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
        }
      });
  }

  openTestDetails(test: TestResult): void {
    const dialogRef = this.dialog.open(TestDetailsModalComponent, {
      width: '700px',
      maxWidth: '90vw',
      data: { testName: test.name, testType: test.type },
      disableClose: false
    });

    const modalComponent = dialogRef.componentInstance;
    modalComponent.loading = true;

    this.testService.getTestScenarios(test.name)
      .pipe(
        takeUntil(this.destroy$),
        tap((details: TestDetails) => console.log('Cenários carregados:', details))
      )
      .subscribe({
        next: (details: TestDetails) => {
          console.log('Recebendo detalhes:', details);
          if (!details.type || details.type === 'unknown') {
            details.type = test.type;
          }
          setTimeout(() => {
            modalComponent.setTestDetails(details);
          }, 0);
        },
        error: (error) => {
          console.error('Erro ao carregar cenários:', error);
          setTimeout(() => {
            modalComponent.setTestDetails({
              testName: test.name,
              type: test.type,
              description: 'Cenários não disponíveis para este teste',
              scenarios: [],
              file: 'N/A'
            });
          }, 0);
        }
      });
  }

  openFilteredScenariosModal(status: 'all' | 'passed' | 'failed'): void {
    let filteredTests: TestResult[];
    let title: string;

    switch (status) {
      case 'all':
        filteredTests = this.tests;
        title = 'Cenários de Todos os Testes';
        break;
      case 'passed':
        filteredTests = this.tests.filter(t => t.status === 'passed');
        title = 'Cenários dos Testes que Passaram';
        break;
      case 'failed':
        filteredTests = this.tests.filter(t => t.status === 'failed');
        title = 'Cenários dos Testes que Falharam';
        break;
      default:
        filteredTests = this.tests;
        title = 'Cenários dos Testes';
    }

    if (filteredTests.length === 0) {
      this.snackBar.open('Nenhum teste encontrado para este filtro', 'Fechar', {
        duration: 3000,
        horizontalPosition: 'end',
        verticalPosition: 'top'
      });
      return;
    }

    const dialogRef = this.dialog.open(TestScenariosListModalComponent, {
      width: '900px',
      maxWidth: '95vw',
      maxHeight: '90vh',
      data: {
        tests: filteredTests,
        filterType: status,
        title: title
      } as TestScenariosListData,
      disableClose: false
    });
  }

  startTutorial(): void {
    const steps = [
      {
        id: 'stat-total',
        title: 'Total de Testes',
        description: 'Este card mostra o total de testes executados. Clique para ver todos os cenários.',
        selector: '.stat-card:first-child',
        position: 'bottom' as const
      },
      {
        id: 'stat-passed',
        title: 'Testes que Passaram',
        description: 'Este card mostra quantos testes passaram. Clique para ver cenários dos testes que passaram.',
        selector: '.stat-card.success',
        position: 'bottom' as const
      },
      {
        id: 'stat-failed',
        title: 'Testes que Falharam',
        description: 'Este card mostra quantos testes falharam. Clique para ver cenários dos testes que falharam.',
        selector: '.stat-card.failed',
        position: 'bottom' as const
      },
      {
        id: 'filter-type',
        title: 'Filtrar por Tipo',
        description: 'Use este filtro para ver apenas testes unitários, de integração ou BDD.',
        selector: 'mat-form-field',
        position: 'bottom' as const
      },
      {
        id: 'btn-run',
        title: 'Executar Testes',
        description: 'Clique aqui para executar todos os testes.',
        selector: 'button[color="primary"]',
        position: 'bottom' as const
      },
      {
        id: 'btn-clear',
        title: 'Limpar Testes',
        description: 'Clique aqui para limpar a lista de testes.',
        selector: 'button[color="warn"]',
        position: 'bottom' as const
      },
      {
        id: 'test-card',
        title: 'Card de Teste',
        description: 'Clique em qualquer teste para ver seus cenários detalhados.',
        selector: '.test-card:first-child',
        position: 'right' as const
      }
    ];

    this.tutorialService.startTutorial(steps);
    
    const overlayRef = this.dialog.open(TutorialOverlayComponent, {
      width: '100%',
      height: '100%',
      maxWidth: '100vw',
      maxHeight: '100vh',
      panelClass: 'tutorial-overlay-panel',
      disableClose: true,
      data: { steps: steps }
    });

    overlayRef.afterClosed().subscribe(() => {
      this.tutorialService.finishTutorial();
    });
  }
}

