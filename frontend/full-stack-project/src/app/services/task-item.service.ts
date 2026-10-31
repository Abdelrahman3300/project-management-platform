import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { TaskItem } from '../models/task-item';

@Injectable({
  providedIn: 'root'
})
export class TaskItemService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = '/api/projects';

  getByProjectId(projectId: string): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(
      `${this.apiUrl}/${projectId}/tasks`
    );
  }

  getById(projectId: string, taskId: string): Observable<TaskItem> {
    return this.http.get<TaskItem>(
      `${this.apiUrl}/${projectId}/tasks/${taskId}`
    );
  }

  create(
    projectId: string,
    task: {
      title: string;
      description: string | null;
      status: string;
      priority: string;
      dueDate: string | null;
      assignedToUserId: string | null;
    }
  ): Observable<TaskItem> {
    return this.http.post<TaskItem>(
      `${this.apiUrl}/${projectId}/tasks`,
      task
    );
  }

  update(
    projectId: string,
    taskId: string,
    task: {
      title: string;
      description: string | null;
      status: string;
      priority: string;
      dueDate: string | null;
      assignedToUserId: string | null;
    }
  ): Observable<TaskItem> {
    return this.http.put<TaskItem>(
      `${this.apiUrl}/${projectId}/tasks/${taskId}`,
      task
    );
  }

  delete(projectId: string, taskId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${projectId}/tasks/${taskId}`
    );
  }
}