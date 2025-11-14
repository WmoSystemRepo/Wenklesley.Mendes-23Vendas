import { Component, Inject, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TestResult, TestDetails } from '../../../../models/test.model';
import { TestService } from '../../../../services/test.service';
import { Subject, takeUntil } from 'rxjs';

export interface TestScenariosListData {
  tests: TestResult[];
  filterType: 'all' | 'passed' | 'failed';
  title: string;
}

@Component({
  selector: 'app-test-scenarios-list-modal',
  templateUrl: './test-scenarios-list-modal.component.html',
  styleUrls: ['./test-scenarios-list-modal.component.scss']
})
export class TestScenariosListModalComponent implements OnInit, OnDestroy {
  testsWithScenarios: Array<{ test: TestResult; details: TestDetails | null }> = [];
  loading = true;
  totalScenarios = 0;
  private destroy$ = new Subject<void>();

  constructor(
    public dialogRef: MatDialogRef<TestScenariosListModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TestScenariosListData,
    private testService: TestService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadScenarios();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadScenarios(): void {
    this.loading = true;
    const loadPromises = this.data.tests.map(test => {
      return this.testService.getTestScenarios(test.name)
        .pipe(takeUntil(this.destroy$))
        .toPromise()
        .then(details => ({
          test,
          details: details || null
        }))
        .catch(() => ({
          test,
          details: null
        }));
    });

    Promise.all(loadPromises).then(results => {
      this.testsWithScenarios = results;
      this.totalScenarios = results.reduce((sum, item) => 
        sum + (item.details?.scenarios?.length || 0), 0
      );
      this.loading = false;
      this.cdr.detectChanges();
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  getTypeLabel(type: string): string {
    switch (type) {
      case 'unit': return 'Unitário';
      case 'integration': return 'Integração';
      case 'bdd': return 'BDD';
      default: return type;
    }
  }

  getTypeColor(type: string): string {
    switch (type) {
      case 'unit': return 'primary';
      case 'integration': return 'accent';
      case 'bdd': return 'warn';
      default: return '';
    }
  }
}

