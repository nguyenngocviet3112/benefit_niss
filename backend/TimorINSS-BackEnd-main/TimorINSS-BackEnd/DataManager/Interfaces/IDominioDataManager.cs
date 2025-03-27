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

        public DominioDescricaoStringResponse getAllTiposDeContracto(string language);

        public DominioDescricaoStringResponse getAllNaturezasDeContracto(string language);

        public DominioDescricaoStringResponse getAllLeisLaboraisAplicaveis(string language);

        public DominioDescricaoStringResponse GetAllTiposDeDocumento(string language);

        public DominioDescricaoStringResponse GetAllSexos(string language);

        public DominioDescricaoStringResponse getAllEstadosCivis(string language);

        public DominioDescricaoStringResponse GetAllNacionalidades(string language);

        public DominioDescricaoStringResponse GetAllTipoDivida(string language);

        public DominioDescricaoStringResponse GetAllSituacaoPagamento(string language);

        public ListagemRegimesResponse GetAllRegimes(string language);

        public SingleDominioDescricaoStringResponse getSalarioMinimo(string language);

        public DominioDescricaoStringResponse getTipoPago(string language);

        public DominioDescricaoStringResponse getTipoGuia(string language);

        public SingleDominioDescricaoStringResponse GetDeclarationDay(string language);

        public DominioDescricaoStringResponse GetAllProfissoes(string language);

        public DominioDescricaoStringResponse GetAllFuncoes(string language);

        public DominioDescricaoStringResponse GetAllGruposCamposEditaveis(string language);

        public DominioDescricaoStringResponse GetAllTiposDeRegime(string language);

        public DominioDescricaoStringResponse GetAllTiposDeDocumentoTarefa(string language);

        public DominiosComGruposResponse GetTiposDocumentoPorTarefaAtiva(GetTiposDocumentoPorTarefaAtivaRequest request);

        public DominioDescricaoStringResponse getAllCaixas(string language);

        public DominioDescricaoStringResponse getAllMovimentosTypes(string language);

        public DominioDescricaoStringResponse GetAllTiposConta(string language);

        public DominioDescricaoStringResponse GetAllEstadosPagamento(string language);
    }
}