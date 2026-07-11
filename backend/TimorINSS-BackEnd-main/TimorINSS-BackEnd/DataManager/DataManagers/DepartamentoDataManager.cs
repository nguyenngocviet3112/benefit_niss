using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class DepartamentoDataManager : IDepartamentoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public DepartamentoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public SelectDescriptionResponse GetAllDepartamentosAtivo()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> selects = _unitOfWork.DepartamentoRepository.GetAllDepartamentosAtivo();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public DepartamentoConfigListResponse GetAllConfig()
        {
            DepartamentoConfigListResponse response = new DepartamentoConfigListResponse();
            try
            {
                Dictionary<long, string> institutionNomes = _unitOfWork.InstitutionRepository.GetAllInstitutionAtivo()
                    .ToDictionary(i => i.id, i => i.nome);

                response.Items = _unitOfWork.DepartamentoRepository.GetAll()
                    .OrderBy(d => d.Nome)
                    .Select(d => new TimorINSSBackEnd.DataContracts.ModelDataContract.DepartamentoConfigDataContract
                    {
                        Id = d.Id,
                        Nome = d.Nome,
                        InstitutionId = d.InstitutionId,
                        InstitutionNome = d.InstitutionId.HasValue && institutionNomes.ContainsKey(d.InstitutionId.Value)
                            ? institutionNomes[d.InstitutionId.Value] : null,
                        IndActivo = d.IndActivo
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public DepartamentoConfigResponse SaveConfig(SaveDepartamentoConfigRequest request)
        {
            DepartamentoConfigResponse response = new DepartamentoConfigResponse { RequestId = request.RequestId };
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nome))
                {
                    response.Errors.Add(new Error { ErrorCode = "DEPT-NOME-REQUIRED", ErrorMessage = "Tên phòng ban không được để trống." });
                    return response;
                }

                Departamento entity;
                if (request.Id > 0)
                {
                    entity = _unitOfWork.DepartamentoRepository.Get(request.Id);
                    if (entity == null)
                    {
                        response.Errors.Add(new Error { ErrorCode = "DEPT-NOT-FOUND", ErrorMessage = "Không tìm thấy phòng ban." });
                        return response;
                    }
                    entity.Nome = request.Nome;
                    entity.InstitutionId = request.InstitutionId;
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.DepartamentoRepository.Update(entity);
                }
                else
                {
                    entity = new Departamento
                    {
                        Nome = request.Nome,
                        InstitutionId = request.InstitutionId,
                        IndActivo = true
                    };
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.DepartamentoRepository.Add(entity);
                }
                _unitOfWork.Commit();

                Departamento saved = _unitOfWork.DepartamentoRepository.Get(entity.Id);
                response.Item = new TimorINSSBackEnd.DataContracts.ModelDataContract.DepartamentoConfigDataContract
                {
                    Id = saved.Id,
                    Nome = saved.Nome,
                    InstitutionId = saved.InstitutionId,
                    IndActivo = saved.IndActivo
                };
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract DeactivateConfig(DeactivateDepartamentoConfigRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Departamento entity = _unitOfWork.DepartamentoRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "DEPT-NOT-FOUND", ErrorMessage = "Không tìm thấy phòng ban." });
                    return response;
                }
                entity.IndActivo = false;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.DepartamentoRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }
    }
}