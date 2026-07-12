import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DashboardSummaryResponse } from '../response-models/dashboard-response';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(private http: HttpClient) { }

  public getSummary(): Observable<DashboardSummaryResponse> {
    return this.http.get<DashboardSummaryResponse>(`${environment.apiUrl}/dashboard/GetSummary`);
  }
}
