import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { UtilizadorTrbalhador } from '../models/utilizador';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UtilizadoresServices {

  constructor(
      private router: Router,
      private http: HttpClient
  ) {}

  public getById(id: number): Observable<UtilizadorTrbalhador>
  {
    return this.http.get<UtilizadorTrbalhador>(`${environment.apiUrl}/api/Utilizadores/` + id).pipe(map(user =>
      {
        return {
          id: user.id,
          idEntidadeEmpregadora: user.idEntidadeEmpregadora
        };
      }));
  }
}
