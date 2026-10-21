import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Project } from './models/project';
import { Router, RouterOutlet } from '@angular/router';

import { AuthService } from './services/auth.service';

import { NotificationsComponent } from './shared/notifications/notifications.component';

import {
  CreateProjectRequest,
  ProjectService,
  UpdateProjectRequest
} from './services/project.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe, RouterOutlet, NotificationsComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {

  private readonly projectService = inject(ProjectService);
  private readonly router = inject(Router);
  readonly authService = inject(AuthService);

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  isProjectDetailsPage(): boolean {
    return this.router.url.startsWith('/projects/');
  }

  projects: Project[] = [];

  loading = true;
  creating = false;
  editing = false;
  updating = false;

  error = '';

  newProject: CreateProjectRequest = {
    name: '',
    description: '',
    ownerId: '3feb00eb-4c36-4a03-80bd-613771a15eb3'
  };

  editingProject: UpdateProjectRequest = {
    name: '',
    description: ''
  };

  editingProjectId = '';

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading = true;
    this.error = '';

    this.projectService.getAll().subscribe({
      next: (projects) => {
        this.projects = projects;
        this.loading = false;
      },
      error: (error) => {
        console.error(error);
        this.error = 'Failed to load projects.';
        this.loading = false;
      }
    });
  }

  createProject(): void {

    if (!this.newProject.name.trim()) {
      return;
    }

    this.creating = true;
    this.error = '';

    this.projectService.create(this.newProject).subscribe({

      next: (project) => {

        this.projects.push(project);

        this.newProject = {
          name: '',
          description: '',
          ownerId: '3feb00eb-4c36-4a03-80bd-613771a15eb3'
        };

        this.creating = false;
      },

      error: (error) => {

        console.error(error);

        this.error = 'Failed to create project.';

        this.creating = false;
      }

    });
  }

  startEdit(project: Project): void {

    this.editingProjectId = project.id;

    this.editingProject = {
      name: project.name,
      description: project.description ?? ''
    };

    this.error = '';
  }

  cancelEdit(): void {

    this.editingProjectId = '';

    this.editingProject = {
      name: '',
      description: ''
    };
  }

  updateProject(): void {

    if (!this.editingProjectId) {
      return;
    }

    if (!this.editingProject.name.trim()) {
      return;
    }

    this.editing = true;
    this.error = '';

    this.projectService
      .update(this.editingProjectId, this.editingProject)
      .subscribe({

        next: (updatedProject) => {

          const index = this.projects.findIndex(
            p => p.id === updatedProject.id
          );

          if (index !== -1) {
            this.projects[index] = updatedProject;
          }

          this.cancelEdit();

          this.editing = false;
        },

        error: (error) => {

          console.error(error);

          this.error = 'Failed to update project.';

          this.editing = false;
        }

      });
  }

  deleteProject(project: Project): void {

    const confirmed = confirm(
      `Are you sure you want to delete "${project.name}"?`
    );

    if (!confirmed) {
      return;
    }

    this.error = '';

    this.projectService.delete(project.id).subscribe({

      next: () => {

        this.projects = this.projects.filter(
          p => p.id !== project.id
        );

      },

      error: (error) => {

        console.error(error);

        this.error = 'Failed to delete project.';

      }

    });
  }

  viewProject(id: string): void {
    this.router.navigate(['/projects', id]);
  }

}
