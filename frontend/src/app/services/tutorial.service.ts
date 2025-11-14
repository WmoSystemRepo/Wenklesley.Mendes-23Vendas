import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { TutorialStep, TutorialState } from '../models/tutorial.model';

@Injectable({
  providedIn: 'root'
})
export class TutorialService {
  private readonly STORAGE_KEY = 'tutorial_state';
  private stateSubject = new BehaviorSubject<TutorialState | null>(null);
  public state$: Observable<TutorialState | null> = this.stateSubject.asObservable();

  constructor() {
    this.loadState();
  }

  startTutorial(steps: TutorialStep[]): void {
    const state: TutorialState = {
      currentStep: 0,
      isActive: true,
      steps: steps
    };
    this.saveState(state);
    this.stateSubject.next(state);
  }

  nextStep(): void {
    const current = this.stateSubject.value;
    if (!current || !current.isActive) return;

    if (current.currentStep < current.steps.length - 1) {
      const newState = {
        ...current,
        currentStep: current.currentStep + 1
      };
      this.saveState(newState);
      this.stateSubject.next(newState);
    } else {
      this.finishTutorial();
    }
  }

  previousStep(): void {
    const current = this.stateSubject.value;
    if (!current || !current.isActive) return;

    if (current.currentStep > 0) {
      const newState = {
        ...current,
        currentStep: current.currentStep - 1
      };
      this.saveState(newState);
      this.stateSubject.next(newState);
    }
  }

  finishTutorial(): void {
    const current = this.stateSubject.value;
    if (current) {
      const newState = {
        ...current,
        isActive: false
      };
      this.saveState(newState);
      this.stateSubject.next(newState);
    }
    setTimeout(() => {
      localStorage.removeItem(this.STORAGE_KEY);
      this.stateSubject.next(null);
    }, 1000);
  }

  getCurrentStep(): TutorialStep | null {
    const state = this.stateSubject.value;
    if (!state || !state.isActive) return null;
    return state.steps[state.currentStep] || null;
  }

  getCurrentState(): TutorialState | null {
    return this.stateSubject.value;
  }

  private saveState(state: TutorialState): void {
    try {
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(state));
    } catch (error) {
      console.warn('Erro ao salvar estado do tutorial:', error);
    }
  }

  private loadState(): void {
    try {
      const saved = localStorage.getItem(this.STORAGE_KEY);
      if (saved) {
        const state = JSON.parse(saved) as TutorialState;
        if (state.isActive) {
          this.stateSubject.next(state);
        }
      }
    } catch (error) {
      console.warn('Erro ao carregar estado do tutorial:', error);
    }
  }
}

