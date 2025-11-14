import { Component, Inject, OnInit, OnDestroy, ChangeDetectorRef, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TutorialService } from '../../../../services/tutorial.service';
import { TutorialStep } from '../../../../models/tutorial.model';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-tutorial-overlay',
  templateUrl: './tutorial-overlay.component.html',
  styleUrls: ['./tutorial-overlay.component.scss']
})
export class TutorialOverlayComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('tooltip', { static: false }) tooltipRef!: ElementRef;
  
  currentStep: TutorialStep | null = null;
  currentStepIndex = 0;
  totalSteps = 0;
  highlightElement: HTMLElement | null = null;
  tooltipPosition = { top: 0, left: 0 };
  private destroy$ = new Subject<void>();

  constructor(
    public dialogRef: MatDialogRef<TutorialOverlayComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { steps: TutorialStep[] },
    private tutorialService: TutorialService,
    private cdr: ChangeDetectorRef
  ) {
    this.totalSteps = data.steps.length;
  }

  ngOnInit(): void {
    this.tutorialService.state$
      .pipe(takeUntil(this.destroy$))
      .subscribe(state => {
        if (state && state.isActive) {
          this.currentStepIndex = state.currentStep;
          this.currentStep = state.steps[state.currentStep];
          this.updateHighlight();
        } else {
          this.close();
        }
      });
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.updateHighlight();
    }, 100);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.removeHighlight();
  }

  updateHighlight(): void {
    if (!this.currentStep) return;

    this.removeHighlight();

    const element = document.querySelector(this.currentStep.selector) as HTMLElement;
    if (!element) {
      console.warn('Elemento não encontrado:', this.currentStep.selector);
      return;
    }

    this.highlightElement = element;
    
    element.classList.add('tutorial-highlight');
    
    this.calculateTooltipPosition(element);
    
    if (this.currentStep.action) {
      this.currentStep.action();
    }

    element.scrollIntoView({ behavior: 'smooth', block: 'center' });
    
    this.cdr.detectChanges();
  }

  calculateTooltipPosition(element: HTMLElement): void {
    const rect = element.getBoundingClientRect();
    const padding = 20;
    const tooltipWidth = 400; // Largura estimada do tooltip
    const tooltipHeight = 200; // Altura estimada do tooltip

    switch (this.currentStep?.position) {
      case 'top':
        this.tooltipPosition = {
          top: Math.max(padding, rect.top - tooltipHeight - padding),
          left: Math.max(padding, Math.min(window.innerWidth - tooltipWidth - padding, rect.left + rect.width / 2 - tooltipWidth / 2))
        };
        break;
      case 'bottom':
        this.tooltipPosition = {
          top: Math.min(window.innerHeight - tooltipHeight - padding, rect.bottom + padding),
          left: Math.max(padding, Math.min(window.innerWidth - tooltipWidth - padding, rect.left + rect.width / 2 - tooltipWidth / 2))
        };
        break;
      case 'left':
        this.tooltipPosition = {
          top: Math.max(padding, Math.min(window.innerHeight - tooltipHeight - padding, rect.top + rect.height / 2 - tooltipHeight / 2)),
          left: Math.max(padding, rect.left - tooltipWidth - padding)
        };
        break;
      case 'right':
        this.tooltipPosition = {
          top: Math.max(padding, Math.min(window.innerHeight - tooltipHeight - padding, rect.top + rect.height / 2 - tooltipHeight / 2)),
          left: Math.min(window.innerWidth - tooltipWidth - padding, rect.right + padding)
        };
        break;
      default:
        this.tooltipPosition = {
          top: Math.min(window.innerHeight - tooltipHeight - padding, rect.bottom + padding),
          left: Math.max(padding, Math.min(window.innerWidth - tooltipWidth - padding, rect.left + rect.width / 2 - tooltipWidth / 2))
        };
    }
  }

  removeHighlight(): void {
    if (this.highlightElement) {
      this.highlightElement.classList.remove('tutorial-highlight');
      this.highlightElement = null;
    }
  }

  nextStep(): void {
    this.tutorialService.nextStep();
  }

  previousStep(): void {
    this.tutorialService.previousStep();
  }

  skipTutorial(): void {
    this.tutorialService.finishTutorial();
    this.close();
  }

  close(): void {
    this.removeHighlight();
    this.dialogRef.close();
  }

  getProgress(): number {
    return ((this.currentStepIndex + 1) / this.totalSteps) * 100;
  }
}

