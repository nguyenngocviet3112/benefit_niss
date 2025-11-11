import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { SelectDescriptionResponse } from '../response-models/utils-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class PaisService {

  constructor(
      private http: HttpClient,
      private api: ApiHelperService
  ) {}

  public getAllPais(): Observable<SelectDescriptionResponse>
  {
    return this.api.get<SelectDescriptionResponse>('Pais/getPais');
  }
}
