import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetDestinatarioRequest, SaveDestinatarioRequest } from '../request-models/destinatario-request';
import { GetDestinatarioResponse } from '../response-models/destinatario-response';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class DestinatarioService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public GetDestinatarioByNissTin(request: GetDestinatarioRequest): Observable<GetDestinatarioResponse>
  {
    return this.api.post<GetDestinatarioResponse>('destinatario/GetDestinatarioByNissTin', request);
  }

  public SaveDestinatario(entity: SaveDestinatarioRequest){
    return this.api.post('destinatario/SaveDestinatario', entity);
  }
}
