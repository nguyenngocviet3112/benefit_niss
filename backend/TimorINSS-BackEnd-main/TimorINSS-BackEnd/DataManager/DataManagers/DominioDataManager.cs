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

        private SingleDominioDescricaoStringResponse getSingleTipoDeDominio(TiposDominio tipo, string language)
        {
            SingleDominioDescricaoStringResponse response = new SingleDominioDescricaoStringResponse();

            DominioDescricaoString dominio = _unitOfWork.DominioRepository.getTipoDeDominio(tipo);
            if (language != null && language.ToUpper().Contains("EN"))
            {
                dominio.descricao = dominio.descricaoEn;
            }
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

        private DominioDescricaoStringResponse getAllTiposDeDominio(TiposDominio tipo, string language)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(tipo);
            List<DominioDescricaoString> dominiosOut = new List<DominioDescricaoString>();
            if (language != null && language.ToUpper().Contains("EN"))
            {
                foreach (DominioDescricaoString dominio in dominios)
                {
                    dominio.descricao = dominio.descricaoEn;
                    dominiosOut.Add(dominio);
                }
                response.dominios = dominiosOut;
            }
            

            return response;
        }

        private DominioDescricaoStringResponse getAllMovimentosTypesFiltered(string language)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPOCONTA).Where(x => x.descricao != "Neutro Receita" && x.descricao != "Neutro Despesa" && x.indActivo == true).ToList();
            List<DominioDescricaoString> dominiosOut = new List<DominioDescricaoString>();
            if (language != null && language.ToUpper().Contains("EN"))
            {
                foreach (DominioDescricaoString dominio in dominios)
                {
                    dominio.descricao = dominio.descricaoEn;
                    dominiosOut.Add(dominio);
                }
                response.dominios = dominiosOut;
            } else 
            { 
                response.dominios = dominios;
            }
                return response;
        }

        private DominioDescricaoStringResponse getAlCaixasFiltered(string language)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.CAIXAS).Where(x => x.indActivo == true).ToList();
            List<DominioDescricaoString> dominiosOut = new List<DominioDescricaoString>();
            if (language != null && language.ToUpper().Contains("EN"))
            {
                foreach (DominioDescricaoString dominio in dominios)
                {
                    dominio.descricao = dominio.descricaoEn;
                    dominiosOut.Add(dominio);
                }
                response.dominios = dominiosOut;
            }
            else
            {
                response.dominios = dominios;
            }


            return response;
        }

        public DominioDescricaoStringResponse getAllTiposDeContracto(string language)
        {
            return getAllTiposDeDominio(TiposDominio.TIPOCONTRACTO, language);
        }

        public DominioDescricaoStringResponse getAllNaturezasDeContracto(string language)
        {
            return getAllTiposDeDominio(TiposDominio.NATUREZACONTRACTO, language);
        }

        public DominioDescricaoStringResponse getAllLeisLaboraisAplicaveis(string language)
        {
            return getAllTiposDeDominio(TiposDominio.LEILABORALAPLICAVEL, language);
        }

        public DominioDescricaoStringResponse GetAllTiposDeDocumento(string language)
        {
            return getAllTiposDeDominio(TiposDominio.TIPODOCUMENTO, language);
        }

        public DominioDescricaoStringResponse GetAllSexos(string language)
        {
            return getAllTiposDeDominio(TiposDominio.SEXO, language);
        }

        public DominioDescricaoStringResponse getAllEstadosCivis(string language)
        {
            return getAllTiposDeDominio(TiposDominio.ESTADOCIVIL, language);
        }

        public DominioDescricaoStringResponse GetAllNacionalidades(string language)
        {
            return getAllTiposDeDominio(TiposDominio.NACIONALIDADE, language);
        }

        public DominioDescricaoStringResponse GetAllTipoDivida(string language)
        {
            return getAllTiposDeDominio(TiposDominio.TIPODIVIDA, language);
        }

        public DominioDescricaoStringResponse GetAllSituacaoPagamento(string language)
        {
            return getAllTiposDeDominio(TiposDominio.SITUACAOPAGAMENTO, language);
        }

        public DominioDescricaoStringResponse GetAllGruposCamposEditaveis(string language)
        {
            return getAllTiposDeDominio(TiposDominio.GRUPOCAMPOSEDITAVEIS, language);
        }

        public ListagemRegimesResponse GetAllRegimes(string language)
        {
            ListagemRegimesResponse response = new ListagemRegimesResponse();
            response.regimes = new List<RegimeDescricaoString>();
            DominioDescricaoStringResponse regimesDominio = getAllTiposDeDominio(TiposDominio.REGIME, language);
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

        public SingleDominioDescricaoStringResponse getSalarioMinimo(string language)
        {
            return getSingleTipoDeDominio(TiposDominio.SALARIOMINIMO,language);
        }

        public DominioDescricaoStringResponse getTipoPago(string language)
        {
            return getAllTiposDeDominio(TiposDominio.INDPAGO, language);
        }

        public DominioDescricaoStringResponse getTipoGuia(string language)
        {
            return getAllTiposDeDominio(TiposDominio.TIPOGUIA, language);
        }

        public SingleDominioDescricaoStringResponse GetDeclarationDay(string language)
        {
            return getSingleTipoDeDominio(TiposDominio.DIADECLRACAO, language);
        }

        public DominioDescricaoStringResponse GetAllProfissoes(string language)
        {
            return getAllTiposDeDominio(TiposDominio.PROFISSAO, language);
        }

        public DominioDescricaoStringResponse GetAllFuncoes(string language)
        {
            return getAllTiposDeDominio(TiposDominio.FUNCAO, language);
        }

        public DominioDescricaoStringResponse GetAllTiposDeRegime(string language)
        {
            return getAllTiposDeDominio(TiposDominio.TIPOREGIME, language);
        }

        public DominioDescricaoStringResponse GetAllTiposDeDocumentoTarefa(string language)
        {
            return getAllTiposDeDominio(TiposDominio.TIPODOCUMENTOTAREFA, language);
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

        public DominioDescricaoStringResponse getAllCaixas(string language)
        {
            return getAlCaixasFiltered(language);
        }

        public DominioDescricaoStringResponse getAllMovimentosTypes(string language)
        {
            return getAllMovimentosTypesFiltered(language);
        }

        public DominioDescricaoStringResponse GetAllTiposConta(string language)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPOCONTA).Where(x => x.indActivo == true).ToList();
            List<DominioDescricaoString> dominiosOut = new List<DominioDescricaoString>();
            if (language != null && language.ToUpper().Contains("EN"))
            {
                foreach (DominioDescricaoString dominio in dominios)
                {
                    dominio.descricao = dominio.descricaoEn;
                    dominiosOut.Add(dominio);
                }
                response.dominios = dominiosOut;
            }
            else
            {
                response.dominios = dominios;
            }

            return response;
        }

        public DominioDescricaoStringResponse GetAllEstadosPagamento(string language)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.ESTADOPAGAMENTO).Where(x => x.indActivo == true).ToList();
            List<DominioDescricaoString> dominiosOut = new List<DominioDescricaoString>();
            if (language != null && language.ToUpper().Contains("EN"))
            {
                foreach (DominioDescricaoString dominio in dominios)
                {
                    dominio.descricao = dominio.descricaoEn;
                    dominiosOut.Add(dominio);
                }
                response.dominios = dominiosOut;
            }
            else
            {
                response.dominios = dominios;
            }

            return response;
        }
    }
}