import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'projects/:id',
    loadComponent: () =>
      import('./pages/project-details/project-details.component')
        .then(m => m.ProjectDetailsComponent)
  },
  {
    path: '',
    redirectTo: 'projects',
    pathMatch: 'full'
  }
];