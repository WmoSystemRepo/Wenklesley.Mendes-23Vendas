export interface TutorialStep {
  id: string;
  title: string;
  description: string;
  selector: string; // CSS selector do elemento
  position: 'top' | 'bottom' | 'left' | 'right';
  action?: () => void; // Ação opcional (ex: clicar, focar)
}

export interface TutorialState {
  currentStep: number;
  isActive: boolean;
  steps: TutorialStep[];
}

