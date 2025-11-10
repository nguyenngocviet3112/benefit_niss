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
    public class ContaCorrenteDataManager : IContaCorrenteDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ContaCorrenteDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public IEnumerable<Contacorrente> GetAll()
        {
            return _unitOfWork.ContaCorrenteRepository.GetAll();
        }

        public Contacorrente Get(long id)
        {
            return _unitOfWork.ContaCorrenteRepository.Get(id);
        }

        public ContacorrenteDto GetDto(long id)
        {
            return _unitOfWork.ContaCorrenteRepository.GetDto(id);
        }

        public void Add(Contacorrente entity)
        {
            _unitOfWork.ContaCorrenteRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Contacorrente entity)
        {
            _unitOfWork.ContaCorrenteRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Contacorrente entity)
        {
            _unitOfWork.ContaCorrenteRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public ContaCorrenteListagemResponse GetContaCorrenteByIdEntidade(ContaCorrenteListagemRequest request)
        {
            // vai buscar a conta corrente de uma determinada entidade empregadora
            ContaCorrenteListagemResponse response = new ContaCorrenteListagemResponse();
            try
            {
                if (request.filter != null)
                {
                    bool valid = false;
                    var userId = request.UserId;
                    var user = _unitOfWork.UtilizadoresRepository.Get(userId);
                    var entidadeEmpregadoraId = user.UtilizadorEntidadeFk;
                    decimal valorPago = 0;

                    Guiapagamento GuiaUltimoGuiaPagamento = new Guiapagamento();

                    Dictionary<int?, int> listaCompostas = new Dictionary<int?, int>();

                    // Validar se o utilizador tem as permissões necessárias
                    if (entidadeEmpregadoraId.HasValue || user.Interno == true)
                        valid = true;

                    if (valid)
                    {
                        request.filter.filterField = "ENTIDADEEMPREGADORA";
                        response = _unitOfWork.ContaCorrenteRepository.GetContaCorrenteByFilter(request);

                    }
                    else
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(),
                            ErrorMessage = ErrorsDataContract.InvalidPermission.ToString()
                        });
                    }
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

        public ResumoContaCorrenteListagemResponse GetResumoContaCorrenteByIdEntidade(ResumoContaCorrenteListagemRequest request)
        {
            // vai buscar a conta corrente de uma determinada entidade empregadora
            ResumoContaCorrenteListagemResponse response = new ResumoContaCorrenteListagemResponse();
            try
            {
                var userId = request.UserId;
                var user = _unitOfWork.UtilizadoresRepository.Get(userId);
                var entidadeEmpregadoraId = user.UtilizadorEntidadeFk;

                // Validar se o utilizador tem as permissões necessárias
                if (!entidadeEmpregadoraId.HasValue && user.Interno != true)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(),
                        ErrorMessage = ErrorsDataContract.InvalidPermission.ToString()
                    });
                    return response;
                }

                response = _unitOfWork.ContaCorrenteRepository.GetResumoContaCorrente(request);


            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public GetAllContasStatesFromYearByFilterResponse GetAllContasStatesFromYearByFilter(GetAllContasStatesFromYearByFilterRequest input)
        {
            GetAllContasStatesFromYearByFilterRequest request = input;
            GetAllContasStatesFromYearByFilterResponse response = new GetAllContasStatesFromYearByFilterResponse();
            if (request.filter == null)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                });
            else if (!request.filter.dateFilterBegin.HasValue)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DateDoesNotExists).ToString(),
                    ErrorMessage = ErrorsDataContract.DateDoesNotExists.ToString()
                });
            else
            {
                const int SECRET_A = 654321;
                const int SECRET_B = 123456789;
                int decodedId = (request.idEntidade - SECRET_B) / SECRET_A;
                DateTime dateFinal;
                DateTime dateInicial = _unitOfWork.EntidadeEmpregadoraRepository.GetByIdEntidade(decodedId).DataInicioActiv;
                DateTime now = DateTime.Now;

                if (request.filter.dateFilterBegin.Value.Year == now.Year)
                {
                    List<DominioDescricaoString> declarationDayList = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.DIADECLRACAO);

                    if (declarationDayList.Count() < 1)
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.DiaDeclaracaoNotFound).ToString(),
                            ErrorMessage = ErrorsDataContract.DiaDeclaracaoNotFound.ToString()
                        });
                        return response;
                    }

                    long declarationDay = declarationDayList[0].value;
                    if (now.Day <= declarationDay)
                        dateFinal = now.AddMonths(-1);
                    else
                        dateFinal = now;

                    if (dateInicial.Year < now.Year)
                        dateInicial = new DateTime(now.Year, 1, 1);
                }
                else
                {
                    dateFinal = new DateTime(request.filter.dateFilterBegin.Value.Year + 1, 1, 1).AddDays(-1);
                    if (dateInicial.Year < dateFinal.Year)
                        dateInicial = new DateTime(dateFinal.Year, 1, 1);
                }

                if (dateInicial > dateFinal)
                {
                    response.contasState = new List<ContasState>();
                    return response;
                }
                else
                {
                    List<ContasState> res = _unitOfWork.ContaCorrenteRepository.GetAllContasStatesFromYearByIdEntidade(request.idEntidade, dateInicial, dateFinal);

                    //List<ContasState> res = new List<ContasState>();

                    //fill blanks (for current and error state)
                    ContasState contaIterator;
                    ContasState emptyConta;
                    Dominio guiaNaoGeradaDominio = _unitOfWork.DominioRepository.getDominioByDescricao(TiposDominio.SITUACAOPAGAMENTO, "Sem Guia Gerada");
                    for (int i = dateInicial.Month; i <= dateFinal.Month; i++)
                    {
                        contaIterator = res.FirstOrDefault(c => c.month == i);
                        if (contaIterator == null)
                        {
                            emptyConta = new ContasState
                            {
                                id = 0,
                                month = i,
                                state = guiaNaoGeradaDominio.Descricao,
                                stateId = guiaNaoGeradaDominio.Valor
                            };
                            res.Add(emptyConta);
                        }
                    }

                    res = res.OrderBy(c => c.month).ToList();
                    response.contasState = res;
                }
            }

            return response;
        }
    }
}