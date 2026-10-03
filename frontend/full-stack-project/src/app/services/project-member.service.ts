import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ProjectMember } from '../models/project-member';
import { AddProjectMemberRequest } from '../models/add-project-member-request';

@Injectable({
  providedIn: 'root'
})
export class ProjectMemberService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'http://localhost:5090/api/projects';

  getMembers(projectId: string): Observable<ProjectMember[]> {
    return this.http.get<ProjectMember[]>(
      `${this.apiUrl}/${projectId}/members`
    );
  }

  addMember(
    projectId: string,
    request: AddProjectMemberRequest
  ): Observable<ProjectMember> {
    return this.http.post<ProjectMember>(
      `${this.apiUrl}/${projectId}/members`,
      request
    );
  }

  removeMember(
    projectId: string,
    userId: string
  ): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${projectId}/members/${userId}`
    );
  }
}