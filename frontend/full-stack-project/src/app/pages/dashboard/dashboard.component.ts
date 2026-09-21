import { Component, OnInit, inject } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';
import {
  DashboardStats,
  ProjectStats
} from '../../models/dashboard-stats';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private readonly dashboardService =
    inject(DashboardService);

  stats: DashboardStats | null = null;

  loading = true;

  error = '';

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading = true;
    this.error = '';

    this.dashboardService.getStats().subscribe({
      next: (stats) => {
        this.stats = stats;
        this.loading = false;
      },
      error: () => {
        this.error =
          'Failed to load dashboard statistics.';
        this.loading = false;
      }
    });
  }
}