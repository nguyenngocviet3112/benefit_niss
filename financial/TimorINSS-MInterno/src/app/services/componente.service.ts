import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ComponenteListagemResponse } from '../response-models/componente-response';
import { ComponenteAccaoTarefaRequest, ComponenteClassificacaoSubClassificTarefaRequest, ComponenteConciliacaoMovimentosRequest, ComponenteControloAcessoPerfilTarefaRequest, ComponenteControloAcessoUtilizadorTarefaRequest, ComponenteDespesaRequest, ComponenteDocumentoTarefaRequest, ComponenteOrcamentoRequest, ComponentePrazoTarefaRequest, ComponenteReceitaRequest, ComponenteTextoRequest } from '../request-models/componente-request';
import { ApiHelperService } from './api-helper.service';

@Injectable({
  providedIn: 'root'
})
export class ComponenteService {

  constructor(
    private router: Router,
    private http: HttpClient,
    private api: ApiHelperService
  ) { }


  public getAllComponentes(): Observable<ComponenteListagemResponse>
  {
    return this.api.get<ComponenteListagemResponse>('componente/GetAllComponentes');
  }

  public editarComponenteTexto(entity: ComponenteTextoRequest) {
    return this.api.post('componente/EditarComponenteTexto', entity);
  }

  public editarComponentePrazoTarefa(entity: ComponentePrazoTarefaRequest) {
    return this.api.post('componente/EditarComponentePrazoTarefa', entity);
  }

  public editarComponenteAccaoTarefa(entity: ComponenteAccaoTarefaRequest) {
    return this.api.post('componente/EditarComponenteAccaoTarefa', entity);
  }

  public editarComponenteDocumentoTarefa(entity: ComponenteDocumentoTarefaRequest) {
    return this.api.post('componente/EditarComponenteDocumentoTarefa', entity);
  }

  public editarComponenteClassificacaoSubClassifTarefa(entity: ComponenteClassificacaoSubClassificTarefaRequest) {
    return this.api.post('componente/EditarComponenteClassificacaoSubClassifTarefa', entity);
  }

  public editarComponenteControloAcessoPerfilTarefa(entity: ComponenteControloAcessoPerfilTarefaRequest) {
    return this.api.post('componente/EditarComponenteControloAcessoPerfilTarefa', entity);
  }

  public editarComponenteControloAcessoUtilizadorTarefa(entity: ComponenteControloAcessoUtilizadorTarefaRequest) {
    return this.api.post('componente/EditarComponenteControloAcessoUtilizadorTarefa', entity);
  }

  public editarComponenteOrcamento(entity: ComponenteOrcamentoRequest) {
    return this.api.post('componente/EditarComponenteOrcamento', entity);
  }

  public editarComponenteDespesa(entity: ComponenteDespesaRequest) {
    return this.api.post('componente/EditarComponenteDespesa', entity);
  }

  public editarComponenteConciliacaoMovimentos(entity: ComponenteConciliacaoMovimentosRequest) {
    return this.api.post('componente/EditarComponenteConciliacaoMovimentos', entity);
  }

  public editarComponenteReceita(entity: ComponenteReceitaRequest) {
    return this.api.post('componente/EditarComponenteReceita', entity);
  }
}
