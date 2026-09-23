import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Comment } from '../models/comment';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'http://localhost:5090/api';

  getByTaskId(taskId: string): Observable<Comment[]> {
    return this.http.get<Comment[]>(
      `${this.apiUrl}/tasks/${taskId}/comments`
    );
  }

  add(
    taskId: string,
    content: string
  ): Observable<Comment> {
    return this.http.post<Comment>(
      `${this.apiUrl}/tasks/${taskId}/comments`,
      {
        content
      }
    );
  }

  delete(commentId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/comments/${commentId}`
    );
  }
}