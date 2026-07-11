using System;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class LanguageConfigDataManager : ILanguageConfigDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public LanguageConfigDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public LanguageConfigListResponse GetAll()
        {
            LanguageConfigListResponse response = new LanguageConfigListResponse();
            try
            {
                response.Items = _unitOfWork.LanguageConfigRepository.GetAll()
                    .Select(ToDataContract)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public LanguageConfigListResponse GetAllActive()
        {
            LanguageConfigListResponse response = new LanguageConfigListResponse();
            try
            {
                response.Items = _unitOfWork.LanguageConfigRepository.GetAllActive()
                    .Select(ToDataContract)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Toggle(ToggleLanguageConfigRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                LanguageConfig entity = _unitOfWork.LanguageConfigRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "LC-NOT-FOUND", ErrorMessage = "Không tìm thấy ngôn ngữ." });
                    return response;
                }

                if (!request.IndActivo)
                {
                    int activeCount = _unitOfWork.LanguageConfigRepository.GetAllActive().Count;
                    if (activeCount <= 1 && entity.IndActivo)
                    {
                        response.Errors.Add(new Error { ErrorCode = "LC-LAST-ACTIVE", ErrorMessage = "Phải giữ lại ít nhất 1 ngôn ngữ đang bật." });
                        return response;
                    }
                }

                entity.IndActivo = request.IndActivo;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.LanguageConfigRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        private static LanguageConfigDataContract ToDataContract(LanguageConfig entity)
        {
            return new LanguageConfigDataContract
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Ordem = entity.Ordem,
                IndActivo = entity.IndActivo
            };
        }
    }
}
