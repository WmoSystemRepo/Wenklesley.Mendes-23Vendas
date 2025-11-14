export interface LogEntry {
  timestamp: string;
  level: 'Information' | 'Warning' | 'Error' | 'Debug' | 'Trace';
  message: string;
  properties?: Record<string, any>;
  exception?: string;
}

export type LogLevel = LogEntry['level'];

