import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { CopyProgramActivityYearRequest, DeactivateProgramActivityRequest, SaveProgramActivityRequest } from '../request-models/program-activity-request';
import { ProgramActivityTreeResponse } from '../response-models/program-activity-response';
import { ResponseBase } from '../response-models/utils-response';

@Injectable({
  providedIn: 'root'
})
export class ProgramActivityService {

  constructor(private http: HttpClient) { }

  public getTree(orcamentoConfigFk: number): Observable<ProgramActivityTreeResponse> {
    return this.http.get<ProgramActivityTreeResponse>(`${environment.apiUrl}/programactivity/GetTree/${orcamentoConfigFk}`);
  }

  public save(request: SaveProgramActivityRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/programactivity/Save`, request);
  }

  public deactivate(request: DeactivateProgramActivityRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/programactivity/Deactivate`, request);
  }

  public copyYear(request: CopyProgramActivityYearRequest): Observable<ResponseBase> {
    return this.http.post<ResponseBase>(`${environment.apiUrl}/programactivity/CopyYear`, request);
  }
}
