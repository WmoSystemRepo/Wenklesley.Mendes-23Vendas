export interface TestResult {
  id: string;
  name: string;
  type: 'unit' | 'integration' | 'bdd';
  status: 'passed' | 'failed' | 'running' | 'pending';
  duration?: number;
  message?: string;
  timestamp: Date;
}

export interface TestStats {
  total: number;
  passed: number;
  failed: number;
  running: number;
  pending: number;
  successRate: number;
}

export interface TestSuite {
  name: string;
  tests: TestResult[];
  stats: TestStats;
}

export interface TestScenario {
  id: string;
  description: string;
  expectedResult: string;
}

export interface TestDetails {
  testName: string;
  type: string;
  description: string;
  scenarios: TestScenario[];
  file: string;
  relatedTests?: string[];
}

