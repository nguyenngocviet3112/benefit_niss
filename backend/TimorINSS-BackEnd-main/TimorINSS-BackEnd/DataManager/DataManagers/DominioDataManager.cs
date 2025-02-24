using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class DominioDataManager : IDominioDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public DominioDataManager(IUnitOfWork unitOfWork,
                                 IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public IEnumerable<Dominio> GetAll()
        {
            return _unitOfWork.DominioRepository.GetAll();
        }

        public Dominio Get(long id)
        {
            return _unitOfWork.DominioRepository.Get(id);
        }

        public DominioDto GetDto(long id)
        {
            return _unitOfWork.DominioRepository.GetDto(id);
        }

        public void Add(Dominio entity)
        {
            _unitOfWork.DominioRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Dominio entity)
        {
            _unitOfWork.DominioRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Dominio entity)
        {
            _unitOfWork.DominioRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        private SingleDominioDescricaoStringResponse getSingleTipoDeDominio(TiposDominio tipo)
        {
            SingleDominioDescricaoStringResponse response = new SingleDominioDescricaoStringResponse();

            DominioDescricaoString dominio = _unitOfWork.DominioRepository.getTipoDeDominio(tipo);
            response.dominio = dominio;

            if (dominio == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DiaDeclaracaoNotFound).ToString(),
                    ErrorMessage = ErrorsDataContract.DiaDeclaracaoNotFound.ToString()
                });
            }

            return response;
        }

        private DominioDescricaoStringResponse getAllTiposDeDominio(TiposDominio tipo)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(tipo);
            response.dominios = dominios;

            return response;
        }

        private DominioDescricaoStringResponse getAllMovimentosTypesFiltered()
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPOCONTA).Where(x => x.descricao != "Neutro Receita" && x.descricao != "Neutro Despesa" && x.indActivo == true).ToList();
            response.dominios = dominios;

            return response;
        }

        private DominioDescricaoStringResponse getAlCaixasFiltered()
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.CAIXAS).Where(x => x.indActivo == true).ToList();
            response.dominios = dominios;

            return response;
        }

        public DominioDescricaoStringResponse getAllTiposDeContracto()
        {
            return getAllTiposDeDominio(TiposDominio.TIPOCONTRACTO);
        }

        public DominioDescricaoStringResponse getAllNaturezasDeContracto()
        {
            return getAllTiposDeDominio(TiposDominio.NATUREZACONTRACTO);
        }

        public DominioDescricaoStringResponse getAllLeisLaboraisAplicaveis()
        {
            return getAllTiposDeDominio(TiposDominio.LEILABORALAPLICAVEL);
        }

        public DominioDescricaoStringResponse GetAllTiposDeDocumento()
        {
            return getAllTiposDeDominio(TiposDominio.TIPODOCUMENTO);
        }

        public DominioDescricaoStringResponse GetAllSexos()
        {
            return getAllTiposDeDominio(TiposDominio.SEXO);
        }

        public DominioDescricaoStringResponse getAllEstadosCivis()
        {
            return getAllTiposDeDominio(TiposDominio.ESTADOCIVIL);
        }

        public DominioDescricaoStringResponse GetAllNacionalidades()
        {
            return getAllTiposDeDominio(TiposDominio.NACIONALIDADE);
        }

        public DominioDescricaoStringResponse GetAllTipoDivida()
        {
            return getAllTiposDeDominio(TiposDominio.TIPODIVIDA);
        }

        public DominioDescricaoStringResponse GetAllSituacaoPagamento()
        {
            return getAllTiposDeDominio(TiposDominio.SITUACAOPAGAMENTO);
        }

        public DominioDescricaoStringResponse GetAllGruposCamposEditaveis()
        {
            return getAllTiposDeDominio(TiposDominio.GRUPOCAMPOSEDITAVEIS);
        }

        public ListagemRegimesResponse GetAllRegimes()
        {
            ListagemRegimesResponse response = new ListagemRegimesResponse();
            response.regimes = new List<RegimeDescricaoString>();
            DominioDescricaoStringResponse regimesDominio = getAllTiposDeDominio(TiposDominio.REGIME);
            Dominio tipoRegime;
            string tipoRegimeDesc = "";
            foreach (DominioDescricaoString dominio in regimesDominio.dominios)
            {
                tipoRegime = _unitOfWork.RegimeRepository.GetTipoRegimeFromRegimePai(dominio.id);

                if (tipoRegime != null)
                    tipoRegimeDesc = tipoRegime.Descricao;

                response.regimes.Add(new RegimeDescricaoString
                {
                    descricao = dominio.descricao,
                    id = dominio.id,
                    indActivo = dominio.indActivo,
                    tipoRegime = tipoRegimeDesc,
                    value = dominio.value
                });
            }
            return response;
        }

        public SingleDominioDescricaoStringResponse getSalarioMinimo()
        {
            return getSingleTipoDeDominio(TiposDominio.SALARIOMINIMO);
        }

        public DominioDescricaoStringResponse getTipoPago()
        {
            return getAllTiposDeDominio(TiposDominio.INDPAGO);
        }

        public DominioDescricaoStringResponse getTipoGuia()
        {
            return getAllTiposDeDominio(TiposDominio.TIPOGUIA);
        }

        public SingleDominioDescricaoStringResponse GetDeclarationDay()
        {
            return getSingleTipoDeDominio(TiposDominio.DIADECLRACAO);
        }

        public DominioDescricaoStringResponse GetAllProfissoes()
        {
            return getAllTiposDeDominio(TiposDominio.PROFISSAO);
        }

        public DominioDescricaoStringResponse GetAllFuncoes()
        {
            return getAllTiposDeDominio(TiposDominio.FUNCAO);
        }

        public DominioDescricaoStringResponse GetAllTiposDeRegime()
        {
            return getAllTiposDeDominio(TiposDominio.TIPOREGIME);
        }

        public DominioDescricaoStringResponse GetAllTiposDeDocumentoTarefa()
        {
            return getAllTiposDeDominio(TiposDominio.TIPODOCUMENTOTAREFA);
        }

        public DominiosComGruposResponse GetTiposDocumentoPorTarefaAtiva(GetTiposDocumentoPorTarefaAtivaRequest request)
        {
            var response = new DominiosComGruposResponse();

            var tarefaAtivo = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.TarefaAtivoId);

            if (tarefaAtivo == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString()
                });
                return response;
            }

            var tarefaConfig = tarefaAtivo.TarefaconfigFkNavigation;
            var listDocumentosPorTarefa = _unitOfWork.ComponenteCarregarDocumentoRepository.GetComponenteByIdTarefa(tarefaConfig.Id);

            if (listDocumentosPorTarefa.Count == 0)
            {
                return response;
            }

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPODOCUMENTOTAREFA);
            response.dominio = new DominiosComGrupos
            {
                groups = new List<Group>()
            };

            //Fill Obrigatorio Documents
            var listaObrigatorios = listDocumentosPorTarefa.Where(x => x.Obrigatorio).ToList();

            if (listaObrigatorios.Count > 0)
            {
                var obrigatorioDominios = dominios.Where(x => listaObrigatorios.Select(u => u.DocumentoFk).ToList().Contains((int)x.id)).OrderBy(x => x.descricao).ToList();
                response.dominio.groups.Add(new Group
                {
                    name = "obrigatorio",
                    values = new List<DominioDescricaoString>(obrigatorioDominios)
                });
            }

            //Fill Não Obrigatorio Documents
            var listaNaoObrigatorios = listDocumentosPorTarefa.Where(x => !x.Obrigatorio).ToList();
            if (listaNaoObrigatorios.Count > 0)
            {
                var naoObrigatorioDominios = dominios.Where(x => listaNaoObrigatorios.Select(u => u.DocumentoFk).ToList().Contains((int)x.id)).OrderBy(x => x.descricao).ToList();
                response.dominio.groups.Add(new Group
                {
                    name = "naoObrigatorio",
                    values = new List<DominioDescricaoString>(naoObrigatorioDominios)
                });
            }

            return response;
        }

        public DominioDescricaoStringResponse getAllCaixas()
        {
            return getAlCaixasFiltered();
        }

        public DominioDescricaoStringResponse getAllMovimentosTypes()
        {
            return getAllMovimentosTypesFiltered();
        }

        public DominioDescricaoStringResponse GetAllTiposConta()
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPOCONTA).Where(x => x.indActivo == true).ToList();
            response.dominios = dominios;

            return response;
        }

        public DominioDescricaoStringResponse GetAllEstadosPagamento()
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.ESTADOPAGAMENTO).Where(x => x.indActivo == true).ToList();
            response.dominios = dominios;

            return response;
        }
    }
}