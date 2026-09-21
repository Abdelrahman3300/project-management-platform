import { Routes } from '@angular/router';

import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login.component')
        .then(m => m.LoginComponent)
  },

  {
    path: 'register',
    loadComponent: () =>
      import('./pages/register/register.component')
        .then(m => m.RegisterComponent)
  },

  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/dashboard/dashboard.component')
        .then(m => m.DashboardComponent)
  },

  {
    path: 'projects/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/project-details/project-details.component')
        .then(m => m.ProjectDetailsComponent)
  },

  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login'
  },

  {
    path: '**',
    redirectTo: 'login'
  }
];