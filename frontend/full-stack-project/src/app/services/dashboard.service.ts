import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashboardStats } from '../models/dashboard-stats';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'http://localhost:5090/api/dashboard';

  getStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(
      `${this.apiUrl}/stats`
    );
  }
}