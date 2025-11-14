import { Component, Inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TestDetails } from '../../../../models/test.model';

@Component({
  selector: 'app-test-details-modal',
  templateUrl: './test-details-modal.component.html',
  styleUrls: ['./test-details-modal.component.scss']
})
export class TestDetailsModalComponent implements OnInit {
  testDetails: TestDetails | null = null;
  loading = true;

  constructor(
    public dialogRef: MatDialogRef<TestDetailsModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { testName: string; testType?: string },
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loading = true;
  }

  setTestDetails(details: TestDetails): void {
    this.testDetails = details;
    this.loading = false;
    this.cdr.detectChanges();
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

