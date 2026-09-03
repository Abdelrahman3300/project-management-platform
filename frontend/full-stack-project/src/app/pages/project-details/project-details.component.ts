import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { Project } from '../../models/project';
import { ProjectService } from '../../services/project.service';

@Component({
  selector: 'app-project-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './project-details.component.html',
  styleUrl: './project-details.component.css'
})
export class ProjectDetailsComponent implements OnInit {

  private readonly projectService = inject(ProjectService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  project: Project | null = null;

  loading = true;
  error = '';

  ngOnInit(): void {

    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.error = 'Project ID was not provided.';
      this.loading = false;
      return;
    }

    this.loadProject(id);
  }

  private loadProject(id: string): void {

    this.projectService.getById(id).subscribe({

      next: (project) => {
        this.project = project;
        this.loading = false;
      },

      error: (error) => {
        console.error(error);
        this.error = 'Failed to load project.';
        this.loading = false;
      }

    });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }
}