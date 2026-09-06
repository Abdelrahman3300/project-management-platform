export interface TaskItem {
    id: string;
    title: string;
    description: string | null;
    status: string;
    priority: string;
    createdAt: string;
    dueDate: string | null;
    projectId: string;
    assignedToUserId: string | null;
    assignedToUserName: string | null;
  }