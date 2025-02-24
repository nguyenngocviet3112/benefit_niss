using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class CamposEditaveisDataManager : ICamposEditaveisDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public CamposEditaveisDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public CamposEditaveisListagemResponse GetAllCamposEditaveis(RequestBaseDataContract request)
        {
            CamposEditaveisListagemResponse response = new CamposEditaveisListagemResponse();
            try
            {
                // Validar se o utilizador tem as permissões necessárias
                bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.CamposEditaveis, _unitOfWork, CRUD.READ);
                if (!permission)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }

                List<CamposEditaveisListagem> campos = _unitOfWork.CamposEditaveisRepository.getAllCamposEditaveis();
                foreach (CamposEditaveisListagem campo in campos)
                {
                    switch (campo.Nome)
                    {
                        case "Natureza jurídica":
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "Codigo",
                                    Size = "2",
                                    Type = "number",
                                    Valor = ""
                                }
                            };
                            campo.NomeSize = 100;
                            break;

                        case "Atividade económica":
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "Codigo",
                                    Size = "3",
                                    Type = "number",
                                    Valor = ""
                                }
                            };
                            campo.NomeSize = 100;
                            break;

                        case "Data limite de declaração":
                            campo.NomeNaoEditavel = true;
                            campo.NaoEliminavel = true;
                            campo.NaoPesquisavel = true;
                            campo.Unico = true;
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "Valor",
                                    Size = "2",
                                    Type = "number",
                                    Valor = ""
                                }
                            };
                            campo.NomeSize = 100;
                            break;

                        case "Juro mensal das contribuições":
                            campo.NomeNaoEditavel = true;
                            campo.NaoPesquisavel = true;
                            campo.NomeSize = 25;
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "Percentagem",
                                    Size = "5",
                                    Type = "decimal",
                                    Valor = "",
                                    Suffix = "%"
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "DataInicio",
                                    Size = "0",
                                    Type = "mesAno",
                                    Valor = ""
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "DataFim",
                                    Size = "0",
                                    Type = "mesAno",
                                    Valor = ""
                                }
                            };
                            break;

                        case "Escalão":
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "Valor",
                                    Size = "19",
                                    Type = "currency",
                                    Valor = ""
                                }
                            };
                            campo.NomeSize = 50;
                            break;

                        case "Dados Regime":
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "DataInicio",
                                    Size = "0",
                                    Type = "date",
                                    Valor = ""
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "DataFim",
                                    Size = "0",
                                    Type = "date",
                                    Valor = "",
                                    Optional = true
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "TaxaEntidade",
                                    Size = "6",
                                    Type = "decimal",
                                    Valor = "",
                                    Suffix = "%"
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "TaxaTrabalhador",
                                    Size = "6",
                                    Type = "decimal",
                                    Valor = "",
                                    Suffix = "%"
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "DiaVencimento",
                                    Size = "2",
                                    Type = "number",
                                    Valor = ""
                                }
                            };
                            campo.NomeSize = 50;
                            break;

                        case "Período de vigência da estrutura do orçamento":
                        case "Período de vigência":
                            campo.NomeNaoEditavel = true;
                            campo.NaoPesquisavel = true;
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "DataInicio",
                                    Size = "0",
                                    Type = "date",
                                    Valor = ""
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "DataFim",
                                    Size = "0",
                                    Type = "date",
                                    Valor = "",
                                    Optional = true
                                },
                            };

                            if (campo.Nome == "Período de vigência")
                                campo.NaoEditavel = true;

                            campo.NomeSize = 100;
                            break;

                        case "Centros de custo":
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "DataInicio",
                                    Size = "0",
                                    Type = "date",
                                    Valor = ""
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "DataFim",
                                    Size = "0",
                                    Type = "date",
                                    Valor = "",
                                    Optional = true
                                },
                            };

                            Camposeditaveis campoPaiModel = _unitOfWork.CamposEditaveisRepository.Get(campo.CampoPaiFk.Value);
                            CamposEditaveisDto campoPai = Utils.MappClassToDto<Camposeditaveis, CamposEditaveisDto>(campoPaiModel);
                            if (campoPai.Nome == "Período de vigência")
                                campo.NaoEditavel = true;
                            campo.NomeSize = 50;
                            break;

                        case "Tipo de Conta":
                            campo.NaoEditavel = true;
                            campo.NomeSize = 100;
                            break;

                        case "Agrupamento":
                        case "SubAgrupamento":
                        case "Rúbrica":
                        case "Alínea":
                        case "SubAlínea":
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "CodigoTotal",
                                    Size = "16",
                                    Type = "number",
                                    Valor = "",
                                    NaoVisivel = true
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "Codigo",
                                    Size = "2",
                                    Type = "number",
                                    Valor = ""
                                }
                            };
                            campo.NomeSize = 75;
                            break;

                        case "Tipo de conta (1.º nível)":
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "CodigoTotal",
                                    Size = "16",
                                    Type = "number",
                                    Valor = "",
                                    NaoVisivel = true
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "Codigo",
                                    Size = "2",
                                    Type = "string",
                                    Valor = ""
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "ValorInicial",
                                    Type = "currency",
                                    Optional = true,
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "DataValorInicial",
                                    Type = "date",
                                    Optional = true,
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "CheckBox",
                                    Type = "checkbox",
                                    Credit = false,
                                    Optional = true,
                                }
                            };
                            campo.NomeSize = 100;
                            break;

                        case "Código de conta (2.º nível)":
                        case "Código de conta (3.º nível)":
                        case "Código de conta (4.º nível)":
                        case "Código de conta (5.º nível)":
                        case "Código de conta (6.º nível)":
                        case "Código de conta (7.º nível)":
                        case "Código de conta (8.º nível)":
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "CodigoTotal",
                                    Size = "16",
                                    Type = "number",
                                    Valor = "",
                                    NaoVisivel = true
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "Codigo",
                                    Size = "1",
                                    Type = "number",
                                    Valor = ""
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "ValorInicial",
                                    Type = "currency",
                                    Optional = true,
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "DataValorInicial",
                                    Type = "date",
                                    Optional = true,
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "CheckBox",
                                    Type = "checkbox",
                                    Credit = false,
                                    Optional = true,
                                }
                            };
                            campo.NomeSize = 100;
                            break;

                        case "Contas bancárias":
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "SWIFT",
                                    Size = "8",
                                    Type = "string",
                                    Valor = ""
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "EntidadeBancaria",
                                    Size = "25",
                                    Type = "string",
                                    Valor = ""
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "Descricao",
                                    Size = "25",
                                    Type = "string",
                                    Valor = ""
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "IBAN",
                                    Size = "25",
                                    Type = "string",
                                    Valor = ""
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "NConta",
                                    Size = "21",
                                    Type = "number",
                                    Valor = ""
                                }
                            };
                            campo.NomeSize = 40;
                            campo.NomeNaoEditavel = true;
                            campo.NaoEliminavel = true;
                            break;

                        case "Movimentos bancários":
                            var dropDownSelectionDescription = _unitOfWork.DominioRepository.getAllActiveTiposDeDominio(TiposDominio.TIPOCONTA).Where(x => x.nome != "Neutro Receita" && x.nome != "Neutro Despesa").ToList();
                            campo.Parametros = new List<ParametrosAdicionais>()
                            {
                                new ParametrosAdicionais
                                {
                                    Nome = "TipoMovimento",
                                    Type = "dropdown",
                                    Valor = "",
                                    DropdownList = dropDownSelectionDescription
                                },
                                new ParametrosAdicionais
                                {
                                    Nome = "Descricao",
                                    Size = "100",
                                    Type = "string",
                                    Valor = ""
                                }
                            };
                            campo.NomeSize = 70;
                            campo.NomeNaoEditavel = true;
                            campo.NaoEliminavel = true;
                            break;

                        case "País":
                        case "Município":
                        case "Posto Administrativo":
                        case "Suco":
                        case "Aldeia":
                        case "Classificação":
                        case "Sub-Classificação":
                            campo.NomeSize = 50;
                            break;

                        case "Departamento":
                            campo.NomeSize = 150;
                            break;

                        default:
                            campo.NomeSize = 100;
                            break;
                    }
                }
                response.Campos = campos;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ValueCampoEditavelListagemResponse GetValorCampoEditavel(ValorCamposEditaveisRequest request)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            try
            {
                // Validar se o utilizador tem as permissões necessárias
                bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.CamposEditaveis, _unitOfWork, CRUD.READ);
                if (!permission)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }

                Camposeditaveis campoEditavelModel = _unitOfWork.CamposEditaveisRepository.Get(request.CampoEditavelId);
                CamposEditaveisDto campoEditavel = Utils.MappClassToDto<Camposeditaveis, CamposEditaveisDto>(campoEditavelModel);
                List<SelectDescription> parentsSelectDescription = new List<SelectDescription>();
                //List<MultipleSelectAdditionalParameter> agrupamentos;
                int parent = 0;

                if (string.IsNullOrWhiteSpace(campoEditavel.DominioString))
                {
                    switch (campoEditavel.Nome)
                    {
                        case "Natureza jurídica":
                            response = _unitOfWork.NaturezaJuridicaRepository.GetAllActiveNaturezaJuridica(request.Filter);
                            break;

                        case "Sector de atividade":
                            response = _unitOfWork.SectorActividadeRepository.GetAllActiveSectorActividade(request.Filter);
                            break;

                        case "Atividade económica":
                            response = _unitOfWork.ActividadeEconomicaRepository.GetAllActiveActividadeEconomica(request.Filter);
                            break;

                        case "País":
                            response = _unitOfWork.PaisRepository.getAllActivePais(request.Filter);
                            break;

                        case "Município":
                            response = _unitOfWork.MunicipioRepository.getAllActiveMunicipio(request.Filter);
                            break;

                        case "Posto Administrativo":
                            response = _unitOfWork.PostoAdministrativoRepository.getAllActivePostoAdministrativo(request.Filter);
                            parentsSelectDescription = _unitOfWork.MunicipioRepository.getAllActiveMunicipio();
                            break;

                        case "Suco":
                            response = _unitOfWork.SucoRepository.getAllActiveSuco(request.Filter);
                            parentsSelectDescription = _unitOfWork.PostoAdministrativoRepository.getAllActivePostoAdministrativo();
                            break;

                        case "Aldeia":
                            response = _unitOfWork.AldeiaRepository.getAllActiveAldeia(request.Filter);
                            parentsSelectDescription = _unitOfWork.SucoRepository.getAllActiveSuco();
                            break;

                        case "Departamento":
                            response = _unitOfWork.DepartamentoRepository.getAllActiveDepartamento(request.Filter);
                            break;

                        case "Regime":
                            response = GetAllActiveRegime(request.Filter);
                            break;

                        case "Escalão":
                            response = _unitOfWork.EscalaoRepository.GetAllActiveEscaloes(request.Filter);
                            parentsSelectDescription = _unitOfWork.DominioRepository.getAllActiveTiposDeDominio(TiposDominio.REGIME);
                            break;

                        case "Dados Regime":
                            response = _unitOfWork.RegimeRepository.GetAllActiveRegimes(request.Filter);
                            parentsSelectDescription = _unitOfWork.DominioRepository.getAllActiveTiposDeDominio(TiposDominio.REGIME);
                            break;

                        case "Juro mensal das contribuições":
                            response = _unitOfWork.TaxaJuroMensalRepository.GetAllActiveTaxas(request.Filter);
                            break;

                        case "Período de vigência da estrutura do orçamento":
                        case "Período de vigência":
                            response = _unitOfWork.OrcamentoConfigRepository.getAllActiveOrcamentoConfig(request.Filter);
                            break;

                        case "Classificação":
                            response = _unitOfWork.ClassificacaoRepository.GetAllActiveClassificacao(request.Filter);
                            break;

                        case "Sub-Classificação":
                            response = _unitOfWork.SubClassificacaoRepository.GetAllActiveSubClassificacao(request.Filter);
                            parentsSelectDescription = _unitOfWork.ClassificacaoRepository.GetAllActiveClassificacao();
                            break;

                        case "Centros de custo":
                            response = _unitOfWork.CentroCustoRepository.GetAllActiveCentroCusto(request.Filter);
                            parentsSelectDescription = _unitOfWork.OrcamentoConfigRepository.GetAllOrcamentoConfig();
                            break;

                        case "Tipo de Conta":
                            response = _unitOfWork.RelTipoDeContaOrcamentoConfigRepository.GetAllActiveRelTipoDeContaOrcamentoConfig(request.Filter);
                            parentsSelectDescription = _unitOfWork.OrcamentoConfigRepository.GetAllOrcamentoConfig();
                            break;

                        case "Agrupamento":
                            response = _unitOfWork.AgrupamentoConfigRepository.GetAllActiveAgrupamentoConfig(request.Filter);
                            if (response.CountValuesCampo > 0)
                            {
                                if (response.ValuesCampo[0].ParentId.HasValue) //we only really need the parents if the response is populated
                                    parentsSelectDescription = _unitOfWork.RelTipoDeContaOrcamentoConfigRepository.GetAllRelTipoDeContaOrcamentoConfigByParent(response.ValuesCampo[0].ParentId.Value);
                                else
                                    throw new Exception("No id was found for the entity parent");
                            }
                            break;

                        case "SubAgrupamento":
                            response = _unitOfWork.AgrupamentoConfigRepository.GetAllActiveSubAgrupamentoConfig(request.Filter);
                            if (response.CountValuesCampo > 0)
                            {
                                if (response.ValuesCampo[0].ParentId.HasValue) //we only really need the parents if the response is populated
                                    parentsSelectDescription = _unitOfWork.AgrupamentoConfigRepository.GetAllAgrupamentoConfigByParent(response.ValuesCampo[0].ParentId.Value);
                                else
                                    throw new Exception("No id was found for the entity parent");
                            }
                            break;

                        case "Rúbrica":
                        case "Alínea":
                        case "SubAlínea":
                            response = _unitOfWork.AgrupamentoConfigRepository.GetAllActiveSubAgrupamentoConfig(request.Filter);
                            if (response.CountValuesCampo > 0)
                            {
                                if (response.ValuesCampo[0].ParentId.HasValue) //we only really need the parents if the response is populated
                                    parentsSelectDescription = _unitOfWork.AgrupamentoConfigRepository.GetAllSubAgrupamentoConfigByParent(response.ValuesCampo[0].ParentId.Value);
                                else
                                    throw new Exception("No id was found for the entity parent");
                            }
                            break;

                        case "Tipo de conta (1.º nível)":
                            if (int.TryParse(request.Filter.filterField, out parent))
                            {
                                //agrupamentos = _unitOfWork.AgrupamentoConfigRepository.GetAllAgrupamentosMultipleSelect(parent);
                                response = _unitOfWork.CodigoContaRepository.GetAllActiveTipoDeContaNivel1(request.Filter);
                                parentsSelectDescription = _unitOfWork.OrcamentoConfigRepository.GetAllOrcamentoConfig();
                                //response.Parametros = new List<ParametrosAdicionais>()
                                //{
                                //    new ParametrosAdicionais
                                //    {
                                //        Nome = "DataValorInicial",
                                //        Type = "date"
                                //    }
                                //};
                            }
                            break;

                        case "Código de conta (2.º nível)":
                            if (int.TryParse(request.Filter.filterField, out parent))
                            {
                                Agrupamentoconfig agrupamento = _unitOfWork.AgrupamentoConfigRepository.Get(parent);
                                //Reltipodecontaorcamentoconfig rel = null;
                                //if (agrupamento != null)
                                //{
                                //    rel = _unitOfWork.RelTipoDeContaOrcamentoConfigRepository.Get(agrupamento.ReltipoDeContaOrcamentoConfigFk);
                                //    agrupamentos = _unitOfWork.AgrupamentoConfigRepository.GetAllAgrupamentosMultipleSelect(rel.OrcamentoConfigFk);
                                //}
                                //else
                                //{
                                //    agrupamentos = new List<MultipleSelectAdditionalParameter>();
                                //}
                                response = _unitOfWork.CodigoContaRepository.GetAllActiveTipoDeContaSubNivel(request.Filter);
                                if (response.CountValuesCampo > 0)
                                {
                                    if (response.ValuesCampo[0].ParentId.HasValue) //we only really need the parents if the response is populated
                                        parentsSelectDescription = _unitOfWork.CodigoContaRepository.GetAllCodigoDeContaFirstLevelByParent(response.ValuesCampo[0].ParentId.Value);
                                    else
                                        throw new Exception("No id was found for the entity parent");
                                }
                                //response.Parametros = new List<ParametrosAdicionais>()
                                //{
                                //    new ParametrosAdicionais
                                //    {
                                //        Nome = "Agrupamento",
                                //        Type = "multipleSelectCheck",
                                //        Valor = "",
                                //        ValuesList = agrupamentos
                                //    }
                                //};
                            }
                            break;

                        case "Código de conta (3.º nível)":
                        case "Código de conta (4.º nível)":
                        case "Código de conta (5.º nível)":
                        case "Código de conta (6.º nível)":
                        case "Código de conta (7.º nível)":
                        case "Código de conta (8.º nível)":
                            if (int.TryParse(request.Filter.filterField, out parent))
                            {
                                Agrupamentoconfig agrupamento = _unitOfWork.AgrupamentoConfigRepository.Get(parent);
                                //Reltipodecontaorcamentoconfig rel = null;
                                //if (agrupamento != null)
                                //{
                                //    rel = _unitOfWork.RelTipoDeContaOrcamentoConfigRepository.Get(agrupamento.ReltipoDeContaOrcamentoConfigFk);
                                //    agrupamentos = _unitOfWork.AgrupamentoConfigRepository.GetAllAgrupamentosMultipleSelect(rel.OrcamentoConfigFk);
                                //}
                                //else
                                //{
                                //    agrupamentos = new List<MultipleSelectAdditionalParameter>();
                                //}
                                response = _unitOfWork.CodigoContaRepository.GetAllActiveTipoDeContaSubNivel(request.Filter);
                                if (response.CountValuesCampo > 0)
                                {
                                    if (response.ValuesCampo[0].ParentId.HasValue) //we only really need the parents if the response is populated
                                        parentsSelectDescription = _unitOfWork.CodigoContaRepository.GetAllCodigoDeContaByParent(response.ValuesCampo[0].ParentId.Value);
                                    else
                                        throw new Exception("No id was found for the entity parent");
                                }
                                //response.Parametros = new List<ParametrosAdicionais>()
                                //{
                                //    new ParametrosAdicionais
                                //    {
                                //        Nome = "Agrupamento",
                                //        Type = "multipleSelectCheck",
                                //        Valor = "",
                                //        ValuesList = agrupamentos
                                //    }
                                //};
                            }
                            break;

                        case "Contas bancárias":
                            response = _unitOfWork.ContaBancariaRepository.GetAllContasBancarias(request.Filter);
                            break;

                        case "Movimentos bancários":
                            var dropDownSelectionDescription = _unitOfWork.DominioRepository.getAllActiveTiposDeDominio(TiposDominio.TIPOCONTA).Where(x => x.nome != "Neutro Receita" && x.nome != "Neutro Despesa").ToList();
                            response = _unitOfWork.MovimentoBancarioRepository.GetAllMovimentosBancarios(request.Filter, dropDownSelectionDescription);
                            break;

                        default:
                            break;
                    }

                    if (campoEditavel.CampoPaiFk.HasValue)
                    {
                        CamposEditaveisDto campoPai = _unitOfWork.CamposEditaveisRepository.GetDto(campoEditavel.CampoPaiFk.Value);
                        response.ValuesCampoParents = new CamposEditaveisParents
                        {
                            Id = campoPai.IdCampoEditavel,
                            Nome = campoPai.Nome,
                            Valores = parentsSelectDescription.Select(s => new ValorCamposEditaveisParents
                            {
                                Id = s.id,
                                Nome = s.nome,
                                IndActivo = s.indActivo
                            }).ToList()
                        };
                    }
                }
                else
                {
                    if (campoEditavel.DominioString == "DIADECLRACAO")
                        response = _unitOfWork.DominioRepository.getAllActiveTiposDeDominio((TiposDominio)Enum.Parse(typeof(TiposDominio), campoEditavel.DominioString), request.Filter, true);
                    else
                        response = _unitOfWork.DominioRepository.getAllActiveTiposDeDominio((TiposDominio)Enum.Parse(typeof(TiposDominio), campoEditavel.DominioString), request.Filter, false);

                    if (campoEditavel.DominioString == "PROFISSAO" || campoEditavel.DominioString == "FUNCAO")
                    {
                        var outro = response.ValuesCampo.Where(c => c.Nome == "Outro").FirstOrDefault();
                        outro.NaoEditavelEliminavel = true;
                    }
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract SaveValorCampoEditavel(ValorCamposEditaveisSaveRequest request)
        {
            var response = new ResponseBaseDataContract();

            try
            {
                bool permission = false;
                // Validar se o utilizador tem as permissões necessárias
                // Caso o ValorCampo.Id seja 0, é permissão de criar, senão é de atualizar
                if (request.ValorCampo.Id > 0)
                    permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.CamposEditaveis, _unitOfWork, CRUD.UPDATE);
                else if (request.ValorCampo.Id == 0)
                    permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.CamposEditaveis, _unitOfWork, CRUD.CREATE);

                if (!permission)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }

                Camposeditaveis campoEditavelModel = _unitOfWork.CamposEditaveisRepository.Get(request.IdCampo);
                CamposEditaveisDto campoEditavel = Utils.MappClassToDto<Camposeditaveis, CamposEditaveisDto>(campoEditavelModel);

                if (string.IsNullOrWhiteSpace(campoEditavel.DominioString))
                {
                    switch (campoEditavel.Nome)
                    {
                        case "Natureza jurídica":
                            Naturezajuridica natureza = BuildNaturezaJuridicaObject(request.ValorCampo);
                            if (_unitOfWork.NaturezaJuridicaRepository.DoesCodeExists(natureza.IdNatJuridica, natureza.Codigo))
                            {
                                response.Errors.Add(new Error
                                {
                                    ErrorCode = ((int)ErrorsDataContract.UniqueCodeExists).ToString(),
                                    ErrorMessage = ErrorsDataContract.UniqueCodeExists.ToString()
                                });
                                return response;
                            }

                            if (natureza.IdNatJuridica > 0)
                                _unitOfWork.NaturezaJuridicaRepository.Update(natureza);
                            else
                                _unitOfWork.NaturezaJuridicaRepository.Add(natureza);
                            break;

                        case "Sector de atividade":
                            Sectoractividade sector = BuildSectorActividadeObject(request.ValorCampo);
                            if (sector.IdSectorActividade > 0)
                                _unitOfWork.SectorActividadeRepository.Update(sector);
                            else
                                _unitOfWork.SectorActividadeRepository.Add(sector);
                            break;

                        case "Atividade económica":
                            Actividadeeconomica actividade = BuildActividadeEconomicaObject(request.ValorCampo);
                            if (_unitOfWork.ActividadeEconomicaRepository.DoesCodeExists(actividade.IdActivEconomica, actividade.Codigo))
                            {
                                response.Errors.Add(new Error
                                {
                                    ErrorCode = ((int)ErrorsDataContract.UniqueCodeExists).ToString(),
                                    ErrorMessage = ErrorsDataContract.UniqueCodeExists.ToString()
                                });
                                return response;
                            }
                            if (actividade.IdActivEconomica > 0)
                                _unitOfWork.ActividadeEconomicaRepository.Update(actividade);
                            else
                                _unitOfWork.ActividadeEconomicaRepository.Add(actividade);
                            break;

                        case "País":
                            Pais pais = BuildPaisObject(request.ValorCampo);
                            if (pais.IdPais > 0)
                                _unitOfWork.PaisRepository.Update(pais);
                            else
                                _unitOfWork.PaisRepository.Add(pais);
                            break;

                        case "Município":
                            Municipio municipio = BuildMunicipioObject(request.ValorCampo);
                            if (municipio.IdMunicipio > 0)
                                _unitOfWork.MunicipioRepository.Update(municipio);
                            else
                                _unitOfWork.MunicipioRepository.Add(municipio);
                            break;

                        case "Posto Administrativo":
                            Postoadministrativo posto = BuildPostoAdministrativoObject(request.ValorCampo);
                            if (posto.IdPostoAdmin > 0)
                                _unitOfWork.PostoAdministrativoRepository.Update(posto);
                            else
                                _unitOfWork.PostoAdministrativoRepository.Add(posto);
                            break;

                        case "Suco":
                            Suco suco = BuildSucoObject(request.ValorCampo);
                            if (suco.IdSuco > 0)
                                _unitOfWork.SucoRepository.Update(suco);
                            else
                                _unitOfWork.SucoRepository.Add(suco);
                            break;

                        case "Aldeia":
                            Aldeia aldeia = BuildAldeiaObject(request.ValorCampo);
                            if (aldeia.IdAldeia > 0)
                                _unitOfWork.AldeiaRepository.Update(aldeia);
                            else
                                _unitOfWork.AldeiaRepository.Add(aldeia);
                            break;

                        case "Departamento":
                            Departamento departamento = BuildDepartamentoObject(request.ValorCampo);
                            if (departamento.Id > 0)
                                _unitOfWork.DepartamentoRepository.Update(departamento);
                            else
                                _unitOfWork.DepartamentoRepository.Add(departamento);
                            break;

                        case "Escalão":
                            Escalao escalao = BuildEscalaoObject(request.ValorCampo);
                            if (escalao.IdEscalao > 0)
                            {
                                response = ValidateUpdateEscalao(escalao);
                                if (response.Errors.Count > 0)
                                    return response;

                                _unitOfWork.EscalaoRepository.Update(escalao);
                            }
                            else
                                _unitOfWork.EscalaoRepository.Add(escalao);
                            break;

                        case "Dados Regime":
                            response = this.SaveDadosRegime(request.ValorCampo);
                            break;

                        case "Juro mensal das contribuições":
                            Taxajuromensal juro = BuildTaxaJuroMensalObject(request.ValorCampo);
                            response = ValidateCreateTaxaJuroMensal(juro);
                            if (response.Errors.Count > 0)
                                return response;
                            if (juro.IdTaxa > 0)
                                _unitOfWork.TaxaJuroMensalRepository.Update(juro);
                            else
                                _unitOfWork.TaxaJuroMensalRepository.Add(juro);
                            break;

                        case "Período de vigência da estrutura do orçamento":
                            Orcamentoconfig orcamentoConfig = BuildOrcamentoConfigObject(request.ValorCampo);
                            response = ValidateCreateOrcamentoConfig(orcamentoConfig);
                            if (response.Errors.Count > 0)
                                return response;
                            if (orcamentoConfig.Id > 0)
                                _unitOfWork.OrcamentoConfigRepository.Update(orcamentoConfig);
                            else
                            {
                                _unitOfWork.OrcamentoConfigRepository.Add(orcamentoConfig);
                                List<long> dominios = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPOCONTA).Select(d => d.id).ToList();
                                foreach (int dominio in dominios)
                                {
                                    _unitOfWork.RelTipoDeContaOrcamentoConfigRepository.Add(BuildReltipodecontaorcamentoconfigObject(orcamentoConfig, dominio));
                                }
                            }
                            break;

                        case "Classificação":
                            Classificacao classificacao = BuildClassificacaoObject(request.ValorCampo);
                            if (classificacao.Id > 0)
                                _unitOfWork.ClassificacaoRepository.Update(classificacao);
                            else
                                _unitOfWork.ClassificacaoRepository.Add(classificacao);
                            break;

                        case "Sub-Classificação":
                            Subclassificacao subClassificacao = BuildSubClassificacaoObject(request.ValorCampo);
                            if (subClassificacao.Id > 0)
                                _unitOfWork.SubClassificacaoRepository.Update(subClassificacao);
                            else
                                _unitOfWork.SubClassificacaoRepository.Add(subClassificacao);
                            break;

                        case "Centros de custo":
                            Centrocusto centroCusto = BuildCentroCustoObject(request.ValorCampo);
                            response = ValidateCreateCentroCusto(centroCusto);
                            if (response.Errors.Count > 0)
                                return response;
                            if (centroCusto.Id > 0)
                                _unitOfWork.CentroCustoRepository.Update(centroCusto);
                            else
                                _unitOfWork.CentroCustoRepository.Add(centroCusto);
                            break;

                        case "Agrupamento":
                            Agrupamentoconfig agrupamentoConfig = BuildAgrupamenttoConfigFirstLevelObject(request.ValorCampo);
                            response = ValidateCreateAgrupamentoConfig(agrupamentoConfig);
                            if (response.Errors.Count > 0)
                                return response;
                            if (agrupamentoConfig.Id > 0)
                                _unitOfWork.AgrupamentoConfigRepository.Update(agrupamentoConfig);
                            else
                                _unitOfWork.AgrupamentoConfigRepository.Add(agrupamentoConfig);
                            break;

                        case "SubAgrupamento":
                        case "Rúbrica":
                        case "Alínea":
                        case "SubAlínea":
                            Agrupamentoconfig subAgrupamentoConfig = BuildAgrupamenttoConfigSubLevelObject(request.ValorCampo);
                            response = ValidateCreateAgrupamentoConfig(subAgrupamentoConfig);
                            if (response.Errors.Count > 0)
                                return response;
                            if (subAgrupamentoConfig.Id > 0)
                                _unitOfWork.AgrupamentoConfigRepository.Update(subAgrupamentoConfig);
                            else
                                _unitOfWork.AgrupamentoConfigRepository.Add(subAgrupamentoConfig);
                            break;

                        case "Tipo de conta (1.º nível)":
                            Codigoconta codigoConta = BuildCodigoContaFirstLevelObject(request.ValorCampo);
                            response = ValidateCreateCodigoConta(codigoConta);
                            if (response.Errors.Count > 0)
                                return response;

                            ParametrosAdicionais parameterClass2 = request.ValorCampo.Parametros.FirstOrDefault(p => p.Nome == "Agrupamento");
                            List<int> agrupamentosRel = new List<int>();
                            if (parameterClass2 != null)
                            {
                                agrupamentosRel = parameterClass2.SelectedValuesList;
                            }

                            if (codigoConta.Id > 0)
                                _unitOfWork.CodigoContaRepository.Update(codigoConta);
                            else
                                _unitOfWork.CodigoContaRepository.Add(codigoConta);

                            response = ManageAgrupamentosRelations(agrupamentosRel, codigoConta);
                            if (response.Errors.Count > 0)
                                return response;

                            break;

                        case "Código de conta (2.º nível)":
                        case "Código de conta (3.º nível)":
                        case "Código de conta (4.º nível)":
                        case "Código de conta (5.º nível)":
                        case "Código de conta (6.º nível)":
                        case "Código de conta (7.º nível)":
                        case "Código de conta (8.º nível)":
                            Codigoconta codigoContaSub = BuildCodigoContaSubLevelObject(request.ValorCampo);
                            response = ValidateCreateCodigoConta(codigoContaSub);
                            if (response.Errors.Count > 0)
                                return response;

                            ParametrosAdicionais parameterClassAgrupamento = request.ValorCampo.Parametros.FirstOrDefault(p => p.Nome == "Agrupamento");
                            List<int> agrupamentosRelSub = new List<int>();
                            if (parameterClassAgrupamento != null)
                            {
                                agrupamentosRelSub = parameterClassAgrupamento.SelectedValuesList;
                            }

                            if (codigoContaSub.Id > 0)
                                _unitOfWork.CodigoContaRepository.Update(codigoContaSub);
                            else
                                _unitOfWork.CodigoContaRepository.Add(codigoContaSub);

                            response = ManageAgrupamentosRelations(agrupamentosRelSub, codigoContaSub);
                            if (response.Errors.Count > 0)
                                return response;

                            break;

                        case "Contas bancárias":
                            Contabancaria contaBancaria = BuildContaBancariaObject(request.ValorCampo);
                            response = ValidateCreateContaBancaria(contaBancaria);
                            if (response.Errors.Count > 0)
                                return response;
                            if (contaBancaria.Id > 0)
                                _unitOfWork.ContaBancariaRepository.Update(contaBancaria);
                            else
                                _unitOfWork.ContaBancariaRepository.Add(contaBancaria);
                            break;

                        case "Movimentos bancários":
                            Movimentobancario movimentoBancario = BuildMovimentoBancarioObject(request.ValorCampo);
                            response = ValidateCreateMovimentoBancario(movimentoBancario);
                            if (response.Errors.Count > 0)
                                return response;
                            if (movimentoBancario.Id > 0)
                                _unitOfWork.MovimentoBancarioRepository.Update(movimentoBancario);
                            else
                                _unitOfWork.MovimentoBancarioRepository.Add(movimentoBancario);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    Dominio dominio = BuildDominioObject(request.ValorCampo, campoEditavel.DominioString);
                    if (dominio.Dominio1 == "DIADECLRACAO" && (dominio.Valor > 28 || dominio.Valor < 1))
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.InvalidDiaDeclaracao).ToString(),
                            ErrorMessage = ErrorsDataContract.InvalidDiaDeclaracao.ToString()
                        });
                        return response;
                    }

                    if (dominio.IdDominio > 0)
                        _unitOfWork.DominioRepository.Update(dominio);
                    else
                        _unitOfWork.DominioRepository.Add(dominio);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract DeleteValorCampoEditavel(ValorCamposEditaveisDeleteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Validar se o utilizador tem as permissões necessárias
                bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.CamposEditaveis, _unitOfWork, CRUD.DELETE);
                if (!permission)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }

                Camposeditaveis campoEditavelModel = _unitOfWork.CamposEditaveisRepository.Get(request.IdCampo);
                CamposEditaveisDto campoEditavel = Utils.MappClassToDto<Camposeditaveis, CamposEditaveisDto>(campoEditavelModel);

                if (string.IsNullOrWhiteSpace(campoEditavel.DominioString))
                {
                    switch (campoEditavel.Nome)
                    {
                        case "Natureza jurídica":
                            Naturezajuridica natureza = _unitOfWork.NaturezaJuridicaRepository.Get(request.IdValorCampo);
                            natureza.IndActivo = false;
                            natureza = _utils.UpdateDetailsToEntity(natureza);
                            _unitOfWork.NaturezaJuridicaRepository.Update(natureza);
                            break;

                        case "Sector de atividade":
                            Sectoractividade sector = _unitOfWork.SectorActividadeRepository.Get(request.IdValorCampo);
                            sector.IndActivo = false;
                            sector = _utils.UpdateDetailsToEntity(sector);
                            _unitOfWork.SectorActividadeRepository.Update(sector);
                            break;

                        case "Atividade económica":
                            Actividadeeconomica actividade = _unitOfWork.ActividadeEconomicaRepository.Get(request.IdValorCampo);
                            actividade.IndActivo = false;
                            actividade = _utils.UpdateDetailsToEntity(actividade);
                            _unitOfWork.ActividadeEconomicaRepository.Update(actividade);
                            break;

                        case "País":
                            Pais pais = _unitOfWork.PaisRepository.Get(request.IdValorCampo);
                            pais.IndActivo = false;
                            pais = _utils.UpdateDetailsToEntity(pais);
                            _unitOfWork.PaisRepository.Update(pais);
                            break;

                        case "Município":
                            Municipio municipio = _unitOfWork.MunicipioRepository.GetWithActiveChilds(request.IdValorCampo);
                            if (municipio.Postoadministrativo.Count > 0)
                            {
                                response.Errors.Add(new Error
                                {
                                    ErrorCode = ((int)ErrorsDataContract.EntityHasDependents).ToString(),
                                    ErrorMessage = ErrorsDataContract.EntityHasDependents.ToString()
                                });
                                return response;
                            }
                            else
                                municipio.IndActivo = false;

                            municipio = _utils.UpdateDetailsToEntity(municipio);
                            _unitOfWork.MunicipioRepository.Update(municipio);
                            break;

                        case "Posto Administrativo":
                            Postoadministrativo posto = _unitOfWork.PostoAdministrativoRepository.GetWithActiveChilds(request.IdValorCampo);
                            if (posto.Suco.Count > 0)
                            {
                                response.Errors.Add(new Error
                                {
                                    ErrorCode = ((int)ErrorsDataContract.EntityHasDependents).ToString(),
                                    ErrorMessage = ErrorsDataContract.EntityHasDependents.ToString()
                                });
                                return response;
                            }
                            else
                                posto.IndActivo = false;

                            posto = _utils.UpdateDetailsToEntity(posto);
                            _unitOfWork.PostoAdministrativoRepository.Update(posto);
                            break;

                        case "Suco":
                            Suco suco = _unitOfWork.SucoRepository.GetWithActiveChilds(request.IdValorCampo);
                            if (suco.Aldeia.Count > 0)
                            {
                                response.Errors.Add(new Error
                                {
                                    ErrorCode = ((int)ErrorsDataContract.EntityHasDependents).ToString(),
                                    ErrorMessage = ErrorsDataContract.EntityHasDependents.ToString()
                                });
                                return response;
                            }
                            else
                                suco.IndActivo = false;

                            suco = _utils.UpdateDetailsToEntity(suco);
                            _unitOfWork.SucoRepository.Update(suco);
                            break;

                        case "Aldeia":
                            Aldeia aldeia = _unitOfWork.AldeiaRepository.Get(request.IdValorCampo);
                            aldeia.IndActivo = false;
                            aldeia = _utils.UpdateDetailsToEntity(aldeia);
                            _unitOfWork.AldeiaRepository.Update(aldeia);
                            break;

                        case "Departamento":
                            Departamento departamento = _unitOfWork.DepartamentoRepository.Get(request.IdValorCampo);
                            departamento.IndActivo = false;
                            departamento = _utils.UpdateDetailsToEntity(departamento);
                            _unitOfWork.DepartamentoRepository.Update(departamento);
                            break;

                        case "Regime":
                            Dominio regime = _unitOfWork.DominioRepository.Get(request.IdValorCampo);
                            List<Regime> regimeChilds = _unitOfWork.RegimeRepository.GetAllActiveRegimesByParent(regime.IdDominio);
                            List<Escalao> escaloes = _unitOfWork.EscalaoRepository.GetAllEscaloesByParent(regime.IdDominio);

                            if (escaloes.Count > 0 || regimeChilds.Count > 0)
                            {
                                response.Errors.Add(new Error
                                {
                                    ErrorCode = ((int)ErrorsDataContract.EntityHasDependents).ToString(),
                                    ErrorMessage = ErrorsDataContract.EntityHasDependents.ToString()
                                });
                                return response;
                            }
                            else
                                regime.IndActivo = false;

                            regime = _utils.UpdateDetailsToEntity(regime);
                            _unitOfWork.DominioRepository.Update(regime);
                            break;

                        case "Escalão":
                            Escalao escalao = _unitOfWork.EscalaoRepository.Get(request.IdValorCampo);
                            escalao.IndActivo = false;
                            escalao = _utils.UpdateDetailsToEntity(escalao);
                            _unitOfWork.EscalaoRepository.Update(escalao);
                            break;

                        case "Dados Regime":
                            if (_unitOfWork.DeclaracaoRemuneracaoRepository.ExistsDeclaracaoWithRegime(request.IdValorCampo))
                            {
                                response.Errors.Add(new Error
                                {
                                    ErrorCode = ((int)ErrorsDataContract.RegimeEmDeclaracoes).ToString(),
                                    ErrorMessage = ErrorsDataContract.RegimeEmDeclaracoes.ToString()
                                });
                                return response;
                            }
                            else
                            {
                                Regime dadosRegime = _unitOfWork.RegimeRepository.Get(request.IdValorCampo);
                                dadosRegime.IndActivo = false;
                                dadosRegime = _utils.UpdateDetailsToEntity(dadosRegime);
                                _unitOfWork.RegimeRepository.Update(dadosRegime);
                            }
                            break;

                        case "Período de vigência da estrutura do orçamento":
                            Orcamentoconfig orcamento = _unitOfWork.OrcamentoConfigRepository.Get(request.IdValorCampo);
                            response = ValidateDeleteOrcamentoConfig(orcamento);
                            if (response.Errors.Count > 0)
                                return response;
                            orcamento.IndActivo = false;
                            orcamento = _utils.UpdateDetailsToEntity(orcamento);
                            _unitOfWork.OrcamentoConfigRepository.Update(orcamento);
                            break;

                        case "Classificação":
                            Classificacao classificacao = _unitOfWork.ClassificacaoRepository.Get(request.IdValorCampo);
                            classificacao.IndActivo = false;
                            classificacao = _utils.UpdateDetailsToEntity(classificacao);
                            _unitOfWork.ClassificacaoRepository.Update(classificacao);
                            break;

                        case "Sub-Classificação":
                            Subclassificacao subClassificacao = _unitOfWork.SubClassificacaoRepository.Get(request.IdValorCampo);
                            subClassificacao.IndActivo = false;
                            subClassificacao = _utils.UpdateDetailsToEntity(subClassificacao);
                            _unitOfWork.SubClassificacaoRepository.Update(subClassificacao);
                            break;

                        case "Centros de custo":
                            Centrocusto centroCusto = _unitOfWork.CentroCustoRepository.Get(request.IdValorCampo);
                            response = ValidateDeleteCentroCusto(centroCusto);
                            if (response.Errors.Count > 0)
                                return response;
                            centroCusto.IndActivo = false;
                            centroCusto = _utils.UpdateDetailsToEntity(centroCusto);
                            _unitOfWork.CentroCustoRepository.Update(centroCusto);
                            break;

                        case "Agrupamento":
                        case "SubAgrupamento":
                        case "Rúbrica":
                        case "Alínea":
                        case "SubAlínea":
                            Agrupamentoconfig subAgrupamentoConfig = _unitOfWork.AgrupamentoConfigRepository.Get(request.IdValorCampo);
                            response = ValidateDeleteAgrupamento(subAgrupamentoConfig);
                            if (response.Errors.Count > 0)
                                return response;
                            subAgrupamentoConfig.IndActivo = false;
                            subAgrupamentoConfig = _utils.UpdateDetailsToEntity(subAgrupamentoConfig);
                            _unitOfWork.AgrupamentoConfigRepository.Update(subAgrupamentoConfig);
                            break;

                        case "Tipo de conta (1.º nível)":
                        case "Código de conta (2.º nível)":
                        case "Código de conta (3.º nível)":
                        case "Código de conta (4.º nível)":
                        case "Código de conta (5.º nível)":
                        case "Código de conta (6.º nível)":
                        case "Código de conta (7.º nível)":
                        case "Código de conta (8.º nível)":
                            Codigoconta codigoConta = _unitOfWork.CodigoContaRepository.Get(request.IdValorCampo);
                            response = ValidateDeleteCodigoConta(codigoConta);
                            if (response.Errors.Count > 0)
                                return response;
                            codigoConta.IndActivo = false;
                            codigoConta = _utils.UpdateDetailsToEntity(codigoConta);

                            var listRels = codigoConta.Relcodigocontaagrupamentoconfig;
                            Relcodigocontaagrupamentoconfig relUpdated;
                            foreach (Relcodigocontaagrupamentoconfig rel in listRels)
                            {
                                relUpdated = rel;
                                relUpdated.IndActivo = false;
                                relUpdated = _utils.UpdateDetailsToEntity(rel);

                                _unitOfWork.RelCodigoContaAgrupamentoConfigRepository.Update(relUpdated);
                            }

                            _unitOfWork.CodigoContaRepository.Update(codigoConta);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    Dominio dominio = _unitOfWork.DominioRepository.Get(request.IdValorCampo);
                    dominio.IndActivo = false;
                    _unitOfWork.DominioRepository.Update(dominio);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract SaveRegimeCampoEditavel(RegimeCampoEditaveisDeleteRequest request)
        {
            var response = new ResponseBaseDataContract();

            try
            {
                bool permission = false;
                // Validar se o utilizador tem as permissões necessárias
                // Caso o ValorCampo.Id seja 0, é permissão de criar, senão é de atualizar
                if (request.ValorCampo.Id > 0)
                    permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.CamposEditaveis, _unitOfWork, CRUD.UPDATE);
                else if (request.ValorCampo.Id == 0)
                    permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.CamposEditaveis, _unitOfWork, CRUD.CREATE);

                if (!permission)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }

                int tipoRegimeID = 0;
                if (request.ValorCampo.Parametros.Count > 0)
                {
                    string tipoRegimeValor = request.ValorCampo.Parametros[0].Valor;
                    var tipoRegimeClass = _unitOfWork.DominioRepository.getDominioByDescricao(TiposDominio.TIPOREGIME, tipoRegimeValor);
                    tipoRegimeID = tipoRegimeClass.IdDominio;
                }

                Dominio regime = BuildDominioObject(request.ValorCampo, "REGIME");
                if (regime.IdDominio > 0)
                {
                    List<Regime> regimeChilds = _unitOfWork.RegimeRepository.GetAllRegimesByParent(regime.IdDominio);

                    bool exists = false;

                    // see if there is a declaration with the regime
                    foreach (Regime regimeChild in regimeChilds)
                    {
                        if (_unitOfWork.DeclaracaoRemuneracaoRepository.ExistsDeclaracaoWithRegime(regimeChild.IdRegime))
                        {
                            exists = true;
                            break;
                        }
                    }

                    // see if trying to update Type of Regime
                    Dominio tipoRegime = _unitOfWork.RegimeRepository.GetTipoRegimeFromRegimePai(regime.IdDominio);
                    if (exists)
                    {
                        if (tipoRegime.IdDominio != tipoRegimeID)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.RegimeEmDeclaracoes).ToString(),
                                ErrorMessage = ErrorsDataContract.RegimeEmDeclaracoes.ToString()
                            });
                            return response;
                        }
                    }

                    //update type of Regime
                    Regime newRegimeChild;
                    foreach (Regime regimeChild in regimeChilds)
                    {
                        regimeChild.TipoRegime = tipoRegimeID;
                        newRegimeChild = _utils.UpdateDetailsToEntity(regimeChild);
                        _unitOfWork.RegimeRepository.Update(newRegimeChild);
                    }

                    regime = _utils.UpdateDetailsToEntity(regime);
                    _unitOfWork.DominioRepository.Update(regime);
                }
                else
                {
                    regime = _utils.SetDetailsToEntity(regime);
                    _unitOfWork.DominioRepository.Add(regime);
                    Regime regimeChild = new Regime
                    {
                        IdRegime = 0,
                        DataInicio = System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                        DataFim = System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                        DataVencimento = 0,
                        IndActivo = false,
                        PercentEntidadeEmpreg = 0,
                        PercentTrabalhador = 0,
                        RegimePaiNavigation = regime,
                        NomeRegime = "blankRegime",
                        TipoRegime = tipoRegimeID,
                    };
                    regimeChild = _utils.SetDetailsToEntity(regimeChild);
                    _unitOfWork.RegimeRepository.Add(regimeChild);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        #region Regimes

        private ValueCampoEditavelListagemResponse GetAllActiveRegime(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            response = _unitOfWork.DominioRepository.getAllActiveTiposDeDominio(TiposDominio.REGIME, filter, false);

            int regimeId;
            foreach (ValorCamposEditaveis campo in response.ValuesCampo)
            {
                regimeId = campo.Id;
                Dominio tipoRegime = _unitOfWork.RegimeRepository.GetTipoRegimeFromRegimePai(regimeId);
                campo.Parametros = new List<ParametrosAdicionais>()
                {
                    new ParametrosAdicionais
                    {
                        Nome = "TipoRegime",
                        Size = "1",
                        Type = "text",
                        Valor = tipoRegime.Descricao
                    }
                };
            }
            return response;
        }

        private ResponseBaseDataContract SaveDadosRegime(ValorCamposEditaveis valorCampo)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                Regime regime = BuildRegimeObject(valorCampo);

                if (regime.DataVencimento > 28)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.MaxDataVencimento).ToString(),
                        ErrorMessage = ErrorsDataContract.MaxDataVencimento.ToString()
                    });
                    return response;
                }

                List<Regime> regimesIrmaos = _unitOfWork.RegimeRepository.GetAllActiveRegimesByParent(valorCampo.ParentId.Value);

                foreach (var regimeIrmao in regimesIrmaos)
                {
                    if (regimeIrmao.IdRegime != regime.IdRegime && regimeIrmao.DataInicio < (regime.DataFim ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value) &&
                        regime.DataInicio < (regimeIrmao.DataFim ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value))
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.RegimeDataRange).ToString(),
                            ErrorMessage = ErrorsDataContract.RegimeDataRange.ToString()
                        });
                        return response;
                    }
                }

                if (regime.IdRegime > 0)
                {
                    if (_unitOfWork.DeclaracaoRemuneracaoRepository.ExistsDeclaracaoWithRegime(regime.IdRegime))
                    {
                        Regime regimeOriginal = _unitOfWork.RegimeRepository.Get(valorCampo.Id);

                        if (regime.DataInicio != regimeOriginal.DataInicio || regime.DataFim != regimeOriginal.DataFim ||
                            regime.PercentEntidadeEmpreg != regimeOriginal.PercentEntidadeEmpreg || regime.PercentTrabalhador != regimeOriginal.PercentTrabalhador ||
                            regime.TipoRegime != regimeOriginal.TipoRegime || regime.RegimePai != regimeOriginal.RegimePai || regime.RegimePai != regimeOriginal.RegimePai)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.RegimeEmDeclaracoes).ToString(),
                                ErrorMessage = ErrorsDataContract.RegimeEmDeclaracoes.ToString()
                            });
                            return response;
                        }
                    }
                    _unitOfWork.RegimeRepository.Update(regime);
                }
                else
                    _unitOfWork.RegimeRepository.Add(regime);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        #endregion Regimes

        #region Builders

        private Dominio BuildDominioObject(ValorCamposEditaveis valorCampo, string dominio)
        {
            ParametrosAdicionais? valueClasse = null;
            if (valorCampo.Parametros != null)
                valueClasse = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Valor");

            int value = 0;

            if (valueClasse == null)
                value = _unitOfWork.DominioRepository.getNextValue(dominio);
            else
            {
                if (!int.TryParse(valueClasse.Valor, out value))
                    value = 0;
            }

            DominioDto entity = new DominioDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.DominioRepository.GetDto(valorCampo.Id);

            entity.IdDominio = valorCampo.Id;
            entity.Dominio1 = dominio;
            entity.Valor = value;
            entity.Descricao = valorCampo.Nome;
            entity.IndActivo = true;

            if (entity.IdDominio > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<DominioDto, Dominio>(entity);
        }

        private Naturezajuridica BuildNaturezaJuridicaObject(ValorCamposEditaveis valorCampo)
        {
            ParametrosAdicionais parameterClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Codigo");

            string codigo = "00";

            if (parameterClass != null)
            {
                codigo = parameterClass.Valor;
            }
            else
            {
                throw new Exception("No code was found for the entity");
            }

            NaturezajuridicaDto entity = new NaturezajuridicaDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.NaturezaJuridicaRepository.GetDto(valorCampo.Id);

            entity.IdNatJuridica = valorCampo.Id;
            entity.Codigo = codigo;
            entity.Descricao = valorCampo.Nome;
            entity.IndActivo = true;

            if (entity.IdNatJuridica > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<NaturezajuridicaDto, Naturezajuridica>(entity);
        }

        private Sectoractividade BuildSectorActividadeObject(ValorCamposEditaveis valorCampo)
        {
            SectoractividadeDto entity = new SectoractividadeDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.SectorActividadeRepository.GetDto(valorCampo.Id);

            entity.IdSectorActividade = valorCampo.Id;
            entity.Descricao = valorCampo.Nome;
            entity.IndActivo = true;

            if (entity.IdSectorActividade > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<SectoractividadeDto, Sectoractividade>(entity);
        }

        private Actividadeeconomica BuildActividadeEconomicaObject(ValorCamposEditaveis valorCampo)
        {
            ParametrosAdicionais parameterClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Codigo");

            string codigo = "000";

            if (parameterClass != null)
            {
                codigo = parameterClass.Valor;
            }
            else
            {
                throw new Exception("No code was found for the entity");
            }

            ActividadeeconomicaDto entity = new ActividadeeconomicaDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.ActividadeEconomicaRepository.GetDto(valorCampo.Id);

            entity.IdActividadeeconomica = valorCampo.Id;
            entity.Descricao = valorCampo.Nome;
            entity.Codigo = codigo;
            entity.IndActivo = true;

            if (entity.IdActividadeeconomica > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<ActividadeeconomicaDto, Actividadeeconomica>(entity);
        }

        private Pais BuildPaisObject(ValorCamposEditaveis valorCampo)
        {
            PaisDto entity = new PaisDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.PaisRepository.GetDto(valorCampo.Id);

            entity.IdPais = valorCampo.Id;
            entity.Nome = valorCampo.Nome;
            entity.IndActivo = true;

            if (entity.IdPais > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<PaisDto, Pais>(entity);
        }

        private Municipio BuildMunicipioObject(ValorCamposEditaveis valorCampo)
        {
            MunicipioDto entity = new MunicipioDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.MunicipioRepository.GetDto(valorCampo.Id);

            entity.IdMunicipio = valorCampo.Id;
            entity.Nome = valorCampo.Nome;
            entity.IndActivo = true;

            if (entity.IdMunicipio > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<MunicipioDto, Municipio>(entity);
        }

        private Postoadministrativo BuildPostoAdministrativoObject(ValorCamposEditaveis valorCampo)
        {
            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            PostoadministrativoDto entity = new PostoadministrativoDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.PostoAdministrativoRepository.GetDto(valorCampo.Id);

            entity.IdPostoAdmin = valorCampo.Id;
            entity.Nome = valorCampo.Nome;
            entity.IndActivo = true;
            entity.PostoAdminMunicipioFk = valorCampo.ParentId.Value;

            if (entity.IdPostoAdmin > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<PostoadministrativoDto, Postoadministrativo>(entity);
        }

        private Suco BuildSucoObject(ValorCamposEditaveis valorCampo)
        {
            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            SucoDto entity = new SucoDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.SucoRepository.GetDto(valorCampo.Id);

            entity.IdSuco = valorCampo.Id;
            entity.Nome = valorCampo.Nome;
            entity.IndActivo = true;
            entity.SucoPostoAdminFk = valorCampo.ParentId.Value;

            if (entity.IdSuco > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<SucoDto, Suco>(entity);
        }

        private Aldeia BuildAldeiaObject(ValorCamposEditaveis valorCampo)
        {
            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            AldeiaDto entity = new AldeiaDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.AldeiaRepository.GetDto(valorCampo.Id);

            entity.IdAldeia = valorCampo.Id;
            entity.Nome = valorCampo.Nome;
            entity.IndActivo = true;
            entity.AldeiaSucoFk = valorCampo.ParentId.Value;

            if (entity.IdAldeia > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<AldeiaDto, Aldeia>(entity);
        }

        private Departamento BuildDepartamentoObject(ValorCamposEditaveis valorCampo)
        {
            DepartamentoDto entity = new DepartamentoDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.DepartamentoRepository.GetDto(valorCampo.Id);

            entity.Id = valorCampo.Id;
            entity.Nome = valorCampo.Nome;
            entity.IndActivo = true;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<DepartamentoDto, Departamento>(entity);
        }

        public Escalao BuildEscalaoObject(ValorCamposEditaveis valorCampo)
        {
            ParametrosAdicionais parameterClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Valor");

            decimal valorEscalao;

            if (parameterClass != null)
            {
                try
                {
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    valorEscalao = Convert.ToDecimal(parameterClass.Valor, CultureInfo.CreateSpecificCulture("en-Us"));
                }
                catch (Exception e)
                {
                    throw new Exception("The value could not be converted" + e.Message);
                }
            }
            else
                throw new Exception("No Value was found for the entity");

            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            EscalaoDto entity = new EscalaoDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.EscalaoRepository.GetDto(valorCampo.Id);

            entity.IdEscalao = valorCampo.Id;
            entity.DescNivelEscalao = valorCampo.Nome;
            entity.Valor = valorEscalao;
            entity.IndActivo = true;
            entity.EscalaoRegimeFk = valorCampo.ParentId.Value;

            if (entity.IdEscalao > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<EscalaoDto, Escalao>(entity);
        }

        public Regime BuildRegimeObject(ValorCamposEditaveis valorCampo)
        {
            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            ParametrosAdicionais dataInicioClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DataInicio");
            ParametrosAdicionais dataFimClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DataFim");
            ParametrosAdicionais percEntidadeClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "TaxaEntidade");
            ParametrosAdicionais percTrabalhadorClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "TaxaTrabalhador");
            ParametrosAdicionais diaVencimentoClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DiaVencimento");

            DateTime dataInicio;
            DateTime? dataFim = null;
            decimal percEntidade;
            decimal percTrabalhador;
            int diaVencimento;

            if (dataInicioClass != null && dataFimClass != null && percEntidadeClass != null && percTrabalhadorClass != null &&
                diaVencimentoClass != null && dataInicioClass.DateValor.HasValue)
            {
                dataInicio = dataInicioClass.DateValor.Value;

                if (dataFimClass.DateValor.HasValue)
                    dataFim = dataFimClass.DateValor.Value;

                try
                {
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    percEntidade = Convert.ToDecimal(percEntidadeClass.Valor, CultureInfo.CreateSpecificCulture("en-Us"));
                }
                catch (Exception e)
                {
                    throw new Exception("The value could not be converted" + e.Message);
                }

                try
                {
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    percTrabalhador = Convert.ToDecimal(percTrabalhadorClass.Valor, CultureInfo.CreateSpecificCulture("en-Us"));
                }
                catch (Exception e)
                {
                    throw new Exception("The value could not be converted" + e.Message);
                }

                if (!int.TryParse(diaVencimentoClass.Valor, out diaVencimento))
                    diaVencimento = 0;
            }
            else
                throw new Exception("No Value was found for the entity");

            int tipoRegime = 0;
            if (valorCampo.ParentId.HasValue)
            {
                tipoRegime = _unitOfWork.RegimeRepository.GetTipoRegimeFromRegimePai(valorCampo.ParentId.Value).IdDominio;
            }

            if (tipoRegime == 0)
            {
                throw new Exception("No Value was found for tipoRegime");
            }

            RegimeDto entity = new RegimeDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.RegimeRepository.GetDto(valorCampo.Id);

            entity.IdRegime = valorCampo.Id;
            entity.NomeRegime = valorCampo.Nome;
            entity.IndActivo = true;
            entity.DataInicio = dataInicio;
            entity.DataFim = dataFim;
            entity.PercentEntidadeEmpreg = percEntidade;
            entity.PercentTrabalhador = percTrabalhador;
            entity.DataVencimento = diaVencimento;
            entity.RegimePai = valorCampo.ParentId.Value;
            entity.TipoRegime = tipoRegime;

            if (entity.IdRegime > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<RegimeDto, Regime>(entity);
        }

        private Taxajuromensal BuildTaxaJuroMensalObject(ValorCamposEditaveis valorCampo)
        {
            ParametrosAdicionais parameterClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Percentagem");
            ParametrosAdicionais dataInicioClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DataInicio");
            ParametrosAdicionais dataFimClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DataFim");

            DateTime dataInicio;
            DateTime? dataFim = null;
            decimal valor = 0;

            if (dataInicioClass != null && dataFimClass != null && dataInicioClass.DateValor.HasValue)
            {
                dataInicio = dataInicioClass.DateValor.Value;

                if (dataFimClass.DateValor.HasValue)
                {
                    dataFim = dataFimClass.DateValor.Value;
                    dataFim = new DateTime(dataFim.Value.Year, dataFim.Value.Month, 1);
                    dataFim = dataFim.Value.AddMonths(1);
                    dataFim = dataFim.Value.AddDays(-1);
                }
            }
            else
                throw new Exception("No Value was found for the entity");

            if (parameterClass != null)
            {
                try
                {
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    valor = Convert.ToDecimal(parameterClass.Valor, CultureInfo.CreateSpecificCulture("en-Us"));
                }
                catch (Exception e)
                {
                    throw new Exception("The value could not be converted" + e.Message);
                }
            }
            else
            {
                throw new Exception("No code was found for the entity");
            }

            TaxajuromensalDto entity = new TaxajuromensalDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.TaxaJuroMensalRepository.GetDto(valorCampo.Id);

            entity.IdTaxa = valorCampo.Id;
            entity.DataInicio = dataInicio;
            entity.DataFim = dataFim;
            entity.Percentagem = valor;
            entity.IndActivo = true;

            if (entity.IdTaxa > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<TaxajuromensalDto, Taxajuromensal>(entity);
        }

        private Orcamentoconfig BuildOrcamentoConfigObject(ValorCamposEditaveis valorCampo)
        {
            ParametrosAdicionais dataInicioClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DataInicio");
            ParametrosAdicionais dataFimClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DataFim");

            DateTime dataInicio;
            DateTime? dataFim = null;

            if (dataInicioClass != null && dataFimClass != null && dataInicioClass.DateValor.HasValue)
            {
                dataInicio = dataInicioClass.DateValor.Value;

                if (dataFimClass.DateValor.HasValue)
                    dataFim = dataFimClass.DateValor.Value;
            }
            else
                throw new Exception("No Value was found for the entity");

            OrcamentoConfigDto entity = new OrcamentoConfigDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.OrcamentoConfigRepository.GetDto(valorCampo.Id);

            entity.Id = valorCampo.Id;
            entity.IndActivo = true;
            entity.DataInicio = dataInicio;
            entity.DataFim = dataFim;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<OrcamentoConfigDto, Orcamentoconfig>(entity);
        }

        private Classificacao BuildClassificacaoObject(ValorCamposEditaveis valorCampo)
        {
            ClassificacaoDto entity = new ClassificacaoDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.ClassificacaoRepository.GetDto(valorCampo.Id);

            entity.Id = valorCampo.Id;
            entity.Nome = valorCampo.Nome;
            entity.IndActivo = true;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<ClassificacaoDto, Classificacao>(entity);
        }

        private Subclassificacao BuildSubClassificacaoObject(ValorCamposEditaveis valorCampo)
        {
            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            SubClassificacaoDto entity = new SubClassificacaoDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.SubClassificacaoRepository.GetDto(valorCampo.Id);

            entity.Id = valorCampo.Id;
            entity.Nome = valorCampo.Nome;
            entity.IndActivo = true;
            entity.ClassificacaoFk = valorCampo.ParentId.Value;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<SubClassificacaoDto, Subclassificacao>(entity);
        }

        private Centrocusto BuildCentroCustoObject(ValorCamposEditaveis valorCampo)
        {
            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            ParametrosAdicionais dataInicioClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DataInicio");
            ParametrosAdicionais dataFimClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DataFim");

            DateTime dataInicio;
            DateTime? dataFim = null;

            if (dataInicioClass != null && dataFimClass != null && dataInicioClass.DateValor.HasValue)
            {
                dataInicio = dataInicioClass.DateValor.Value;

                if (dataFimClass.DateValor.HasValue)
                    dataFim = dataFimClass.DateValor.Value;
            }
            else
                throw new Exception("No Value was found for the entity");

            CentroCustoDto entity = new CentroCustoDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.CentroCustoRepository.GetDto(valorCampo.Id);

            entity.Id = valorCampo.Id;
            entity.Descricao = valorCampo.Nome;
            entity.IndActivo = true;
            entity.OrcamentoconfigFk = valorCampo.ParentId.Value;
            entity.DataInicio = dataInicio;
            entity.DataFim = dataFim;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<CentroCustoDto, Centrocusto>(entity);
        }

        private Reltipodecontaorcamentoconfig BuildReltipodecontaorcamentoconfigObject(Orcamentoconfig orcamentoConfig, long idDominio)
        {
            RelTipoDeContaOrcamentoConfigDto entity = new RelTipoDeContaOrcamentoConfigDto();

            entity = _utils.SetDetailsToEntity<RelTipoDeContaOrcamentoConfigDto>(entity);
            Reltipodecontaorcamentoconfig rel = Utils.MappClassFromDto<RelTipoDeContaOrcamentoConfigDto, Reltipodecontaorcamentoconfig>(entity);
            rel.OrcamentoConfigFkNavigation = orcamentoConfig;
            rel.TipoContaFk = (int)idDominio;
            rel.IndActivo = true;
            return rel;
        }

        private Agrupamentoconfig BuildAgrupamenttoConfigFirstLevelObject(ValorCamposEditaveis valorCampo)
        {
            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            ParametrosAdicionais parameterClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Codigo");

            string codigo = "00";

            if (parameterClass != null)
            {
                codigo = parameterClass.Valor;
                if (codigo.Length > 2)
                    throw new Exception("The code is in the wrong format");
            }
            else
            {
                throw new Exception("No code was found for the entity");
            }

            AgrupamentoConfigDto entity = new AgrupamentoConfigDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.AgrupamentoConfigRepository.GetDto(valorCampo.Id);

            entity.Id = valorCampo.Id;
            entity.Designacao = valorCampo.Nome;
            entity.IndActivo = true;
            entity.Codigo = codigo;
            entity.ReltipoDeContaOrcamentoConfigFk = valorCampo.ParentId.Value;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<AgrupamentoConfigDto, Agrupamentoconfig>(entity);
        }

        private Agrupamentoconfig BuildAgrupamenttoConfigSubLevelObject(ValorCamposEditaveis valorCampo)
        {
            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            ParametrosAdicionais parameterClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Codigo");

            string codigo = "00";

            if (parameterClass != null)
            {
                codigo = parameterClass.Valor;
                if (codigo.Length > 2)
                    throw new Exception("The code is in the wrong format");
            }
            else
            {
                throw new Exception("No code was found for the entity");
            }

            Agrupamentoconfig parentAgrupamento = _unitOfWork.AgrupamentoConfigRepository.Get(valorCampo.ParentId.Value);

            AgrupamentoConfigDto entity = new AgrupamentoConfigDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.AgrupamentoConfigRepository.GetDto(valorCampo.Id);

            entity.Id = valorCampo.Id;
            entity.Designacao = valorCampo.Nome;
            entity.IndActivo = true;
            entity.Codigo = codigo;
            entity.ParentFk = valorCampo.ParentId.Value;
            entity.ReltipoDeContaOrcamentoConfigFk = parentAgrupamento.ReltipoDeContaOrcamentoConfigFk;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<AgrupamentoConfigDto, Agrupamentoconfig>(entity);
        }

        private Codigoconta BuildCodigoContaFirstLevelObject(ValorCamposEditaveis valorCampo)
        {
            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            ParametrosAdicionais parameterClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Codigo");

            string codigo = "00";

            decimal? initialValue = null;
            if (valorCampo.Parametros.FirstOrDefault(p => p.Nome == "ValorInicial")?.Valor != null)
                initialValue = Decimal.Parse(valorCampo.Parametros.FirstOrDefault(p => p.Nome == "ValorInicial")?.Valor, CultureInfo.InvariantCulture);
            DateTime? initialValueDate = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DataValorInicial")?.DateValor;
            bool? isCredit = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Checkbox")?.Credit;

            if (parameterClass != null)
            {
                codigo = parameterClass.Valor;
                if (codigo.Length > 2)
                    throw new Exception("The code is in the wrong format");
            }
            else
            {
                throw new Exception("No code was found for the entity");
            }

            CodigoContaDto entity = new CodigoContaDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.CodigoContaRepository.GetDto(valorCampo.Id);

            entity.Id = valorCampo.Id;
            entity.Designacao = valorCampo.Nome;
            entity.IndActivo = true;
            entity.Codigo = codigo;
            entity.OrcamentoConfigFk = valorCampo.ParentId.Value;
            entity.InitialValue = initialValue;
            entity.InitialValueDate = initialValueDate;
            entity.IsCredit = isCredit;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<CodigoContaDto, Codigoconta>(entity);
        }

        private Codigoconta BuildCodigoContaSubLevelObject(ValorCamposEditaveis valorCampo)
        {
            if (!valorCampo.ParentId.HasValue)
                throw new Exception("No ParentId was found for the entity");

            ParametrosAdicionais parameterClass = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Codigo");

            string codigo = "00";

            decimal? initialValue = null;
            if (valorCampo.Parametros.FirstOrDefault(p => p.Nome == "ValorInicial")?.Valor != null)
                initialValue = Decimal.Parse(valorCampo.Parametros.FirstOrDefault(p => p.Nome == "ValorInicial")?.Valor.ToString(), CultureInfo.InvariantCulture);
            DateTime? initialValueDate = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "DataValorInicial")?.DateValor;
            bool? isCredit = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Checkbox")?.Credit;

            if (parameterClass != null)
            {
                codigo = parameterClass.Valor;
                if (codigo.Length > 2)
                    throw new Exception("The code is in the wrong format");
            }
            else
            {
                throw new Exception("No code was found for the entity");
            }

            CodigoContaDto entity = new CodigoContaDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.CodigoContaRepository.GetDto(valorCampo.Id);
            
            var parentEntity = _unitOfWork.CodigoContaRepository.GetDto(valorCampo.ParentId.Value);

            entity.Id = valorCampo.Id;
            entity.Designacao = valorCampo.Nome;
            entity.IndActivo = true;
            entity.Codigo = codigo;
            entity.OrcamentoConfigFk = parentEntity.OrcamentoConfigFk;
            entity.ParentFk = valorCampo.ParentId.Value;
            entity.InitialValue = initialValue;
            entity.InitialValueDate = initialValueDate;
            entity.IsCredit = isCredit;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<CodigoContaDto, Codigoconta>(entity);
        }

        private Relcodigocontaagrupamentoconfig BuildRelCodigoContaAgrupamentoConfig(int agrupamento, int? codigoConta = null)
        {
            var result = new Relcodigocontaagrupamentoconfig()
            {
                AgrupamentoConfigFk = agrupamento,
                IndActivo = true
            };
            if (codigoConta.HasValue)
                result.CodigocontaFk = codigoConta.Value;
            result = _utils.SetDetailsToEntity(result);
            return result;
        }

        private Contabancaria BuildContaBancariaObject(ValorCamposEditaveis valorCampo)
        {
            ParametrosAdicionais parameterClassSwift = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "SWIFT");
            ParametrosAdicionais parameterClassEntidade = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "EntidadeBancaria");
            ParametrosAdicionais parameterClassDescricao = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Descricao");
            ParametrosAdicionais parameterClassIban = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "IBAN");
            ParametrosAdicionais parameterClassNumero = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "NConta");

            ContaBancariaDto entity = new ContaBancariaDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.ContaBancariaRepository.GetDto(valorCampo.Id);

            entity.Id = valorCampo.Id;
            entity.Swift = parameterClassSwift.Valor;
            entity.EntidadeBancaria = parameterClassEntidade.Valor;
            entity.Descricao = parameterClassDescricao.Valor;
            entity.Iban = parameterClassIban.Valor;
            entity.Numero = parameterClassNumero.Valor;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<ContaBancariaDto, Contabancaria>(entity);
        }

        private Movimentobancario BuildMovimentoBancarioObject(ValorCamposEditaveis valorCampo)
        {
            ParametrosAdicionais parameterClassDescricao = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "Descricao");
            ParametrosAdicionais parameterClassTipo = valorCampo.Parametros.FirstOrDefault(p => p.Nome == "TipoMovimento");

            MovimentoBancarioDto entity = new MovimentoBancarioDto();

            if (valorCampo.Id > 0)
                entity = _unitOfWork.MovimentoBancarioRepository.GetDto(valorCampo.Id);

            entity.Id = valorCampo.Id;
            entity.Descricao = parameterClassDescricao.Valor;
            entity.TipoMovimento = parameterClassTipo.SelectedDropdown.Value;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<MovimentoBancarioDto, Movimentobancario>(entity);
        }

        #endregion Builders

        #region Validators

        public ResponseBaseDataContract ValidateCreateTaxaJuroMensal(Taxajuromensal taxa)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool valid = _unitOfWork.TaxaJuroMensalRepository.IsTaxaDatesValid(taxa);

            if (!valid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DatasCampoConflito).ToString(),
                    ErrorMessage = ErrorsDataContract.DatasCampoConflito.ToString()
                });

            return response;
        }

        public ResponseBaseDataContract ValidateCreateOrcamentoConfig(Orcamentoconfig orcamento)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            var err = _unitOfWork.OrcamentoConfigRepository.IsOrcamentoValid(orcamento);

            if (err != null)
                response.Errors.Add(err);

            return response;
        }

        public ResponseBaseDataContract ValidateDeleteOrcamentoConfig(Orcamentoconfig orcamento)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool valid = _unitOfWork.OrcamentoConfigRepository.IsOrcamentoDeleteValid(orcamento);

            if (!valid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityHasDependents).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityHasDependents.ToString()
                });

            return response;
        }

        public ResponseBaseDataContract ValidateCreateCentroCusto(Centrocusto centroCusto)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            Orcamentoconfig orcamento = _unitOfWork.OrcamentoConfigRepository.Get(centroCusto.OrcamentoconfigFk);

            if (orcamento.DataInicio > centroCusto.DataInicio)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DatasCampoConflito).ToString(),
                    ErrorMessage = ErrorsDataContract.DatasCampoConflito.ToString()
                });
            }
            else if (orcamento.DataFim.HasValue && centroCusto.DataFim.HasValue)
                if (orcamento.DataFim < centroCusto.DataFim)
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.DatasCampoConflito).ToString(),
                        ErrorMessage = ErrorsDataContract.DatasCampoConflito.ToString()
                    });

            var err = _unitOfWork.OrcamentoConfigRepository.IsComponenteOrcamentoValid(centroCusto.DataInicio, centroCusto.DataFim);

            if (err != null)
            {
                response.Errors.Add(err);
            }

            return response;
        }

        public ResponseBaseDataContract ValidateDeleteCentroCusto(Centrocusto centroCusto)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool valid = _unitOfWork.CentroCustoRepository.IsCentroCustoDeleteValid(centroCusto);

            if (!valid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityHasDependents).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityHasDependents.ToString()
                });

            return response;
        }

        public ResponseBaseDataContract ValidateCreateAgrupamentoConfig(Agrupamentoconfig agrupamento)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool valid = _unitOfWork.AgrupamentoConfigRepository.IsCodeValid(agrupamento);

            if (!valid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.UniqueCodeExists).ToString(),
                    ErrorMessage = ErrorsDataContract.UniqueCodeExists.ToString()
                });

            return response;
        }

        public ResponseBaseDataContract ValidateDeleteAgrupamento(Agrupamentoconfig agrupamento)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool valid = _unitOfWork.AgrupamentoConfigRepository.IsAgrupamentoDeleteValid(agrupamento);

            if (!valid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.CampoAlreadyUsed).ToString(),
                    ErrorMessage = ErrorsDataContract.CampoAlreadyUsed.ToString()
                });

            bool invalid = _unitOfWork.AgrupamentoConfigRepository.AgrupamentoHasChilds(agrupamento);

            if (invalid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityHasDependents).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityHasDependents.ToString()
                });

            return response;
        }

        public ResponseBaseDataContract ValidateCreateCodigoConta(Codigoconta codigoConta)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool valid = _unitOfWork.CodigoContaRepository.IsCodeValid(codigoConta);

            if (!valid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.UniqueCodeExists).ToString(),
                    ErrorMessage = ErrorsDataContract.UniqueCodeExists.ToString()
                });

            return response;
        }

        public ResponseBaseDataContract ValidateDeleteCodigoConta(Codigoconta codigoConta)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool invalid = _unitOfWork.CodigoContaRepository.CodigoContaHasChilds(codigoConta);

            if (invalid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityHasDependents).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityHasDependents.ToString()
                });

            invalid = _unitOfWork.CodigoContaRepository.IsCodigoContaInUse(codigoConta);

            if (invalid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.CampoAlreadyUsed).ToString(),
                    ErrorMessage = ErrorsDataContract.CampoAlreadyUsed.ToString()
                });

            return response;
        }

        public ResponseBaseDataContract ValidateCreateContaBancaria(Contabancaria contaBancaria)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool valid = _unitOfWork.ContaBancariaRepository.IsIbanValid(contaBancaria);

            if (!valid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.UniqueCodeExists).ToString(),
                    ErrorMessage = ErrorsDataContract.UniqueCodeExists.ToString()
                });

            return response;
        }

        public ResponseBaseDataContract ValidateCreateMovimentoBancario(Movimentobancario movimento)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool valid = _unitOfWork.MovimentoBancarioRepository.IsMovimentoValid(movimento);

            if (!valid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.UniqueCodeExists).ToString(),
                    ErrorMessage = ErrorsDataContract.UniqueCodeExists.ToString()
                });

            return response;
        }

        public ResponseBaseDataContract ValidateUpdateEscalao(Escalao escalao)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool valid = _unitOfWork.EscalaoRepository.IsEscalaoDeclared(escalao);

            if (!valid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.CampoAlreadyUsed).ToString(),
                    ErrorMessage = ErrorsDataContract.CampoAlreadyUsed.ToString()
                });

            return response;
        }

        #endregion Validators

        public ResponseBaseDataContract ManageAgrupamentosRelations(List<int> agrupamentosRel, Codigoconta codigoConta)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            Relcodigocontaagrupamentoconfig ite;
            if (codigoConta.Id != 0)
            {
                List<Relcodigocontaagrupamentoconfig> existentRels = _unitOfWork.RelCodigoContaAgrupamentoConfigRepository.GetAllByCodigoConta(codigoConta.Id);

                List<int> existentAgrupamentos = existentRels.Select(r => r.AgrupamentoConfigFk).ToList();
                List<int> agrupamentosToAdd = agrupamentosRel.Where(a => !existentAgrupamentos.Contains(a)).ToList();
                List<Relcodigocontaagrupamentoconfig> agrupamentosToRemove = existentRels.Where(a => !agrupamentosRel.Contains(a.AgrupamentoConfigFk)).ToList();

                foreach (Relcodigocontaagrupamentoconfig rel in agrupamentosToRemove)
                {
                    response = ValidateDeleteRelCodigoContaAgrupamentoConfig(rel);
                    if (response.Errors.Count > 0)
                        return response;

                    rel.IndActivo = false;
                    ite = _utils.UpdateDetailsToEntity(rel);
                    _unitOfWork.RelCodigoContaAgrupamentoConfigRepository.Update(ite);
                }

                foreach (int agrupamentoID in agrupamentosToAdd)
                {
                    ite = BuildRelCodigoContaAgrupamentoConfig(agrupamentoID, codigoConta.Id);
                    _unitOfWork.RelCodigoContaAgrupamentoConfigRepository.Add(ite);
                }
            }
            else
            {
                foreach (int agrupamentoID in agrupamentosRel)
                {
                    ite = BuildRelCodigoContaAgrupamentoConfig(agrupamentoID);
                    ite.CodigocontaFkNavigation = codigoConta;
                    _unitOfWork.RelCodigoContaAgrupamentoConfigRepository.Add(ite);
                }
            }
            return response;
        }

        public ResponseBaseDataContract ValidateDeleteRelCodigoContaAgrupamentoConfig(Relcodigocontaagrupamentoconfig rel)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool invalid = _unitOfWork.RelCodigoContaAgrupamentoConfigRepository.IsRelBeingUsed(rel);

            if (invalid)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.CampoAlreadyUsed).ToString(),
                    ErrorMessage = ErrorsDataContract.CampoAlreadyUsed.ToString()
                });

            return response;
        }
    }
}