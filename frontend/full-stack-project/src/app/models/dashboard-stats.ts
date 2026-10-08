export interface ProjectStats {
  projectId: string;
  projectName: string;
  totalTasks: number;
  todoTasks: number;
  inProgressTasks: number;
  completedTasks: number;
  cancelledTasks: number;
  overdueTasks: number;
  completionPercentage: number;
}
  
  export interface DashboardStats {
    totalProjects: number;
    totalTasks: number;
    todoTasks: number;
    inProgressTasks: number;
    completedTasks: number;
    cancelledTasks: number;
    overdueTasks: number;
    projects: ProjectStats[];
  }