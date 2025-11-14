export interface GitCommit {
  hash: string;
  type: 'feat' | 'fix' | 'docs' | 'style' | 'refactor' | 'test' | 'chore';
  scope?: string;
  subject: string;
  body?: string;
  footer?: string;
  isValid: boolean;
}

export interface GitBranch {
  name: string;
  type: 'main' | 'develop' | 'feature' | 'bugfix' | 'hotfix' | 'release';
  description: string;
  example: string;
}

export interface GitFlowStep {
  step: number;
  title: string;
  description: string;
  command?: string;
  branch?: string;
}

