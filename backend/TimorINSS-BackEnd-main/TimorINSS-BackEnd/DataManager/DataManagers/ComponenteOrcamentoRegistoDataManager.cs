using Microsoft.Extensions.Localization;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using TimorINSSBackEnd.Resources;
using ClosedXML.Excel;

using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.Rendering;




namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ComponenteOrcamentoRegistoDataManager : IComponenteOrcamentoRegistoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ComponenteOrcamentoRegistoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _localizer = localizer;
        }

        public GetComponenteOrcamentoRegistoReponse GetComponenteOrcamentoRegisto(GetComponenteOrcamentoRegistoRequest request)
        {
            GetComponenteOrcamentoRegistoReponse response = new GetComponenteOrcamentoRegistoReponse();

            Tarefaativo tarefa = _unitOfWork.TarefaAtivoRepository.Get(request.IdTarefaActivo);
            ComponenteorcamentoRegisto componente = null;

            if (tarefa != null)
            {
                componente = _unitOfWork.ComponenteOrcamentoRegistoRepository.GetByIdProcessoActivo(tarefa.ProcessoAtivoFk);
            }

            if (componente != null)
            {
                response.ComponenteOrcamentoRegisto = new ComponenteOrcamentoRegistoDataContract
                {
                    Id = componente.Id,
                    DataInicio = componente.DataInicio,
                    DataFim = componente.DataFim,
                    TarefaActivoFk = componente.TarefaActivoFk,
                    Aprovado = componente.Aprovado
                };

                ComponenteOrcamentoValorSearch filter = new ComponenteOrcamentoValorSearch
                {
                    Id = componente.Id
                };
                List<Componenteorcamentovalor> valores = _unitOfWork.ComponenteOrcamentoValorRepository.SearchComponenteOrcamentoValor(filter);

                response.ValoresCorrentes = new List<ComponenteOrcamentoValorFullDataContract>();

                string fullCode = "";
                bool editavel = true;
                string actidadeDes = "";
                //string economicDes = "";
                string funcionalDes = "";
                foreach (Componenteorcamentovalor valor in valores)
                {
                    fullCode = valor.AgrupamentoFk.HasValue ? _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(valor.AgrupamentoFk.Value) : "";
                    editavel = valor.AgrupamentoFkNavigation == null || valor.AgrupamentoFkNavigation.InverseParentFkNavigation.Count == 0;
                    //editavel = true;
                    actidadeDes = valor.ActidadeFk.HasValue ? _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(valor.ActidadeFk.Value) : "";
                    
                    funcionalDes = valor.FuncionalFk.HasValue ? _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(valor.FuncionalFk.Value) : "";


                    response.ValoresCorrentes.Add(new ComponenteOrcamentoValorFullDataContract
                    {
                        Id = valor.Id,
                        AgrupamentoFk = valor.AgrupamentoFk,
                        CentroCustoFk = valor.CentroCustoFk,
                        ComponenteOrcamentoRegistoFk = valor.ComponenteOrcamentoRegistoFk,
                        DepartamentoDescricao = valor.DepartamentoFkNavigation != null ? valor.DepartamentoFkNavigation.Nome : "",
                        InstitutionDescricao = valor.InstitutionFkNavigation != null ? valor.InstitutionFkNavigation.Nome : "",
                        CentroCustoDescricao = valor.CentroCustoFkNavigation != null ? valor.CentroCustoFkNavigation.Descricao : "",
                        DepartamentoFk = valor.DepartamentoFk,
                        InstitutionId = valor.InstitutionId,
                        FuncionalFk=valor.FuncionalFk,
                        ActidadeFk=valor.ActidadeFk,
                        Valor = valor.Valor,
                        TipoDeConta = valor.TipoContaFk ?? 0,
                        TipoDeContaDescricao = valor.TipoContaFkNavigation?.Descricao,
                        Codigo = fullCode,
                        ActidadeDescricao = actidadeDes,
                        FuncionalDescricao = funcionalDes,
                        Descricao = valor.AgrupamentoFkNavigation?.Designacao,
                        Editavel = editavel
                    });
                }

                response.ValoresCorrentes = response.ValoresCorrentes.OrderBy(v => v.TipoDeConta).ThenBy(v => v.Codigo).ToList();

                Orcamentoconfig orcamento = _unitOfWork.OrcamentoConfigRepository.GetOrcamentoConfigByDates(componente.DataInicio, componente.DataFim);

                response.Agrupamentos = _unitOfWork.AgrupamentoConfigRepository.GetAlllActivAgrupamentoConfigByOrcamentoConfig(orcamento.Id);
                int tipoContaActidateId = _unitOfWork.DominioRepository.getDominioByDescricao(TiposDominio.TIPOCONTA, "Actidade").IdDominio;
                int tipoContaFunctionalId = _unitOfWork.DominioRepository.getDominioByDescricao(TiposDominio.TIPOCONTA, "Funcional").IdDominio;
                int orcamentoId = _unitOfWork.OrcamentoConfigRepository.GetOrcamentoConfigCurrentDate().Id;

                response.Actidades = _unitOfWork.AgrupamentoConfigRepository.GetActidadesAgrupamentoConfigByOrcamentoConfig(orcamentoId, tipoContaActidateId);
                response.Functionals = _unitOfWork.AgrupamentoConfigRepository.GetActidadesAgrupamentoConfigByOrcamentoConfig(orcamentoId, tipoContaFunctionalId);
                response.CentrosCusto = _unitOfWork.CentroCustoRepository.GetAllActiveCentroCustoByOrcamentoRegisto(componente.Id);
                response.TiposDeConta = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPOCONTA);
            }

            return response;
        }

        public GetComponenteOrcamentoAprovadoRegistoReponse GetOrcamentoAprovadoDespesaByIdTarefaActivo(GetComponenteOrcamentoRegistoAprovadoRequest request)
        {
            GetComponenteOrcamentoAprovadoRegistoReponse response = new GetComponenteOrcamentoAprovadoRegistoReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            DateTime dataInicioProcesso;
            Tarefaativo tarefaAtivo = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.IdTarefaActivo);

            if (tarefaAtivo != null)
            {
                response.ProcessoId = tarefaAtivo.ProcessoAtivoFk;

                dataInicioProcesso = tarefaAtivo.ProcessoAtivoFkNavigation.DataCriacao;

                //Vai buscar o registo de orçamento por data de inicio do processo pois existe um orçamento aprovado
                var componente = _unitOfWork.ComponenteOrcamentoRegistoRepository.GetOrcamentoAprovadoByDataPInicioProcesso(dataInicioProcesso);
                if (componente != null)
                {
                    //Vai buscar a configuração do orçamento e preenhe os dados necessários
                    var orcamento = _unitOfWork.OrcamentoConfigRepository.Get(componente.OrcamentoConfigFk);

                    if (orcamento != null)
                    {
                        response.CodigoConta = _unitOfWork.CodigoContaRepository.GetAllActivCodigoContaByOrcamentoConfig(orcamento.Id);
                        response.CentrosCusto = _unitOfWork.CentroCustoRepository.GetAllActiveCentroCustoByOrcamentoRegisto(componente.Id);
                        response.TiposDeConta = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPOCONTA);
                        response.existeOrcamentoAprovado = true;
                    }
                    else
                    {
                        response.existeOrcamentoAprovado = true;
                    }
                    response.IdOrcamentoRegisto = componente.Id;
                }
                else
                {
                    response.existeOrcamentoAprovado = false;
                }

                // obter causo houver, o número do pagamento executado
                var pagamentoExecutado = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosByIdProcessoAtivoEstado(tarefaAtivo.ProcessoAtivoFkNavigation.Id,
                _unitOfWork.DominioRepository.getIdDominio("ESTADOPAGAMENTO", 1));

                if (pagamentoExecutado != null && pagamentoExecutado.Count > 0)
                {
                    response.NumPagamento = pagamentoExecutado[0].NumeroPagamento;

                    response.ListaPagamentosDestinatario = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosByNumPagamento(response.NumPagamento);

                    if (response.ListaPagamentosDestinatario != null && response.ListaPagamentosDestinatario.Count > 0)
                    {
                        foreach (var pagamentoDestinatario in response.ListaPagamentosDestinatario)
                        {
                            var idDestinatario = pagamentoDestinatario.Destinatario.Id;
                            //Logica consoante se temos uma relação com entidade ou com trabalhador, e vai buscar as informações consoante o que temos disponível
                            if (pagamentoDestinatario.Destinatario != null && pagamentoDestinatario.Destinatario.EntidadeFk != null
                                && pagamentoDestinatario.Destinatario.EntidadeFk > 0)
                            {
                                pagamentoDestinatario.Destinatario = _unitOfWork.EntidadeEmpregadoraRepository.GetDestinatarioByIdEntidade((int)pagamentoDestinatario.Destinatario.EntidadeFk);

                                if (pagamentoDestinatario.Destinatario != null)
                                {
                                    pagamentoDestinatario.Destinatario.Id = idDestinatario;
                                    var moradaDestinatario = _unitOfWork.MoradaRepository.GetMoradasPrincipalByEntidadeFk((int)pagamentoDestinatario.Destinatario.EntidadeFk);

                                    if (moradaDestinatario != null)
                                    {
                                        pagamentoDestinatario.Destinatario.Morada = BuildMoradaString(moradaDestinatario);
                                    }
                                }
                            }

                            if (pagamentoDestinatario.Destinatario != null && pagamentoDestinatario.Destinatario.TrabalhadorFk != null
                                && pagamentoDestinatario.Destinatario.TrabalhadorFk > 0)
                            {
                                pagamentoDestinatario.Destinatario = _unitOfWork.TrabalhadoresRepository.GetDestinatarioByIdTrabalhador((int)pagamentoDestinatario.Destinatario.TrabalhadorFk);
                                if (pagamentoDestinatario.Destinatario != null)
                                {
                                    pagamentoDestinatario.Destinatario.Id = idDestinatario;
                                    var moradaDestinatario = _unitOfWork.MoradaRepository.GetMoradasPrincipalByEntidadeFk((int)pagamentoDestinatario.Destinatario.TrabalhadorFk);

                                    if (moradaDestinatario != null)
                                    {
                                        pagamentoDestinatario.Destinatario.Morada = BuildMoradaString(moradaDestinatario);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    pagamentoExecutado = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosByIdProcessoAtivo(tarefaAtivo.ProcessoAtivoFkNavigation.Id);

                    if (pagamentoExecutado != null && pagamentoExecutado.Count > 0)
                    {
                        // buscar o ultimo pagamento executado

                        int numSequencial = int.Parse(pagamentoExecutado[^1].NumeroPagamento.Substring(0, 1));
                        response.NumPagamento = numSequencial + 1 + "/" + tarefaAtivo.ProcessoAtivoFkNavigation.NumeroProcesso;
                    }
                    else
                    {
                        response.NumPagamento = "1" + "/" + tarefaAtivo.ProcessoAtivoFkNavigation.NumeroProcesso;
                    }
                }
            }
            else
            {
                response.existeOrcamentoAprovado = false;
            }
            return response;
        }

        public UpdateComponenteOrcamentoRegistoDatesResponse UpdateComponenteOrcamentoRegistoDates(UpdateComponenteOrcamentoRegistoDatesRequest request)
        {
            UpdateComponenteOrcamentoRegistoDatesResponse response = new UpdateComponenteOrcamentoRegistoDatesResponse();

            Tarefaativo tarefa = _unitOfWork.TarefaAtivoRepository.Get(request.IdTarefaActivo);
            ComponenteorcamentoRegisto componenteOrcamentoRegisto = null;

            if (tarefa != null)
            {
                componenteOrcamentoRegisto = _unitOfWork.ComponenteOrcamentoRegistoRepository.GetByIdProcessoActivo(tarefa.ProcessoAtivoFk);
            }

            bool criacao = componenteOrcamentoRegisto == null;

            if (request.DataFim <= request.DataInicio)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroDataFimRegistoOrcamento).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroDataFimRegistoOrcamento.ToString()
                });
                return response;
            }

            Orcamentoconfig orcamento = _unitOfWork.OrcamentoConfigRepository.GetOrcamentoConfigByDates(request.DataInicio, request.DataFim);

            if (orcamento == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.OrcamentoNaoConfigurado).ToString(),
                    ErrorMessage = ErrorsDataContract.OrcamentoNaoConfigurado.ToString()
                });
                return response;
            }

            List<ComponenteorcamentoRegisto> orcamentosAprovados = _unitOfWork.ComponenteOrcamentoRegistoRepository.GetComponentesOrcamentoRegistoAprovado(request.DataInicio, request.DataFim);
            if (orcamentosAprovados.Count > 0)
            {
                if (orcamentosAprovados.Count > 1)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.VariosOrcamentoAprovados).ToString(),
                        ErrorMessage = ErrorsDataContract.VariosOrcamentoAprovados.ToString()
                    });
                    return response;
                }
                else
                {
                    ComponenteorcamentoRegisto orcamentoAprovado = orcamentosAprovados[0];
                    if (orcamentoAprovado.InverseOrcamentoRetificadoFkNavigation.Count > 0)
                    {
                        ComponenteorcamentoRegisto retificado = orcamentoAprovado.InverseOrcamentoRetificadoFkNavigation.First();
                        if (!criacao)
                        {
                            if (retificado.Id != componenteOrcamentoRegisto.Id)
                            {
                                response.Errors.Add(new Error
                                {
                                    ErrorCode = ((int)ErrorsDataContract.OrcamentoRetificado).ToString(),
                                    ErrorMessage = ErrorsDataContract.OrcamentoRetificado.ToString()
                                });
                                return response;
                            }
                        }
                        else
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.OrcamentoRetificado).ToString(),
                                ErrorMessage = ErrorsDataContract.OrcamentoRetificado.ToString()
                            });
                            return response;
                        }
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(((int)ErrorsDataContract.OrcamentoARetificar).ToString());
                        // ÿ é o valor de separador de datas para ser identificado no front para poder traduzir a mensagem de warning - ex: "As datas x e z (xÿz) sobrepoem a uma data de orçamento já existente"
                        sb.Append("ÿ");
                        sb.Append(orcamentoAprovado.DataInicio.ToString("dd-MM-yyyy"));
                        sb.Append("ÿ");
                        sb.Append(orcamentoAprovado.DataFim.ToString("dd-MM-yyyy"));
                        response.Errors.Add(new Error
                        {
                            ErrorCode = sb.ToString(),
                            ErrorMessage = ErrorsDataContract.OrcamentoARetificar.ToString()
                        });
                        return response;
                    }
                }
            }

            List<SelectDescription> centrosDeCusto = new List<SelectDescription>();
            if (!criacao)
            {
                centrosDeCusto = _unitOfWork.CentroCustoRepository.GetAllActiveCentroCustoByOrcamentoRegisto(componenteOrcamentoRegisto.Id);
                Orcamentoconfig orcamentoOriginal = _unitOfWork.OrcamentoConfigRepository.GetOrcamentoConfigByDates(componenteOrcamentoRegisto.DataInicio, componenteOrcamentoRegisto.DataFim);

                ComponenteOrcamentoValorSearch filter = new ComponenteOrcamentoValorSearch
                {
                    Id = componenteOrcamentoRegisto.Id
                };
                List<Componenteorcamentovalor> valores = _unitOfWork.ComponenteOrcamentoValorRepository.SearchComponenteOrcamentoValor(filter);

                if (orcamentoOriginal.Id != orcamento.Id && valores.Count > 0)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.OrcamentoConfigDif).ToString(),
                        ErrorMessage = ErrorsDataContract.OrcamentoConfigDif.ToString()
                    });
                    return response;
                }

                foreach (Componenteorcamentovalor valor in valores)
                {
                    if (!centrosDeCusto.Exists(c => c.id == valor.CentroCustoFk))
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.CentroCustoConflit).ToString(),
                            ErrorMessage = ErrorsDataContract.CentroCustoConflit.ToString()
                        });
                        return response;
                    }
                }
            }

            try
            {
                if (!criacao)
                {
                    componenteOrcamentoRegisto = BuildUpdatedComponenteOrcamentoRegistoObject(componenteOrcamentoRegisto, request.DataInicio, request.DataFim, orcamento.Id);
                    _unitOfWork.ComponenteOrcamentoRegistoRepository.Update(componenteOrcamentoRegisto);
                }
                else
                {
                    componenteOrcamentoRegisto = BuildComponenteOrcamentoRegistoObject(request.IdTarefaActivo, request.DataInicio, request.DataFim, orcamento.Id);
                    _unitOfWork.ComponenteOrcamentoRegistoRepository.Add(componenteOrcamentoRegisto);
                    response.UpdateValues = true;
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
                return response;
            }

            if (criacao)
                centrosDeCusto = _unitOfWork.CentroCustoRepository.GetAllActiveCentroCustoByOrcamentoRegisto(componenteOrcamentoRegisto.Id);

            if (response.UpdateValues)
            {
                response.Agrupamentos = _unitOfWork.AgrupamentoConfigRepository.GetAlllActivAgrupamentoConfigByOrcamentoConfig(orcamento.Id);
                response.TiposDeConta = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPOCONTA);
            }
            response.CentrosCusto = centrosDeCusto;

            return response;
        }

        public GetComponenteOrcamentoRegistoReponse RetificarOrcamentoAprovado(UpdateComponenteOrcamentoRegistoDatesRequest request)
        {
            GetComponenteOrcamentoRegistoReponse response = new GetComponenteOrcamentoRegistoReponse();

            List<ComponenteorcamentoRegisto> orcamentosAprovados = _unitOfWork.ComponenteOrcamentoRegistoRepository.GetComponentesOrcamentoRegistoAprovado(request.DataInicio, request.DataFim);

            Tarefaativo tarefa = _unitOfWork.TarefaAtivoRepository.Get(request.IdTarefaActivo);

            // não deixa retificar se o orçamento não estiver aprovado
            if (orcamentosAprovados.Count == 0)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.OrcamentoNaoConfigurado).ToString(),
                    ErrorMessage = ErrorsDataContract.OrcamentoNaoConfigurado.ToString()
                });
                return response;
            }

            //não deixa retificar se existir mais de um orçamento aprovado para o período
            else if (orcamentosAprovados.Count > 1)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.VariosOrcamentoAprovados).ToString(),
                    ErrorMessage = ErrorsDataContract.VariosOrcamentoAprovados.ToString()
                });
                return response;
            }

            ComponenteorcamentoRegisto orcamentoAprovado = orcamentosAprovados[0];

            //Manda erro caso já haja um orçamento aprovado e tentem retificar para o mesmo processo
            if (orcamentoAprovado.InverseOrcamentoRetificadoFkNavigation.Count > 0)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.OrcamentoRetificado).ToString(),
                    ErrorMessage = ErrorsDataContract.OrcamentoRetificado.ToString()
                });
                return response;
            }

            if (tarefa != null && orcamentoAprovado.TarefaActivoFkNavigation != null && tarefa.ProcessoAtivoFk == orcamentoAprovado.TarefaActivoFkNavigation.ProcessoAtivoFk)
            {

                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroRetificarOrcamentoMesmoProcesso).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroRetificarOrcamentoMesmoProcesso.ToString()
                });
                return response;

            }

            List<ComponentedespesaRegisto> componenteDespesa = _unitOfWork.ComponenteDespesaRegistoRepository.GetComponenteDespesaRegistoByIdOrcamento(orcamentoAprovado.Id, request.DataInicio, request.DataFim);
            List<ComponentereceitaRegisto> componenteReceita = _unitOfWork.ComponenteReceitaRegistoRepository.GetComponenteReceitaRegistoByIdOrcamento(orcamentoAprovado.Id, request.DataInicio, request.DataFim);

            if ((componenteDespesa != null && componenteDespesa.Count > 0) || (componenteReceita != null && componenteReceita.Count > 0))
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.existemValoresExecutados).ToString(),
                    ErrorMessage = ErrorsDataContract.existemValoresExecutados.ToString()
                });
                return response;
            }

            response.Agrupamentos = _unitOfWork.AgrupamentoConfigRepository.GetAlllActivAgrupamentoConfigByOrcamentoConfig(orcamentoAprovado.OrcamentoConfigFk);
            response.TiposDeConta = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPOCONTA);
            response.CentrosCusto = _unitOfWork.CentroCustoRepository.GetAllActiveCentroCustoByOrcamentoRegisto(orcamentoAprovado.Id);

            ComponenteorcamentoRegisto orcamentoRetificado = _unitOfWork.ComponenteOrcamentoRegistoRepository.GetByIdTarefaActivo(request.IdTarefaActivo);
            try
            {
                ComponenteOrcamentoValorDto dto;
                Componenteorcamentovalor componenteIte;
                if (orcamentoRetificado == null)
                {
                    orcamentoRetificado = BuildComponenteOrcamentoRegistoObject(request.IdTarefaActivo, orcamentoAprovado.DataInicio, orcamentoAprovado.DataFim, orcamentoAprovado.OrcamentoConfigFk);
                    orcamentoRetificado.OrcamentoRetificadoFk = orcamentoAprovado.Id;
                    _unitOfWork.ComponenteOrcamentoRegistoRepository.Add(orcamentoRetificado);
                }
                else
                {
                    List<Componenteorcamentovalor> componentesAEliminar = _unitOfWork.ComponenteOrcamentoValorRepository.getAllComponentesOrcamentoValorByRegisto(orcamentoRetificado.Id);

                    foreach (Componenteorcamentovalor componente in componentesAEliminar)
                    {
                        dto = Utils.MappClassToDto<Componenteorcamentovalor, ComponenteOrcamentoValorDto>(componente);
                        dto = _utils.SetDetailsToEntity(dto);
                        dto.IndActivo = false;
                        componenteIte = Utils.MappClassFromDto<ComponenteOrcamentoValorDto, Componenteorcamentovalor>(dto);
                        _unitOfWork.ComponenteOrcamentoValorRepository.Update(componenteIte);
                    }

                    orcamentoRetificado = BuildUpdatedComponenteOrcamentoRegistoObject(orcamentoRetificado, orcamentoAprovado.DataInicio, orcamentoAprovado.DataFim, orcamentoAprovado.OrcamentoConfigFk);
                    orcamentoRetificado.OrcamentoRetificadoFk = orcamentoAprovado.Id;
                    _unitOfWork.ComponenteOrcamentoRegistoRepository.Update(orcamentoRetificado);
                }

                List<Componenteorcamentovalor> componentesAReplicar = _unitOfWork.ComponenteOrcamentoValorRepository.getAllComponentesOrcamentoValorByRegisto(orcamentoAprovado.Id);
                foreach (Componenteorcamentovalor componente in componentesAReplicar)
                {
                    componenteIte = BuildComponenteOrcamentoValorReplicadoObject(componente);
                    componenteIte.ComponenteOrcamentoRegistoFkNavigation = orcamentoRetificado;
                    _unitOfWork.ComponenteOrcamentoValorRepository.Add(componenteIte);
                }

                _unitOfWork.Commit();

                ComponenteOrcamentoValorSearch filter = new ComponenteOrcamentoValorSearch
                {
                    Id = orcamentoRetificado.Id
                };
                List<Componenteorcamentovalor> valores = _unitOfWork.ComponenteOrcamentoValorRepository.SearchComponenteOrcamentoValor(filter);

                response.ValoresCorrentes = new List<ComponenteOrcamentoValorFullDataContract>();

                string fullCode = "";
                bool editavel = true;
                foreach (Componenteorcamentovalor valor in valores)
                {
                    //fullCode = _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(valor.AgrupamentoFk);
                    editavel = valor.AgrupamentoFkNavigation.InverseParentFkNavigation.Count == 0;
                    response.ValoresCorrentes.Add(new ComponenteOrcamentoValorFullDataContract
                    {
                        Id = valor.Id,
                        //AgrupamentoFk = valor.AgrupamentoFk,
                        CentroCustoFk = valor.CentroCustoFk,
                        //ComponenteOrcamentoRegistoFk = valor.ComponenteOrcamentoRegistoFk,
                        DepartamentoFk = valor.DepartamentoFk,
                        DepartamentoDescricao = valor.DepartamentoFkNavigation != null ? valor.DepartamentoFkNavigation.Nome : "",
                        CentroCustoDescricao = valor.CentroCustoFkNavigation != null ? valor.CentroCustoFkNavigation.Descricao : "",
                        Valor = valor.Valor,
                        TipoDeConta = valor.AgrupamentoFkNavigation.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFkNavigation.IdDominio,
                        TipoDeContaDescricao = valor.AgrupamentoFkNavigation.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFkNavigation.Descricao,
                        Codigo = fullCode,
                        Descricao = valor.AgrupamentoFkNavigation.Designacao,
                        Editavel = editavel
                    });
                }

                response.ValoresCorrentes = response.ValoresCorrentes.OrderBy(v => v.TipoDeConta).ThenBy(v => v.Codigo).ToList();
                response.ComponenteOrcamentoRegisto = new ComponenteOrcamentoRegistoDataContract
                {
                    Id = orcamentoRetificado.Id,
                    DataInicio = orcamentoRetificado.DataInicio,
                    DataFim = orcamentoRetificado.DataFim,
                    TarefaActivoFk = orcamentoRetificado.TarefaActivoFk,
                    Aprovado = orcamentoRetificado.Aprovado
                };
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
                return response;
            }

            return response;
        }

        public ResponseBaseDataContract AprovarOrcamento(GetComponenteOrcamentoRegistoAprovadoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool retificacao = false;

            Tarefaativo tarefa = _unitOfWork.TarefaAtivoRepository.Get(request.IdTarefaActivo);
            ComponenteorcamentoRegisto componente = null;

            if (tarefa != null)
            {
                componente = _unitOfWork.ComponenteOrcamentoRegistoRepository.GetByIdProcessoActivo(tarefa.ProcessoAtivoFk);
            }

            List<ComponenteorcamentoRegisto> orcamentosAprovados = null;
            if (componente != null)
            {
                orcamentosAprovados = _unitOfWork.ComponenteOrcamentoRegistoRepository.GetComponentesOrcamentoRegistoAprovado(componente.DataInicio, componente.DataFim);
            }

            if (orcamentosAprovados != null && orcamentosAprovados.Count > 0)
            {
                if (orcamentosAprovados.Count > 1)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.VariosOrcamentoAprovados).ToString(),
                        ErrorMessage = ErrorsDataContract.VariosOrcamentoAprovados.ToString()
                    });
                    return response;
                }
                else
                {
                    if (orcamentosAprovados[0].Id == componente.OrcamentoRetificadoFk)
                    {
                        retificacao = true;
                    }
                    else
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.ExisteOrcamentoAprovado).ToString(),
                            ErrorMessage = ErrorsDataContract.ExisteOrcamentoAprovado.ToString()
                        });
                        return response;
                    }
                }
            }

            try
            {
                ComponenteOrcamentoRegistoDto dto;

                if (retificacao)
                {
                    ComponenteorcamentoRegisto retificado = _unitOfWork.ComponenteOrcamentoRegistoRepository.Get(componente.OrcamentoRetificadoFk.Value);
                    dto = Utils.MappClassToDto<ComponenteorcamentoRegisto, ComponenteOrcamentoRegistoDto>(retificado);
                    dto = _utils.SetDetailsToEntity(dto);
                    dto.IndActivo = false;
                    retificado = Utils.MappClassFromDto<ComponenteOrcamentoRegistoDto, ComponenteorcamentoRegisto>(dto);
                    _unitOfWork.ComponenteOrcamentoRegistoRepository.Update(retificado);
                }

                dto = Utils.MappClassToDto<ComponenteorcamentoRegisto, ComponenteOrcamentoRegistoDto>(componente);
                dto = _utils.SetDetailsToEntity(dto);
                dto.Aprovado = true;
                componente = Utils.MappClassFromDto<ComponenteOrcamentoRegistoDto, ComponenteorcamentoRegisto>(dto);
                _unitOfWork.ComponenteOrcamentoRegistoRepository.Update(componente);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
                return response;
            }

            return response;
        }

        private Dictionary<string, List<ExcTractOrcamentoValor>> SearchForExtract(OrcamentoExtractRequest request)
        {
            Dictionary<string, List<ExcTractOrcamentoValor>> combinationsFilter = new Dictionary<string, List<ExcTractOrcamentoValor>>();

            ComponenteOrcamentoValorSearch filterSearch = new ComponenteOrcamentoValorSearch()
            {
                Id = request.IdComponenteOrcamentoRegisto,
                CentrosDeCusto = request.FiltrosCentroDeCusto,
                Departamentos = request.FiltrosDepartamento,
                TiposDeConta = request.FiltrosTipoDeConta,
            };

            List<ExcTractOrcamentoValor> queryResponse = _unitOfWork.ComponenteOrcamentoValorRepository.SearchComponenteOrcamentoValorExtraction(filterSearch);

            List<string> filtros = new List<string>();

            int countFiltro = 0;

            if (request.FiltrosDepartamento.Count > 0)
                countFiltro++;

            if (request.FiltrosCentroDeCusto.Count > 0)
                countFiltro++;

            if (request.FiltrosTipoDeConta.Count > 0)
                countFiltro++;

            List<ExcTractOrcamentoValor> listValores;
            StringBuilder builder = new StringBuilder();
            if (countFiltro == 3)
            {
                foreach (int idDepartamento in request.FiltrosDepartamento)
                {
                    foreach (int idCentroDeCusto in request.FiltrosCentroDeCusto)
                    {
                        foreach (int idTipoDeConta in request.FiltrosTipoDeConta)
                        {
                            listValores = queryResponse.Where(v => v.DepartamentoFk == idDepartamento && v.CentroCustoFk == idCentroDeCusto && v.TipoContaFk == idTipoDeConta).ToList();
                            if (listValores.Count > 0)
                            {
                                builder.Append(_localizer["departamento"].Value + ": ");
                                builder.Append(listValores[0].Departamento);
                                builder.Append(" - ");
                                builder.Append(_localizer["centroCusto"].Value + ": ");
                                builder.Append(listValores[0].CentrosCusto);
                                builder.Append(" - ");
                                builder.Append(_localizer["tipoConta"].Value + ": ");
                                builder.Append(listValores[0].TipoConta);
                                combinationsFilter.Add(builder.ToString(), listValores);
                                builder.Clear();
                            }
                        }
                    }
                }
            }
            else if (countFiltro == 2)
            {
                if (request.FiltrosDepartamento.Count == 0)
                {
                    foreach (int idCentroDeCusto in request.FiltrosCentroDeCusto)
                    {
                        foreach (int idTipoDeConta in request.FiltrosTipoDeConta)
                        {
                            listValores = queryResponse.Where(v => v.CentroCustoFk == idCentroDeCusto && v.TipoContaFk == idTipoDeConta).ToList();
                            if (listValores.Count > 0)
                            {
                                builder.Append(_localizer["centroCusto"].Value + ": ");
                                builder.Append(listValores[0].CentrosCusto);
                                builder.Append(" - ");
                                builder.Append(_localizer["tipoConta"].Value + ": ");
                                builder.Append(listValores[0].TipoConta);
                                combinationsFilter.Add(builder.ToString(), listValores);
                                builder.Clear();
                            }
                        }
                    }
                }
                else if (request.FiltrosCentroDeCusto.Count == 0)
                {
                    foreach (int idDepartamento in request.FiltrosDepartamento)
                    {
                        foreach (int idTipoDeConta in request.FiltrosTipoDeConta)
                        {
                            listValores = queryResponse.Where(v => v.DepartamentoFk == idDepartamento && v.TipoContaFk == idTipoDeConta).ToList();
                            if (listValores.Count > 0)
                            {
                                builder.Append(_localizer["departamento"].Value + ": ");
                                builder.Append(listValores[0].Departamento);
                                builder.Append(" - ");
                                builder.Append(_localizer["tipoConta"].Value + ": ");
                                builder.Append(listValores[0].TipoConta);
                                combinationsFilter.Add(builder.ToString(), listValores);
                                builder.Clear();
                            }
                        }
                    }
                }
                else
                {
                    foreach (int idDepartamento in request.FiltrosDepartamento)
                    {
                        foreach (int idCentroDeCusto in request.FiltrosCentroDeCusto)
                        {
                            listValores = queryResponse.Where(v => v.DepartamentoFk == idDepartamento && v.CentroCustoFk == idCentroDeCusto).ToList();
                            if (listValores.Count > 0)
                            {
                                builder.Append(_localizer["departamento"].Value + ": ");
                                builder.Append(listValores[0].Departamento);
                                builder.Append(" - ");
                                builder.Append(_localizer["centroCusto"].Value + ": ");
                                builder.Append(listValores[0].CentrosCusto);
                                combinationsFilter.Add(builder.ToString(), listValores);
                                builder.Clear();
                            }
                        }
                    }
                }
            }
            else if (countFiltro == 1)
            {
                if (request.FiltrosDepartamento.Count > 0)
                {
                    foreach (int idDepartamento in request.FiltrosDepartamento)
                    {
                        listValores = queryResponse.Where(v => v.DepartamentoFk == idDepartamento).ToList();
                        if (listValores.Count > 0)
                            combinationsFilter.Add(_localizer["departamento"].Value + ": " + listValores[0].Departamento, listValores);
                    }
                }
                else if (request.FiltrosCentroDeCusto.Count > 0)
                {
                    foreach (int idCentroDeCusto in request.FiltrosCentroDeCusto)
                    {
                        listValores = queryResponse.Where(v => v.CentroCustoFk == idCentroDeCusto).ToList();
                        if (listValores.Count > 0)
                            combinationsFilter.Add(_localizer["centroCusto"].Value + ": " + listValores[0].CentrosCusto, listValores);
                    }
                }
                else
                {
                    foreach (int idTipoDeConta in request.FiltrosTipoDeConta)
                    {
                        listValores = queryResponse.Where(v => v.TipoContaFk == idTipoDeConta).ToList();
                        if (listValores.Count > 0)
                            combinationsFilter.Add(_localizer["tipoConta"].Value + ": " + listValores[0].TipoConta, listValores);
                    }
                }
            }
            else
            {
                combinationsFilter.Add(_localizer["orcamentoGeralINSS"].Value, queryResponse);
            }
            return combinationsFilter;
        }

        //public OrcamentoExtractToExcelReponse ExtractToExcel(OrcamentoExtractRequest request)
        //{
        //    OrcamentoExtractToExcelReponse response = new OrcamentoExtractToExcelReponse();

        //    Dictionary<string, List<ExcTractOrcamentoValor>> combinationsFilter = SearchForExtract(request);

        //    var styles = Extensions.ServiceExtensions.ExcelDocumentTextStyles.ToDictionary(entry => entry.Key,
        //                                                                                   entry => entry.Value);

        //    // Estilos dos dados da coluna
        //    styles.Add("Level1", new ExcelDocumentTextStyle()
        //    {
        //        Bold = true,
        //        BackgroundColor = "#BFBFBF",
        //        Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
        //        HorizontalAlign = ExcelHorizontalAlignment.Left
        //    });

        //    styles.Add("Level2", new ExcelDocumentTextStyle()
        //    {
        //        BackgroundColor = "#CFCFCF",
        //        Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
        //        HorizontalAlign = ExcelHorizontalAlignment.Left

        //    });

        //    styles.Add("Level3", new ExcelDocumentTextStyle()
        //    {
        //        BackgroundColor = "#DFDFDF",
        //        Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
        //        HorizontalAlign = ExcelHorizontalAlignment.Left
        //    });

        //    styles.Add("Level4", new ExcelDocumentTextStyle()
        //    {
        //        BackgroundColor = "#EFEFEF",
        //        Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
        //        HorizontalAlign = ExcelHorizontalAlignment.Left
        //    });

        //    styles.Add("Level1Valor", new ExcelDocumentTextStyle()
        //    {
        //        NumberFormat = "$ #,##0.00",
        //        Bold = true,
        //        BackgroundColor = "#BFBFBF",
        //        Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
        //        HorizontalAlign = ExcelHorizontalAlignment.Left

        //    });

        //    styles.Add("Level2Valor", new ExcelDocumentTextStyle()
        //    {
        //        NumberFormat = "$ #,##0.00",
        //        BackgroundColor = "#CFCFCF",
        //        Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
        //        HorizontalAlign = ExcelHorizontalAlignment.Left
        //    });

        //    styles.Add("Level3Valor", new ExcelDocumentTextStyle()
        //    {
        //        NumberFormat = "$ #,##0.00",
        //        BackgroundColor = "#DFDFDF",
        //        Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
        //        HorizontalAlign = ExcelHorizontalAlignment.Left
        //    });

        //    styles.Add("Level4Valor", new ExcelDocumentTextStyle()
        //    {
        //        NumberFormat = "$ #,##0.00",
        //        BackgroundColor = "#EFEFEF",
        //        Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
        //        HorizontalAlign = ExcelHorizontalAlignment.Left
        //    });

        //    // Inicialização do documento excel
        //    var excelDocument = new ExcelDocument(_localizer["filtro"].Value + " 1", new ExcelDocumentOptions()
        //    {
        //        TextStyles = styles
        //    });

        //    // Adição dos título
        //    ComponenteorcamentoRegisto orcamento = _unitOfWork.ComponenteOrcamentoRegistoRepository.Get(request.IdComponenteOrcamentoRegisto);
        //    string header = "";
        //    if (orcamento.Aprovado)
        //        header = string.Format(_localizer["ocamentoAprovadoPeriodo"].Value, orcamento.DataInicio.ToString("dd/MM/yyyy"), orcamento.DataFim.ToString("dd/MM/yyyy")); // "Orçamento Aprovado para o período de " + orcamento.DataInicio.ToString("dd/MM/yyyy") + " a " + orcamento.DataFim.ToString("dd/MM/yyyy");
        //    else
        //        header = string.Format(_localizer["propostaOrcamentoPeriodo"].Value, orcamento.DataInicio.ToString("dd/MM/yyyy"), orcamento.DataFim.ToString("dd/MM/yyyy"));  // "Proposta de Orçamento para o período de " + orcamento.DataInicio.ToString("dd/MM/yyyy") + " a " + orcamento.DataFim.ToString("dd/MM/yyyy");

        //    var page = 0;
        //    foreach (string combination in combinationsFilter.Keys)
        //    {
        //        if (page != 0) excelDocument.AddPage(_localizer["filtro"].Value + " " + (page + 1));

        //        // Adição dos títulos dos filtros
        //        excelDocument.Pages[page].AddText(header, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(10, 1));

        //        var filters = combination.Split('-');

        //        var row = 2;

        //        for (int i = 0; i < filters.Length; i++)
        //        {
        //            excelDocument.Pages[page].AddText(filters[i], new ExcelDocumentTextPosition(1, row), "Header", new ExcelDocumentTextPosition(10, row));
        //            row++;
        //        }

        //        // Retorna o o estilo do texto consoante o objeto
        //        static string dataTextStyleKeyFunc(ExcTractOrcamentoValor data) => string.IsNullOrWhiteSpace(data.SubAgrupamento) ? "Level1" : string.IsNullOrWhiteSpace(data.Rubrica) ? "Level2" : string.IsNullOrWhiteSpace(data.Alinea) ? "Level3" : "Level4";
        //        static string dataTextStyleKeyFuncValor(ExcTractOrcamentoValor data) => string.IsNullOrWhiteSpace(data.SubAgrupamento) ? "Level1Valor" : string.IsNullOrWhiteSpace(data.Rubrica) ? "Level2Valor" : string.IsNullOrWhiteSpace(data.Alinea) ? "Level3Valor" : "Level4Valor";

        //        // Adição da tabela
        //        excelDocument.Pages[page].AddTable(new ExcelDocumentTextPosition(1, row), combinationsFilter[combination], new List<ColumnOption<ExcTractOrcamentoValor>>()
        //        {
        //            new ColumnOption<ExcTractOrcamentoValor>()
        //            {
        //                Name = _localizer["tipoConta"].Value,
        //                ColumnTextStyleKey = "TableColumn",
        //                DataTextStyleKeyFunc = dataTextStyleKeyFunc,
        //                Value = (data) => data.TipoConta
        //            },
        //            new ColumnOption<ExcTractOrcamentoValor>()
        //            {
        //                Name = _localizer["departamento"].Value,
        //                ColumnTextStyleKey = "TableColumn",
        //                DataTextStyleKeyFunc = dataTextStyleKeyFunc,
        //                Value = (data) => data.Departamento
        //            },
        //            new ColumnOption<ExcTractOrcamentoValor>()
        //            {
        //                Name = _localizer["centroCusto"].Value,
        //                ColumnTextStyleKey = "TableColumn",
        //                DataTextStyleKeyFunc = dataTextStyleKeyFunc,
        //                Value = (data) => data.CentrosCusto
        //            },
        //            new ColumnOption<ExcTractOrcamentoValor>()
        //            {
        //                Name = _localizer["agrupamento"].Value,
        //                ColumnTextStyleKey = "TableColumn",
        //                DataTextStyleKeyFunc = dataTextStyleKeyFunc,
        //                Value = (data) => data.Agrupamento
        //            },
        //            new ColumnOption<ExcTractOrcamentoValor>()
        //            {
        //                Name = _localizer["subAgrupamento"].Value,
        //                ColumnTextStyleKey = "TableColumn",
        //                DataTextStyleKeyFunc = dataTextStyleKeyFunc,
        //                Value = (data) => data.SubAgrupamento
        //            },
        //            new ColumnOption<ExcTractOrcamentoValor>()
        //            {
        //                Name = _localizer["rubrica"].Value,
        //                ColumnTextStyleKey = "TableColumn",
        //                DataTextStyleKeyFunc = dataTextStyleKeyFunc,
        //                Value = (data) => data.Rubrica
        //            },
        //            new ColumnOption<ExcTractOrcamentoValor>()
        //            {
        //                Name = _localizer["alinea"].Value,
        //                ColumnTextStyleKey = "TableColumn",
        //                DataTextStyleKeyFunc = dataTextStyleKeyFunc,
        //                Value = (data) => data.Alinea
        //            },
        //            new ColumnOption<ExcTractOrcamentoValor>()
        //            {
        //                Name = _localizer["subAlinea"].Value,
        //                ColumnTextStyleKey = "TableColumn",
        //                DataTextStyleKeyFunc = dataTextStyleKeyFunc,
        //                Value = (data) => data.SubAlinea
        //            },
        //            new ColumnOption<ExcTractOrcamentoValor>()
        //            {
        //                Name = _localizer["designacao"].Value,
        //                ColumnTextStyleKey = "TableColumn",
        //                DataTextStyleKeyFunc = dataTextStyleKeyFunc,
        //                Value = (data) => data.Designacao
        //            },
        //            new ColumnOption<ExcTractOrcamentoValor>()
        //            {
        //                Name = _localizer["valor"].Value,
        //                ColumnTextStyleKey = "TableColumn",
        //                DataTextStyleKeyFunc = dataTextStyleKeyFuncValor,
        //                Value = (data) => data.Valor
        //            },
        //        });

        //        page++;
        //    }

        //    response.ExcelExtraido = excelDocument.GetFileString();

        //    return response;
        //}

        public OrcamentoExtractToExcelReponse ExtractToExcel(OrcamentoExtractRequest request)
        {
            var response = new OrcamentoExtractToExcelReponse();

            // Lấy dữ liệu
            var combinationsFilter = SearchForExtract(request);

            // Header theo trạng thái
            var orcamento = _unitOfWork.ComponenteOrcamentoRegistoRepository.Get(request.IdComponenteOrcamentoRegisto);
            string header = orcamento.Aprovado
                ? string.Format(_localizer["ocamentoAprovadoPeriodo"].Value, orcamento.DataInicio.ToString("dd/MM/yyyy"), orcamento.DataFim.ToString("dd/MM/yyyy"))
                : string.Format(_localizer["propostaOrcamentoPeriodo"].Value, orcamento.DataInicio.ToString("dd/MM/yyyy"), orcamento.DataFim.ToString("dd/MM/yyyy"));

            using var wb = new XLWorkbook();

            int page = 0;
            foreach (var kv in combinationsFilter)
            {
                var combination = kv.Key;
                var rows = kv.Value;

                string sheetName = page == 0
                    ? _localizer["filtro"].Value + " 1"
                    : _localizer["filtro"].Value + $" {page + 1}";

                var ws = wb.Worksheets.Add(sheetName);

                // Header (A1:J1 merge)
                ws.Range("A1:J1").Merge();
                var h = ws.Cell(1, 1);
                h.Value = header;
                h.Style.Font.Bold = true;
                h.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                h.Style.Fill.BackgroundColor = XLColor.FromHtml("#BFBFBF");
                BorderAll(ws.Range("A1:J1"));

                // In các bộ lọc (bắt đầu từ dòng 2)
                var filters = combination.Split('-');
                int rowIdx = 2;
                foreach (var f in filters)
                {
                    ws.Range(rowIdx, 1, rowIdx, 10).Merge();
                    var c = ws.Cell(rowIdx, 1);
                    c.Value = f;
                    c.Style.Font.Bold = true;
                    c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    c.Style.Fill.BackgroundColor = XLColor.FromHtml("#EFEFEF");
                    BorderAll(ws.Range(rowIdx, 1, rowIdx, 10));
                    rowIdx++;
                }

                // Header bảng
                var headers = new[]
                {
            _localizer["tipoConta"].Value,
            _localizer["departamento"].Value,
            _localizer["centroCusto"].Value,
            _localizer["agrupamento"].Value,
            _localizer["subAgrupamento"].Value,
            _localizer["rubrica"].Value,
            _localizer["alinea"].Value,
            _localizer["subAlinea"].Value,
            _localizer["designacao"].Value,
            _localizer["valor"].Value
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    var c = ws.Cell(rowIdx, i + 1);
                    c.Value = headers[i];
                    c.Style.Font.Bold = true;
                    c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    c.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
                    BorderAll(c);
                }
                rowIdx++;

                // Helpers style theo Level
                string LevelKey(ExcTractOrcamentoValor d)
                    => string.IsNullOrWhiteSpace(d.SubAgrupamento) ? "L1"
                     : string.IsNullOrWhiteSpace(d.Rubrica) ? "L2"
                     : string.IsNullOrWhiteSpace(d.Alinea) ? "L3"
                     : "L4";

                XLColor Bg(string key) => key switch
                {
                    "L1" => XLColor.FromHtml("#BFBFBF"),
                    "L2" => XLColor.FromHtml("#CFCFCF"),
                    "L3" => XLColor.FromHtml("#DFDFDF"),
                    _ => XLColor.FromHtml("#EFEFEF")
                };
                bool IsBold(string key) => key == "L1";

                // Dữ liệu
                foreach (var d in rows)
                {
                    var key = LevelKey(d);
                    var bg = Bg(key);
                    bool bold = IsBold(key);

                    ws.Cell(rowIdx, 1).Value = d.TipoConta;
                    ws.Cell(rowIdx, 2).Value = d.Departamento;
                    ws.Cell(rowIdx, 3).Value = d.CentrosCusto;
                    ws.Cell(rowIdx, 4).Value = d.Agrupamento;
                    ws.Cell(rowIdx, 5).Value = d.SubAgrupamento;
                    ws.Cell(rowIdx, 6).Value = d.Rubrica;
                    ws.Cell(rowIdx, 7).Value = d.Alinea;
                    ws.Cell(rowIdx, 8).Value = d.SubAlinea;
                    ws.Cell(rowIdx, 9).Value = d.Designacao;

                    var valorCell = ws.Cell(rowIdx, 10);
                    valorCell.Value = d.Valor;
                    valorCell.Style.NumberFormat.Format = "$ #,##0.00";

                    // Áp style cho cả dòng
                    for (int col = 1; col <= 10; col++)
                    {
                        var c = ws.Cell(rowIdx, col);
                        c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        c.Style.Fill.BackgroundColor = bg;
                        if (bold && col != 10) c.Style.Font.Bold = true; // Level1Valor
                        BorderAll(c);
                    }

                    rowIdx++;
                }

                ws.Columns().AdjustToContents();
                page++;
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            response.ExcelExtraido = Convert.ToBase64String(ms.ToArray());
            return response;

           
        }

        // helpers
        static void BorderAll(IXLRange range)
        {
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        }
        static void BorderAll(IXLCell cell) => BorderAll(cell.AsRange());

        public OrcamentoExtractToPDFReponse ExtractToPDF(OrcamentoExtractRequest request)
        {
            var response = new OrcamentoExtractToPDFReponse();

            // Dữ liệu
            var combinationsFilter = SearchForExtract(request);

            var orcamento = _unitOfWork.ComponenteOrcamentoRegistoRepository
                                       .Get(request.IdComponenteOrcamentoRegisto);

            string headerS = orcamento.Aprovado
                ? string.Format(_localizer["ocamentoAprovadoPeriodo"].Value,
                                orcamento.DataInicio.ToString("dd/MM/yyyy"),
                                orcamento.DataFim.ToString("dd/MM/yyyy"))
                : string.Format(_localizer["propostaOrcamentoPeriodo"].Value,
                                orcamento.DataInicio.ToString("dd/MM/yyyy"),
                                orcamento.DataFim.ToString("dd/MM/yyyy"));

            // Tạo document
            var doc = new Document();
            doc.Info.Title = "Orçamento";
            doc.DefaultPageSetup.PageFormat = PageFormat.A4;
            doc.DefaultPageSetup.Orientation = Orientation.Landscape;
            doc.DefaultPageSetup.TopMargin = Unit.FromCentimeter(2.5);
            doc.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(0.5);
            doc.DefaultPageSetup.RightMargin = Unit.FromCentimeter(0.5);
            doc.DefaultPageSetup.HeaderDistance = Unit.FromCentimeter(0.3);

            foreach (var kv in combinationsFilter)
            {
                var combination = kv.Key;
                var values = kv.Value;

                var sec = doc.AddSection();

                // Header
                var header = sec.Headers.Primary;
                var par = header.AddParagraph(headerS);
                par.Format.Alignment = ParagraphAlignment.Center;
                par.Format.Font.Bold = true;
                par.Format.Font.Color = Hex("#145792");
                par.Format.Font.Size = 10;

                var splitted = combination.Split('-');
                foreach (var s in splitted)
                {
                    par = header.AddParagraph(s);
                    par.Format.Alignment = ParagraphAlignment.Center;
                    par.Format.Font.Bold = true;
                    par.Format.Font.Color = Hex("#1a6aaf");
                    par.Format.Font.Size = 10;
                }

                // Footer (Trang hiện tại / Tổng trang)
                var footer = sec.Footers.Primary;
                par = footer.AddParagraph();
                par.AddPageField();
                par.AddText("/");
                par.AddNumPagesField();
                par.Format.Alignment = ParagraphAlignment.Right;

                // Khoảng cách dưới header
                sec.AddParagraph().Format.Alignment = ParagraphAlignment.Center;

                // Bảng
                var table = new Table { Borders = { Width = 0.5 } };

                // Kích thước cột
                table.AddColumn(Unit.FromCentimeter(3));   // Tipo de Conta
                table.AddColumn(Unit.FromCentimeter(5));   // Departamento
                table.AddColumn(Unit.FromCentimeter(5));   // Centro de Custo
                table.AddColumn(Unit.FromCentimeter(1.5)); // Agrupamento
                table.AddColumn(Unit.FromCentimeter(1.5)); // SubAgrupamento
                table.AddColumn(Unit.FromCentimeter(1.5)); // Rubrica
                table.AddColumn(Unit.FromCentimeter(1.5)); // Alinea
                table.AddColumn(Unit.FromCentimeter(1.5)); // SubAlinea
                table.AddColumn(Unit.FromCentimeter(5));   // Designacao
                table.AddColumn(Unit.FromCentimeter(3));   // Valor

                // Header hàng tiêu đề
                var row = table.AddRow();
                row.HeadingFormat = true;
                row.Shading.Color = Hex("#2A81CC");
                row.Format.Font.Color = Hex("#FFFFFF");
                row.Format.Font.Bold = true;
                row.VerticalAlignment = VerticalAlignment.Center;

                row.Cells[0].AddParagraph(_localizer["tipoConta"].Value);
                row.Cells[1].AddParagraph(_localizer["departamento"].Value);
                row.Cells[2].AddParagraph(_localizer["centroCusto"].Value);
                row.Cells[3].AddParagraph(_localizer["agrupamentoMin"].Value);
                row.Cells[4].AddParagraph(_localizer["subAgrupamentoMin"].Value);
                row.Cells[5].AddParagraph(_localizer["rubricaMin"].Value);
                row.Cells[6].AddParagraph(_localizer["alineaMin"].Value);
                row.Cells[7].AddParagraph(_localizer["subAlineaMin"].Value);
                row.Cells[8].AddParagraph(_localizer["designacao"].Value);
                row.Cells[9].AddParagraph(_localizer["valor"].Value);

                // Dòng dữ liệu
                foreach (var value in values)
                {
                    row = table.AddRow();
                    row.VerticalAlignment = VerticalAlignment.Center;

                    if (!string.IsNullOrEmpty(value.TipoConta)) row.Cells[0].AddParagraph(value.TipoConta);
                    if (!string.IsNullOrEmpty(value.Departamento)) row.Cells[1].AddParagraph(value.Departamento);
                    if (!string.IsNullOrEmpty(value.CentrosCusto)) row.Cells[2].AddParagraph(value.CentrosCusto);
                    if (!string.IsNullOrEmpty(value.Agrupamento)) row.Cells[3].AddParagraph(value.Agrupamento);
                    if (!string.IsNullOrEmpty(value.SubAgrupamento)) row.Cells[4].AddParagraph(value.SubAgrupamento);
                    if (!string.IsNullOrEmpty(value.Rubrica)) row.Cells[5].AddParagraph(value.Rubrica);
                    if (!string.IsNullOrEmpty(value.Alinea)) row.Cells[6].AddParagraph(value.Alinea);
                    if (!string.IsNullOrEmpty(value.SubAlinea)) row.Cells[7].AddParagraph(value.SubAlinea);
                    if (!string.IsNullOrEmpty(value.Designacao)) row.Cells[8].AddParagraph(value.Designacao);

                    var pValor = row.Cells[9].AddParagraph(value.Valor.ToString("#,##0.00"));
                    pValor.Format.Alignment = ParagraphAlignment.Right;

                    // Tô màu theo “level”
                    if (string.IsNullOrWhiteSpace(value.SubAgrupamento))
                    {
                        row.Format.Font.Bold = true;
                        row.Shading.Color = Hex("#BFBFBF");
                    }
                    else if (string.IsNullOrWhiteSpace(value.Rubrica))
                    {
                        row.Shading.Color = Hex("#CFCFCF");
                    }
                    else if (string.IsNullOrWhiteSpace(value.Alinea))
                    {
                        row.Shading.Color = Hex("#DFDFDF");
                    }
                    else if (string.IsNullOrWhiteSpace(value.SubAlinea))
                    {
                        row.Shading.Color = Hex("#EFEFEF");
                    }
                }

                table.Rows.Alignment = RowAlignment.Center;
                sec.Add(table);
            }

            // Render PDF (MigraDocCore.Rendering)
            var renderer = new PdfDocumentRenderer(unicode: true)   // <= chỉ truyền unicode
            {
                Document = doc
            };


            renderer.RenderDocument();

            using var ms = new MemoryStream();
            renderer.PdfDocument.Save(ms, false);
            response.PDFExtraido = Convert.ToBase64String(ms.ToArray());

            return response;
        }

        static Color Hex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return Colors.Black;

            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            var r = Convert.ToByte(hex.Substring(0, 2), 16) / 255.0;
            var g = Convert.ToByte(hex.Substring(2, 2), 16) / 255.0;
            var b = Convert.ToByte(hex.Substring(4, 2), 16) / 255.0;

            // Chuyển RGB sang CMYK tạm (đơn giản)
            double k = 1 - Math.Max(r, Math.Max(g, b));
            double c = (1 - r - k) / (1 - k + 1e-8);
            double m = (1 - g - k) / (1 - k + 1e-8);
            double y = (1 - b - k) / (1 - k + 1e-8);

            return Color.FromCmyk(c, m, y, k);
        }



        //public OrcamentoExtractToPDFReponse ExtractToPDF(OrcamentoExtractRequest request)
        //{
        //    OrcamentoExtractToPDFReponse response = new OrcamentoExtractToPDFReponse();

        //    Dictionary<string, List<ExcTractOrcamentoValor>> combinationsFilter = SearchForExtract(request);

        //    ComponenteorcamentoRegisto orcamento = _unitOfWork.ComponenteOrcamentoRegistoRepository.Get(request.IdComponenteOrcamentoRegisto);
        //    string headerS = "";
        //    if (orcamento.Aprovado)
        //        headerS = string.Format(_localizer["ocamentoAprovadoPeriodo"].Value, orcamento.DataInicio.ToString("dd/MM/yyyy"), orcamento.DataFim.ToString("dd/MM/yyyy"));  // "Orçamento Aprovado para o período de " + orcamento.DataInicio.ToString("dd/MM/yyyy") + " a " + orcamento.DataFim.ToString("dd/MM/yyyy");
        //    else
        //        headerS = string.Format(_localizer["propostaOrcamentoPeriodo"].Value, orcamento.DataInicio.ToString("dd/MM/yyyy"), orcamento.DataFim.ToString("dd/MM/yyyy"));  // "Proposta de Orçamento para o período de " + orcamento.DataInicio.ToString("dd/MM/yyyy") + " a " + orcamento.DataFim.ToString("dd/MM/yyyy");

        //    Document doc = new Document();
        //    List<ExcTractOrcamentoValor> values;
        //    string[] splitted;
        //    Section sec;
        //    Table table;
        //    Column column;
        //    Row row;
        //    Cell cell;
        //    Paragraph par;
        //    HeaderFooter header;
        //    HeaderFooter footer;
        //    doc.DefaultPageSetup.PageFormat = PageFormat.A4;
        //    doc.DefaultPageSetup.Orientation = Orientation.Landscape;
        //    foreach (string combination in combinationsFilter.Keys)
        //    {
        //        values = combinationsFilter[combination];

        //        sec = doc.AddSection();

        //        header = sec.Headers.Primary;

        //        par = header.AddParagraph(headerS);

        //        par.Format.Alignment = ParagraphAlignment.Center;
        //        par.Format.Font.Bold = true;
        //        par.Format.Font.Color = MigraDoc.DocumentObjectModel.Color.Parse("#145792");
        //        par.Format.Font.Size = 14;

        //        splitted = combination.Split('-');
        //        foreach (string s in splitted)
        //        {
        //            par = header.AddParagraph(s);
        //            par.Format.Alignment = ParagraphAlignment.Center;
        //            par.Format.Font.Bold = true;
        //            par.Format.Font.Color = MigraDoc.DocumentObjectModel.Color.Parse("#1a6aaf");
        //            par.Format.Font.Size = 12;
        //        }

        //        footer = sec.Footers.Primary;

        //        par = footer.AddParagraph();
        //        par.AddPageField();
        //        par.AddChar('/');
        //        par.AddNumPagesField();
        //        par.Format.Alignment = ParagraphAlignment.Right;

        //        sec.AddParagraph();
        //        sec.LastParagraph.Format.Alignment = ParagraphAlignment.Center;
        //        sec.PageSetup.TopMargin = "2.5cm";
        //        sec.PageSetup.TopMargin = "2.5cm";
        //        sec.PageSetup.LeftMargin = "0.5cm";
        //        sec.PageSetup.RightMargin = "0.5cm";
        //        sec.PageSetup.HeaderDistance = "0.3cm";

        //        table = new Table();
        //        table.Borders.Width = 0.5;

        //        #region Tamanhos Colunas

        //        //Tipo de Conta
        //        column = table.AddColumn("3cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;

        //        //Departamento
        //        column = table.AddColumn("5cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;

        //        //Centro de Custo
        //        column = table.AddColumn("5cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;

        //        //Agrupamento
        //        column = table.AddColumn("1.5cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;

        //        //SubAgrupamento
        //        column = table.AddColumn("1.5cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;

        //        //Rúbrica
        //        column = table.AddColumn("1.5cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;

        //        //Alínea
        //        column = table.AddColumn("1.5cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;

        //        //SubAlínea
        //        column = table.AddColumn("1.5cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;

        //        //Designação
        //        column = table.AddColumn("5cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;

        //        //Valor
        //        column = table.AddColumn("3cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;

        //        #endregion Tamanhos Colunas

        //        #region Titulos Colunas

        //        row = table.AddRow();
        //        cell = row.Cells[0];
        //        cell.AddParagraph(_localizer["tipoConta"].Value);
        //        cell.Format.Font.Bold = true;

        //        cell = row.Cells[1];
        //        cell.AddParagraph(_localizer["departamento"].Value);
        //        cell.Format.Font.Bold = true;

        //        cell = row.Cells[2];
        //        cell.AddParagraph(_localizer["centroCusto"].Value);

        //        cell = row.Cells[3];
        //        cell.AddParagraph(_localizer["agrupamentoMin"].Value);

        //        cell = row.Cells[4];
        //        cell.AddParagraph(_localizer["subAgrupamentoMin"].Value);

        //        cell = row.Cells[5];
        //        cell.AddParagraph(_localizer["rubricaMin"].Value);

        //        cell = row.Cells[6];
        //        cell.AddParagraph(_localizer["alineaMin"].Value);

        //        cell = row.Cells[7];
        //        cell.AddParagraph(_localizer["subAlineaMin"].Value);

        //        cell = row.Cells[8];
        //        cell.AddParagraph(_localizer["designacao"].Value);

        //        cell = row.Cells[9];
        //        cell.AddParagraph(_localizer["valor"].Value);
        //        row.Format.Font.Bold = true;

        //        row.Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse("#2A81CC");
        //        row.Format.Font.Color = MigraDoc.DocumentObjectModel.Color.Parse("#FFFFFF");
        //        row.HeadingFormat = true;

        //        #endregion Titulos Colunas

        //        #region Valores Colunas

        //        foreach (ExcTractOrcamentoValor value in values)
        //        {
        //            row = table.AddRow();
        //            row.VerticalAlignment = VerticalAlignment.Center;
        //            if (!string.IsNullOrEmpty(value.TipoConta))
        //            {
        //                cell = row.Cells[0];
        //                cell.AddParagraph(value.TipoConta);
        //            }

        //            if (!string.IsNullOrEmpty(value.Departamento))
        //            {
        //                cell = row.Cells[1];
        //                cell.AddParagraph(value.Departamento);
        //            }

        //            if (!string.IsNullOrEmpty(value.CentrosCusto))
        //            {
        //                cell = row.Cells[2];
        //                cell.AddParagraph(value.CentrosCusto);
        //            }

        //            if (!string.IsNullOrEmpty(value.Agrupamento))
        //            {
        //                cell = row.Cells[3];
        //                cell.AddParagraph(value.Agrupamento);
        //            }

        //            if (!string.IsNullOrEmpty(value.SubAgrupamento))
        //            {
        //                cell = row.Cells[4];
        //                cell.AddParagraph(value.SubAgrupamento);
        //            }

        //            if (!string.IsNullOrEmpty(value.Rubrica))
        //            {
        //                cell = row.Cells[5];
        //                cell.AddParagraph(value.Rubrica);
        //            }

        //            if (!string.IsNullOrEmpty(value.Alinea))
        //            {
        //                cell = row.Cells[6];
        //                cell.AddParagraph(value.Alinea);
        //            }

        //            if (!string.IsNullOrEmpty(value.SubAlinea))
        //            {
        //                cell = row.Cells[7];
        //                cell.AddParagraph(value.SubAlinea);
        //            }

        //            if (!string.IsNullOrEmpty(value.Designacao))
        //            {
        //                cell = row.Cells[8];
        //                cell.AddParagraph(value.Designacao);
        //            }

        //            cell = row.Cells[9];
        //            cell.AddParagraph(value.Valor.ToString("#,##0.00"));

        //            if (string.IsNullOrWhiteSpace(value.SubAgrupamento))
        //            {
        //                row.Format.Font.Bold = true;
        //                row.Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse("#BFBFBF");
        //            }
        //            else if (string.IsNullOrWhiteSpace(value.Rubrica))
        //            {
        //                row.Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse("#CFCFCF");
        //            }
        //            else if (string.IsNullOrWhiteSpace(value.Alinea))
        //            {
        //                row.Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse("#DFDFDF");
        //            }
        //            else if (string.IsNullOrWhiteSpace(value.SubAlinea))
        //            {
        //                row.Shading.Color = MigraDoc.DocumentObjectModel.Color.Parse("#EFEFEF");
        //            }
        //        }

        //        #endregion Valores Colunas

        //        table.Rows.Alignment = RowAlignment.Center;
        //        doc.LastSection.Add(table);
        //    }

        //    PdfDocumentRenderer docRend = new PdfDocumentRenderer
        //    {
        //        Document = doc
        //    };
        //    docRend.RenderDocument();

        //    MemoryStream stream = new MemoryStream();
        //    docRend.Save(stream, false);

        //    byte[] byteArray = stream.ToArray();
        //    string base64String = Convert.ToBase64String(byteArray);
        //    response.PDFExtraido = base64String;

        //    return response;
        //}

        public GetComponenteOrcamentoAprovadoRegistoReponse GetOrcamentoAprovadoReceitaByIdTarefaActivo(GetComponenteOrcamentoRegistoAprovadoRequest request)
        {
            GetComponenteOrcamentoAprovadoRegistoReponse response = new GetComponenteOrcamentoAprovadoRegistoReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            DateTime dataInicioProcesso;
            Tarefaativo tarefaAtivo = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.IdTarefaActivo);
            if (tarefaAtivo != null)
            {
                response.ProcessoId = tarefaAtivo.ProcessoAtivoFk;

                dataInicioProcesso = tarefaAtivo.ProcessoAtivoFkNavigation.DataCriacao;
                var componente = _unitOfWork.ComponenteOrcamentoRegistoRepository.GetOrcamentoAprovadoByDataPInicioProcesso(dataInicioProcesso);
                if (componente != null)
                {
                    var orcamento = _unitOfWork.OrcamentoConfigRepository.Get(componente.OrcamentoConfigFk);

                    if (orcamento != null)
                    {
                        response.CodigoConta = _unitOfWork.CodigoContaRepository.GetAllActivCodigoContaByOrcamentoConfig(orcamento.Id);
                        response.CentrosCusto = _unitOfWork.CentroCustoRepository.GetAllActiveCentroCustoByOrcamentoRegisto(componente.Id);
                        response.TiposDeConta = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.TIPOCONTA);
                        response.existeOrcamentoAprovado = true;
                    }
                    else
                    {
                        response.existeOrcamentoAprovado = true;
                    }
                    response.IdOrcamentoRegisto = componente.Id;
                }
                else
                {
                    response.existeOrcamentoAprovado = false;
                }
            }
            else
            {
                response.existeOrcamentoAprovado = false;
            }
            return response;
        }

        private ComponenteorcamentoRegisto BuildComponenteOrcamentoRegistoObject(int idTarefaActivo, DateTime dataInicio, DateTime dataFim, int orcamentoConfigId)
        {
            ComponenteOrcamentoRegistoDto entity = new ComponenteOrcamentoRegistoDto
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                TarefaActivoFk = idTarefaActivo,
                OrcamentoConfigFk = orcamentoConfigId,
                IndActivo = true,
                Aprovado = false
            };
            entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<ComponenteOrcamentoRegistoDto, ComponenteorcamentoRegisto>(entity);
        }

        private ComponenteorcamentoRegisto BuildUpdatedComponenteOrcamentoRegistoObject(ComponenteorcamentoRegisto original, DateTime dataInicio, DateTime dataFim, int orcamentoConfigId)
        {
            ComponenteOrcamentoRegistoDto originalDto = Utils.MappClassToDto<ComponenteorcamentoRegisto, ComponenteOrcamentoRegistoDto>(original);

            originalDto.DataInicio = dataInicio;
            originalDto.DataFim = dataFim;
            originalDto.OrcamentoConfigFk = orcamentoConfigId;
            originalDto = _utils.UpdateDetailsToEntity(originalDto);

            return Utils.MappClassFromDto<ComponenteOrcamentoRegistoDto, ComponenteorcamentoRegisto>(originalDto);
        }

        private Componenteorcamentovalor BuildComponenteOrcamentoValorReplicadoObject(Componenteorcamentovalor original)
        {
            ComponenteOrcamentoValorDto entity = new ComponenteOrcamentoValorDto
            {
                AgrupamentoFk = original.AgrupamentoFk,
                CentroCustoFk = original.CentroCustoFk,
                DepartamentoFk = original.DepartamentoFk,
                Valor = original.Valor,
                IndActivo = true
            };
            entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<ComponenteOrcamentoValorDto, Componenteorcamentovalor>(entity);
        }

        private string BuildMoradaString(Morada morada)
        {
            var moradaFinal = !string.IsNullOrEmpty(morada.Rua) ? morada.Rua + "," : "";
            moradaFinal += !string.IsNullOrEmpty(morada.NumPorta) ? morada.NumPorta + " - " : "";

            //Existe uma adição de todas as strings pois para a morada final tem de constar tudo: aldeia, posto, suco etc... caso existam
            if (morada.MoradaAldeiaFkNavigation != null && morada.MoradaAldeiaFkNavigation.Nome != null)
            {
                moradaFinal += morada.MoradaAldeiaFkNavigation.Nome + " ";

                if (morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation != null && morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.Nome != null)
                {
                    moradaFinal += morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.Nome + " ";

                    if (morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation != null && morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.Nome != null)
                    {
                        moradaFinal += morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.Nome + " ";

                        if (morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.PostoAdminMunicipioFkNavigation != null && morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.PostoAdminMunicipioFkNavigation.Nome != null)
                        {
                            moradaFinal += morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.PostoAdminMunicipioFkNavigation.Nome + " ";
                        }
                    }
                }
            }

            if (morada.MoradaPaisFkNavigation != null && morada.MoradaPaisFkNavigation.Nome != null)
            {
                moradaFinal += morada.MoradaPaisFkNavigation.Nome;
            }

            return moradaFinal;
        }
    }
}