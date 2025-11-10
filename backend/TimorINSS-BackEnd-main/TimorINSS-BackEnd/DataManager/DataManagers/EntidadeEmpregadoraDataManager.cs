using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class EntidadeEmpregadoraDataManager : IEntidadeEmpregadoraDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public EntidadeEmpregadoraDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public IEnumerable<Entidadeempregadora> GetAll()
        {
            return _unitOfWork.EntidadeEmpregadoraRepository.GetAll();
        }

        public Entidadeempregadora Get(long id)
        {
            return _unitOfWork.EntidadeEmpregadoraRepository.Get(id);
        }

        public EntidadeempregadoraDto GetDto(long id)
        {
            return _unitOfWork.EntidadeEmpregadoraRepository.GetDto(id);
        }

        public void Add(Entidadeempregadora entity)
        {
            _unitOfWork.EntidadeEmpregadoraRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Entidadeempregadora entity)
        {
            _unitOfWork.EntidadeEmpregadoraRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Entidadeempregadora entity)
        {
            _unitOfWork.EntidadeEmpregadoraRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public EntidadeEmpregadoraConsultaResponse GetByIdEntidade(int id)
        {
            bool isSuspenso = _utils.IsSuspenso(id, 0, _unitOfWork);

            EntidadeEmpregadoraConsultaResponse response = _unitOfWork.EntidadeEmpregadoraRepository.GetByIdEntidade(id);

            if (isSuspenso && response != null)
            {
                //Regra de negócio, significa suspenso
                response.SituacInscricao = "S";
            }
            return response;
        }

        public EntidadeEmpregadoraConsultaResponse UpdateEntidade(EntidadeEmpregadoraRequest request)
        {
            var response = new EntidadeEmpregadoraConsultaResponse { RequestId = request.RequestId };

            //validações

            //Build Objects
            Entidadeempregadora entidadeEmpregadora = BuildEntidadeObject(request);

            // Ao fazer o Update da data de inicio de atividade da entidade empregadora, deve-se confirmar se não é inferior à data de inicio
            //de vinculo dos trabalhadores vinculados à entidade
            TrabalhadorListagemRequest requestTrabalhador = new TrabalhadorListagemRequest
            {
                id = request.EntidadeEmpregadora.IdEntidadeEmpreg,
                filter = new SearchFilter
                {
                    index = 0,
                    filterBy = ""
                }
            };

            bool updateEntidade = true;

            var listaRel = _unitOfWork.TrabalhadoresRepository.GetTrabalhadoresByIdEntidadeEmpregadora(requestTrabalhador);

            if (listaRel != null && listaRel.trabalhadores != null && listaRel.trabalhadores.Count > 0)
            {
                foreach (var trabalhador in listaRel.trabalhadores)
                {
                    if (entidadeEmpregadora.DataInicioActiv != null && trabalhador.dtInicioDeVinculo != null
                        && entidadeEmpregadora.DataInicioActiv > trabalhador.dtInicioDeVinculo)
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.InvalidDataInicioAtividade).ToString(),
                            ErrorMessage = ErrorsDataContract.InvalidDataInicioAtividade.ToString()
                        });
                        updateEntidade = false;
                        break;
                    }
                }
            }

            //validar data de início de atividade e data de inicio de trabalhadores no serviço
            /*By rule the date of initial date of activity is
            equal to the initial date with the employees or
            is newer. Though there is no specific period for
            the gap (number of days,months) but we
            believe common logic for the gap need to be
            observed*/

            if (entidadeEmpregadora.DataInicioActiv > entidadeEmpregadora.DataInicioTrabServico)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroDataInicioAtividade).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroDataInicioAtividade.ToString()
                });
                updateEntidade = false;
            }

            //Insert in DB
            try
            {
                if (updateEntidade)
                {
                    _unitOfWork.EntidadeEmpregadoraRepository.Update(entidadeEmpregadora);
                    _unitOfWork.Commit();
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        public ResponseBaseDataContract UpsertEntidade(EntidadeEmpregadoraUpsertRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            if (request.EntidadeEmpregadora.Id.HasValue)
            {
                var entidade = _unitOfWork.EntidadeEmpregadoraRepository.GetByIdEntidade(request.EntidadeEmpregadora.Id.Value);

                if (entidade == null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.ExcepcaoGenerica).ToString(),
                        ErrorMessage = ErrorsDataContract.ExcepcaoGenerica.ToString()
                    });
                    return response;
                }

                _unitOfWork.EntidadeEmpregadoraRepository.Update(request);
            }
            else
            {
                var entityWithTin = _unitOfWork.EntidadeEmpregadoraRepository.GetByTin(request.EntidadeEmpregadora.Tin);

                if (entityWithTin != null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.TinAlreadyExists).ToString(),
                        ErrorMessage = ErrorsDataContract.TinAlreadyExists.ToString()
                    });
                    return response;
                }

                var entityWithNiss = _unitOfWork.EntidadeEmpregadoraRepository.GetByNiss(request.EntidadeEmpregadora.Niss);

                if (entityWithNiss != null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.NissAlreadyExists).ToString(),
                        ErrorMessage = ErrorsDataContract.NissAlreadyExists.ToString()
                    });
                    return response;
                }

                _unitOfWork.EntidadeEmpregadoraRepository.Create(request);
            }

            //Insert in DB
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

        private Entidadeempregadora BuildEntidadeObject(EntidadeEmpregadoraRequest request)
        {
            EntidadeempregadoraDto entidadeEmpregadora = new EntidadeempregadoraDto
            {
                IdEntidadeEmpreg = request.EntidadeEmpregadora.IdEntidadeEmpreg,
                Nome = request.EntidadeEmpregadora.Nome,
                Niss = request.EntidadeEmpregadora.Niss,
                Tin = request.EntidadeEmpregadora.Tin,
                SituacInscricao = "A",
                NumTrabalhador = request.EntidadeEmpregadora.NumTrabalhador,
                DataInicioActiv = request.EntidadeEmpregadora.DataInicioActiv,
                DataInicioTrabServico = request.EntidadeEmpregadora.DataInicioTrabServico,
                EntidadeNatJuridicaFk = request.EntidadeEmpregadora.IdNaturezaJuridica,
                EntidadeActEconomicaFk = request.EntidadeEmpregadora.IdActividadeEconomica,
                EntidadeSectorActFk = request.EntidadeEmpregadora.IdSectorActividade,
                DtInscricao = request.EntidadeEmpregadora.DtInscricao,
                DataFimActiv = request.EntidadeEmpregadora.DataFimActiv,
                FlagImportado = false,
                DataCriacao = DateTime.Now,
                DataAlteracao = DateTime.Now,
                DtHoraUltimoAcesso = DateTime.Now
            };
            entidadeEmpregadora = _utils.SetDetailsToEntity<EntidadeempregadoraDto>(entidadeEmpregadora);
            return Utils.MappClassFromDto<EntidadeempregadoraDto, Entidadeempregadora>(entidadeEmpregadora);
        }

        public EntidadeEmpregadoraDeclaracaoViewResponse GetEntidadeInfoForDeclaracao(EntidadeEmpregadoraIdRequest request)
        {
            EntidadeEmpregadoraDeclaracaoViewResponse response = new EntidadeEmpregadoraDeclaracaoViewResponse();

            if (request.IdEntidade < 0)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(),
                    ErrorMessage = ErrorsDataContract.InvalidPermission.ToString()
                });
                return response;
            }

            response = _unitOfWork.EntidadeEmpregadoraRepository.GetEntidadeInfoForDeclaracao(request);

            DateTime? firstContrato = _unitOfWork.RelEntidadeTrabalhadorRepository.GetFirstContratoDateByEntidade(request.IdEntidade);

            if (firstContrato.HasValue)
                if (response.dataInicioDeclaracao < firstContrato)
                    response.dataInicioDeclaracao = firstContrato.Value;

            return response;
        }

        public EntidadeEmpregadoraConsultaResponse GetEntidadeByNiss(EntidadeEmpregadoraNissRequest request)
        {
            EntidadeEmpregadoraConsultaResponse response = new EntidadeEmpregadoraConsultaResponse();

            if (request.Niss.Length == 0)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(),
                    ErrorMessage = ErrorsDataContract.InvalidPermission.ToString()
                });
                return response;
            }

            response = _unitOfWork.EntidadeEmpregadoraRepository.GetEntidadeByNiss(request.Niss);

            if (response == null)
            {
                response = new EntidadeEmpregadoraConsultaResponse();
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString()
                });
                return response;
            }

            return response;
        }
    }
}