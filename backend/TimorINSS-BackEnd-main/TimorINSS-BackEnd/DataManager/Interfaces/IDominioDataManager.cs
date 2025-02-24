using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IDominioDataManager
    {
        public IEnumerable<Dominio> GetAll();

        public Dominio Get(long id);

        public DominioDto GetDto(long id);

        public void Add(Dominio entity);

        public void Update(Dominio entity);

        public void Delete(Dominio entity);

        public DominioDescricaoStringResponse getAllTiposDeContracto();

        public DominioDescricaoStringResponse getAllNaturezasDeContracto();

        public DominioDescricaoStringResponse getAllLeisLaboraisAplicaveis();

        public DominioDescricaoStringResponse GetAllTiposDeDocumento();

        public DominioDescricaoStringResponse GetAllSexos();

        public DominioDescricaoStringResponse getAllEstadosCivis();

        public DominioDescricaoStringResponse GetAllNacionalidades();

        public DominioDescricaoStringResponse GetAllTipoDivida();

        public DominioDescricaoStringResponse GetAllSituacaoPagamento();

        public ListagemRegimesResponse GetAllRegimes();

        public SingleDominioDescricaoStringResponse getSalarioMinimo();

        public DominioDescricaoStringResponse getTipoPago();

        public DominioDescricaoStringResponse getTipoGuia();

        public SingleDominioDescricaoStringResponse GetDeclarationDay();

        public DominioDescricaoStringResponse GetAllProfissoes();

        public DominioDescricaoStringResponse GetAllFuncoes();

        public DominioDescricaoStringResponse GetAllGruposCamposEditaveis();

        public DominioDescricaoStringResponse GetAllTiposDeRegime();

        public DominioDescricaoStringResponse GetAllTiposDeDocumentoTarefa();

        public DominiosComGruposResponse GetTiposDocumentoPorTarefaAtiva(GetTiposDocumentoPorTarefaAtivaRequest request);

        public DominioDescricaoStringResponse getAllCaixas();

        public DominioDescricaoStringResponse getAllMovimentosTypes();

        public DominioDescricaoStringResponse GetAllTiposConta();

        public DominioDescricaoStringResponse GetAllEstadosPagamento();
    }
}