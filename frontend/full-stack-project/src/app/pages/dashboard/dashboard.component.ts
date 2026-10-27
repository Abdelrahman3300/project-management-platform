
import { Component, OnInit, inject } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';
import {
  DashboardStats
} from '../../models/dashboard-stats';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private readonly dashboardService =
    inject(DashboardService);

  private readonly router = inject(Router);

  stats: DashboardStats | null = null;
  loading = false;
  error = '';

  ngOnInit(): void {
    this.loadDashboard();
  }


  loadDashboard(): void {
    // Prevent duplicate requests while loading.
    if (this.loading) {
      return;
    }

    this.loading = true;
    this.error = '';
    this.stats = null;

    this.dashboardService.getStats().subscribe({
      next: (stats) => {
        this.stats = stats;
        this.loading = false;
      },
      error: () => {
        this.error =
          'Failed to load dashboard statistics. Please try again.';
        this.loading = false;
      }
    });
  }


  openProject(projectId: string): void {
    this.router.navigate(['/projects', projectId]);
  }
}
