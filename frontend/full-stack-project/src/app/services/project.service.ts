import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Project } from '../models/project';

export interface CreateProjectRequest {
  name: string;
  description: string;
  ownerId: string;
}

export interface UpdateProjectRequest {
  name: string;
  description: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:5090/api/projects';

  getAll(): Observable<Project[]> {
    return this.http.get<Project[]>(this.apiUrl);
  }

  getById(id: string): Observable<Project> {
    return this.http.get<Project>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateProjectRequest): Observable<Project> {
    return this.http.post<Project>(this.apiUrl, request);
  }

  update(
    id: string,
    request: UpdateProjectRequest
  ): Observable<Project> {
    return this.http.put<Project>(
      `${this.apiUrl}/${id}`,
      request
    );
  }
  
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
