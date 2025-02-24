using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
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
    public class MovimentosPorConciliarDataManager : IMovimentosPorConciliarDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration Configuration;
        private readonly IEmailSenderDataManager _emailSenderDataManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUtilsDataManager _utils;

        public MovimentosPorConciliarDataManager(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailSenderDataManager emailSenderDataManager, IHttpContextAccessor httpContextAccessor, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            Configuration = configuration;
            _emailSenderDataManager = emailSenderDataManager;
            _httpContextAccessor = httpContextAccessor;
            _utils = utils;
        }

        public IEnumerable<Movimentosporconciliar> GetAll()
        {
            return _unitOfWork.MovimentosPorConciliarRepository.GetAll();
        }

        public Movimentosporconciliar Get(long id)
        {
            return _unitOfWork.MovimentosPorConciliarRepository.Get(id);
        }

        public MovimentosPorConciliarDto GetDto(long id)
        {
            return _unitOfWork.MovimentosPorConciliarRepository.GetDto(id);
        }

        public void Add(Movimentosporconciliar entity)
        {
            entity.IndActivo = true;
            //entity.UtilizadorCriacao = 2;
            entity.DataCriacao = DateTime.Now;
            _unitOfWork.MovimentosPorConciliarRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Movimentosporconciliar entity)
        {
            entity.DataAlteracao = DateTime.Now;
            _unitOfWork.MovimentosPorConciliarRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Movimentosporconciliar entity)
        {
            _unitOfWork.MovimentosPorConciliarRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public MovimentosPorConciliar GetListagem(MovimentosPorConciliarListagemRequest request)
        {
            var list = _unitOfWork.MovimentosPorConciliarRepository.GetListagem(request);
            var filterdValues = _unitOfWork.MovimentosPorConciliarRepository.GetExtraDataByListIds(list.movimentos.Where(x => x.type == MovimentosPorConciliarListagemType.MovimentoAConciliar).Select(y => y.id).ToList());
            list.movimentos.Where(x => x.type == MovimentosPorConciliarListagemType.MovimentoAConciliar).ToList().ForEach(element =>
            {
                element.movimentoBancarioId = filterdValues.FirstOrDefault(x => x.id == element.id).movimentoBancarioId;
                element.tipoDocumento = filterdValues.FirstOrDefault(x => x.id == element.id).tipoDocumento;
                element.nomeComprovativo = filterdValues.FirstOrDefault(x => x.id == element.id).nomeComprovativo;
                element.contabilidadeCredito = filterdValues.FirstOrDefault(x => x.id == element.id).contabilidadeCredito;
                element.contabilidadeDebito = filterdValues.FirstOrDefault(x => x.id == element.id).contabilidadeDebito;
                element.departamentoINSS = filterdValues.FirstOrDefault(x => x.id == element.id).departamentoINSS;
                element.centroCusto = filterdValues.FirstOrDefault(x => x.id == element.id).centroCusto;
                element.tipoConta = filterdValues.FirstOrDefault(x => x.id == element.id).tipoConta;
                element.contaOSS = filterdValues.FirstOrDefault(x => x.id == element.id).contaOSS;
            });
            return list;
        }

        public ResponseBaseDataContract CreateMovimentoPorConciliar(CreateMovimentosPorConciliarRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            var tarefaConfig = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.TarefaAtivoId).TarefaconfigFkNavigation;

            // Validar se a tarefa tem permissões para o componente
            if (tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo)?.PermissaoMovimentosConciliar != 2)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // Se for receita, o comprovativo é obrigatório
            var formIsValid = !request.IsReceita || (request.Comprovativo != null && request.Comprovativo.Length > 0);

            if (!formIsValid)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.ExcepcaoGenerica).ToString(), ErrorMessage = ErrorsDataContract.ExcepcaoGenerica.ToString() });
                return response;
            }

            var movimentoPorConciliar = new Movimentosporconciliar()
            {
                TarefaAtivoFk = request.TarefaAtivoId,
                MovimentoBancarioFk = request.MovimentoBancarioId,
                IsReceita = request.IsReceita,
                Valor = request.Valor,
                TipoDocumento = request.TipoDocumento,
                NumeroDocumento = request.NumeroDocumento,
                Comprovativo = request.Comprovativo != null ? Convert.FromBase64String(request.Comprovativo) : null,
                NomeComprovativo = request.NomeComprovativo,
                CodigoContaDebitoFk = request.ContabilidadeDebito,
                CodigoContaCreditoFk = request.ContabilidadeCredito,
                DepartamentoFk = request.DepartamentoINSS,
                CentroCustoFk = request.CentroCusto,
                TipoContaFk = request.TipoConta,
                AgrupamentoConfigFk = request.ContaOSS
            };

            _utils.SetDetailsToEntity(movimentoPorConciliar);

            //DB
            try
            {
                _unitOfWork.MovimentosPorConciliarRepository.Add(movimentoPorConciliar);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        public ResponseBaseDataContract UpdateMovimentoPorConciliar(UpdateMovimentosPorConciliarRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            var tarefaConfig = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.TarefaAtivoId).TarefaconfigFkNavigation;
            
            // Validar se a tarefa tem permissões para o componente
            if (tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoMovimentosConciliar != 2)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // Se for receita, o comprovativo é obrigatório
            var formIsValid = !request.IsReceita || (request.Comprovativo != null && request.Comprovativo.Length > 0);
            var movimentoPorConciliar = _unitOfWork.MovimentosPorConciliarRepository.Get(request.Id);

            if (!formIsValid || (request.IsGuia != true && request.IsReserva != true && movimentoPorConciliar.RelMovimentosporconciliarMovimentos.Any()))
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.ExcepcaoGenerica).ToString(), ErrorMessage = ErrorsDataContract.ExcepcaoGenerica.ToString() });
                return response;
            }

            bool isNewMovimento = false;
            Movimentosporconciliar movExistente = null;

            if (request.IsGuia == true)
            {
                movExistente = _unitOfWork.MovimentosPorConciliarRepository.GuiaExistenteNosMovimentos(request.Id);
            }
            else if (request.IsReserva == true)
            {
                movExistente = _unitOfWork.MovimentosPorConciliarRepository.ReservaCreditoExistenteNosMovimentos(request.Id);
            }

            if (request.IsGuia == true || request.IsReserva == true)
            {
                isNewMovimento = movExistente == null;

                if (isNewMovimento)
                {
                    movimentoPorConciliar = new Movimentosporconciliar
                    {
                        TarefaAtivoFk = request.TarefaAtivoId,
                        IsReceita = request.IsReceita,
                        Valor = request.Valor,
                        GuiapagamentoId = request.IsGuia == true ? request.Id : (int?)null,
                        ReservaCreditoId = request.IsReserva == true ? request.Id : (int?)null,
                        TipoDocumento = request.IsGuia == true ? "Guia de Pagamento" : "Reserva de Crédito",
                        NumeroDocumento = request.NumeroDocumento,
                        Comprovativo = Convert.FromBase64String(request.Comprovativo),
                        NomeComprovativo = request.IsGuia == true ? "Guia de Pagamento - " + request.NumeroDocumento + ".pdf" : "",
                        IndActivo = true,
                        CodigoContaDebitoFk = request.ContabilidadeDebito,
                        CodigoContaCreditoFk = request.ContabilidadeCredito,

                    };
                    movimentoPorConciliar = _utils.SetDetailsToEntity(movimentoPorConciliar);
                }
                else
                {
                    movimentoPorConciliar = movExistente;
                    movimentoPorConciliar.CodigoContaCreditoFk = request.ContabilidadeCredito;
                    movimentoPorConciliar.CodigoContaDebitoFk = request.ContabilidadeDebito;
                    movimentoPorConciliar = _utils.UpdateDetailsToEntity(movimentoPorConciliar);
                }
            }
            else
            {
                movimentoPorConciliar.MovimentoBancarioFk = request.MovimentoBancarioId;
                movimentoPorConciliar.Valor = request.Valor;
                movimentoPorConciliar.TipoDocumento = request.TipoDocumento;
                movimentoPorConciliar.NumeroDocumento = request.NumeroDocumento;
                movimentoPorConciliar.Comprovativo = Convert.FromBase64String(request.Comprovativo);
                movimentoPorConciliar.NomeComprovativo = request.NomeComprovativo;
                movimentoPorConciliar.CodigoContaCreditoFk = request.ContabilidadeCredito;
                movimentoPorConciliar.CodigoContaDebitoFk = request.ContabilidadeDebito;

                _utils.UpdateDetailsToEntity(movimentoPorConciliar);
            }

            //DB
            try
            {
                if (request.IsGuia == true || request.IsReserva == true)
                {
                    if (isNewMovimento)
                        _unitOfWork.MovimentosPorConciliarRepository.Add(movimentoPorConciliar);
                    else
                        _unitOfWork.MovimentosPorConciliarRepository.Update(movimentoPorConciliar);
                }
                else
                {
                    _unitOfWork.MovimentosPorConciliarRepository.Update(movimentoPorConciliar);
                }
                
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        public ResponseBaseDataContract ConciliarMovimentos(ConciliarMovimentosRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            var tarefaConfig = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.TarefaAtivoId).TarefaconfigFkNavigation;

            // Validar se a tarefa tem permissões para o componente
            if (tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo)?.PermissaoConciliar != 2)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            var movimentosBancariosValores = _unitOfWork.MovimentosbancariosRepository.GetValores(request.MovimentosBancarios);
            var movimentosPorConciliarValores = _unitOfWork.MovimentosPorConciliarRepository.GetValores(request.MovimentosAConciliar);

            var formIsValid = ((request.MovimentosBancarios.Count == 1 && request.MovimentosAConciliar.Count >= 1) || // Se tiver vários movimentos a conciliar, só pode ter um movimento bancário
                              (request.MovimentosAConciliar.Count == 1 && request.MovimentosBancarios.Count >= 1)) && // Se tiver vários movimentos bancários, só pode ter um movimento a conciliar
                              movimentosBancariosValores.Count == request.MovimentosBancarios.Count && // Validar se a quantidade de movimentos bancários no request é igual ao que veio da base de dados
                              (movimentosBancariosValores.Count(e => e > 0) == 0 || movimentosBancariosValores.Count(e => e > 0) == request.MovimentosBancarios.Count) && // Validar se os valores dos movimentos bancários são superiores a 0
                              movimentosPorConciliarValores.Count == request.MovimentosAConciliar.Count && // Validar se a quantidade de movimentos por conciliar no request é igual ao que veio da base de dados
                              (movimentosPorConciliarValores.Count(e => e > 0) == 0 || movimentosPorConciliarValores.Count(e => e > 0) == request.MovimentosAConciliar.Count) && // Validar se os valores dos movimentos por conciliar são superiores a 0
                              movimentosBancariosValores.Select(e => Math.Abs(e)).Sum() - movimentosPorConciliarValores.Select(e => Math.Abs(e)).Sum() == 0 && // Só permite conciliar os movimentos se a diferença dos valores for 0
                              !_unitOfWork.MovimentosPorConciliarRepository.TemConciliados(request.MovimentosAConciliar) && // Valida se o movimento por conciliar do request já tem algum movimento conciliado
                              !_unitOfWork.MovimentosbancariosRepository.TemConciliados(request.MovimentosBancarios); // Valida se o movimento bancário do request já tem algum movimento conciliado

            if (!formIsValid)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidConfiguration).ToString(), ErrorMessage = ErrorsDataContract.InvalidConfiguration.ToString() });
                return response;
            }

            if (request.MovimentosBancarios.Count == 1)
            {
                var movBancarioId = request.MovimentosBancarios.First();

                request.MovimentosAConciliar.ForEach(mov =>
                {

                    var estadoMovimento = _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 1);
                    var rel = _unitOfWork.MovimentosPorConciliarRepository.CreateRelationObject(movBancarioId, mov, estadoMovimento);

                    _utils.SetDetailsToEntity(rel);

                    _unitOfWork.MovimentosPorConciliarRepository.AddRelation(rel);
                });
            }
            else
            {
                var movPorConciliar = request.MovimentosAConciliar.First();

                request.MovimentosBancarios.ForEach(mov =>
                {
                    var estadoMovimento = _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 1);
                    var rel = _unitOfWork.MovimentosPorConciliarRepository.CreateRelationObject(mov, movPorConciliar, estadoMovimento);

                    _utils.SetDetailsToEntity(rel);

                    _unitOfWork.MovimentosPorConciliarRepository.AddRelation(rel);
                });
            }

            var contasCorrente = new List<int>();
            // Check all guias de pagamento and change indpago
            if (request.MovimentosAConciliar.Any(x => x.Type == MovimentosPorConciliarListagemType.GuiaPagamento))
            {
                var pagoStates = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.INDPAGO);
                var situacaoPagamentos = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.SITUACAOPAGAMENTO);
                var guiasPagamento = _unitOfWork.GuiaPagamentoRepository.GetGuiasByIds(request.MovimentosAConciliar.Where(x => x.Type == MovimentosPorConciliarListagemType.GuiaPagamento).Select(y => y.Id).ToList());
                foreach (var guia in guiasPagamento)
                {
                    if (guia.IndPago == pagoStates.FirstOrDefault(x => x.descricao == "Comprovativo em Validação").id)
                    {
                        guia.IndPago = (int)pagoStates.FirstOrDefault(x => x.descricao == "Guia Paga").id;
                    }
                    else
                    {
                        guia.IndPago = (int)pagoStates.FirstOrDefault(x => x.descricao == "Guia Parcialmente Paga").id;
                    }

                    if (!contasCorrente.Contains(guia.ContaCorrenteId)) contasCorrente.Add(guia.ContaCorrenteId);

                    _utils.UpdateDetailsToEntity(guia);
                    _unitOfWork.GuiaPagamentoRepository.Update(guia);
                }
            }

            // Check all conciliated reservas de crédito and enable them
            if (request.MovimentosAConciliar.Any(x => x.Type == MovimentosPorConciliarListagemType.ReservaCredito))
            {
                var reservasCredito = _unitOfWork.ReservaCreditoRepository.GetByIds(request.MovimentosAConciliar.Where(x => x.Type == MovimentosPorConciliarListagemType.ReservaCredito).Select(y => y.Id).ToList());
                foreach (var reserva in reservasCredito)
                {
                    reserva.IndActivo = true;
                    _utils.UpdateDetailsToEntity(reserva);
                    _unitOfWork.ReservaCreditoRepository.Update(reserva);
                }
            }

            //DB
            try
            {
                _unitOfWork.Commit();
                contasCorrente.ForEach(e => _unitOfWork.ContaCorrenteRepository.UpdateSituacaoPagamento(e));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        public ResponseBaseDataContract DesfazerConciliacao(DesfazerConciliacaoMovimentosRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            var tarefaConfig = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.TarefaAtivoId).TarefaconfigFkNavigation;

            // Validar se a tarefa tem permissões para o componente
            if (tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo)?.PermissaoDesfazerConciliar != 2)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // Movimentos bancarios ou Movimentos por conciliar, relacionados com o Id enviado no request
            var movimentosRel = request.Type == null ? _unitOfWork.RelMovimentosPorConciliarMovimentosRepository.GetAllFromMovimentoBancario(request.Id).ToList() :
                                                       _unitOfWork.RelMovimentosPorConciliarMovimentosRepository.GetAllFromMovimentosPorConciliar(request.Id, request.Type.Value).ToList();

            var estado = _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 2);

            if (!movimentosRel.Any() || movimentosRel.Any(e => e.IndActivo.Value && e.Estado == estado))
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidConfiguration).ToString(), ErrorMessage = ErrorsDataContract.InvalidConfiguration.ToString() });
                return response;
            }

            movimentosRel.ForEach(mov =>
            {
                _utils.UpdateDetailsToEntity(mov);
                mov.IndActivo = false;
            });

            //DB
            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        public MovimentosPorConciliar GetListagemConciliacao(MovimentosPorConciliarConciliacaoListagemRequest request)
        {
            var response = new MovimentosPorConciliar { RequestId = request.RequestId };

            if (request.ConciliadoCom.HasValue)
            {
                var movimentoBancario = _unitOfWork.MovimentosbancariosRepository.Get(request.ConciliadoCom.Value);

                if (movimentoBancario == null || !movimentoBancario.RelMovimentosporconciliarMovimentos.Any())
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidConfiguration).ToString(), ErrorMessage = ErrorsDataContract.InvalidConfiguration.ToString() });
                    return response;
                }
            }

            return _unitOfWork.MovimentosPorConciliarRepository.GetListagemConciliacao(request, _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 2));
        }

        public MovimentosPorConciliar GetMovimentosConciliados(SearchFilterRequest request)
        {
            MovimentosPorConciliar response = new MovimentosPorConciliar();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações
            try
            {
                if (request.filter != null)
                {
                    response = _unitOfWork.MovimentosPorConciliarRepository.GetMovimentosConciliados(request, _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 1));
                }
                else
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                        ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                    });
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public Movimentosporconciliar GetByGuiaOrReserva(int id, bool isGuia)
        {
            if (isGuia)
            {
                return _unitOfWork.MovimentosPorConciliarRepository.GuiaExistenteNosMovimentos(id);
            }
            return _unitOfWork.MovimentosPorConciliarRepository.ReservaCreditoExistenteNosMovimentos(id);

        }

    }
}