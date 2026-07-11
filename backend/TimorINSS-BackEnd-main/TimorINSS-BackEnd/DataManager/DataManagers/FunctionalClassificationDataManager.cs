using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class FunctionalClassificationDataManager : IFunctionalClassificationDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public FunctionalClassificationDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public FunctionalClassificationTreeResponse GetAllActive()
        {
            FunctionalClassificationTreeResponse response = new FunctionalClassificationTreeResponse();
            try
            {
                List<FunctionalClassification> items = _unitOfWork.FunctionalClassificationRepository.GetAllActive();
                HashSet<int> parentIds = items.Where(a => a.ParentFk.HasValue).Select(a => a.ParentFk.Value).ToHashSet();

                response.Items = items.Select(a => new FunctionalClassificationDataContract
                {
                    Id = a.Id,
                    Codigo = a.Codigo,
                    Designacao = a.Designacao,
                    ParentFk = a.ParentFk,
                    IndActivo = a.IndActivo,
                    HasKids = parentIds.Contains(a.Id)
                }).ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract SaveFunctionalClassification(SaveFunctionalClassificationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                FunctionalClassification entity = new FunctionalClassification
                {
                    Id = request.Id,
                    Codigo = request.Codigo,
                    Designacao = request.Designacao,
                    ParentFk = request.ParentFk,
                    IndActivo = true
                };

                if (!_unitOfWork.FunctionalClassificationRepository.IsCodeValid(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "FC-DUP-CODE", ErrorMessage = $"Código '{request.Codigo}' đã tồn tại." });
                    return response;
                }

                if (entity.Id > 0)
                {
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.FunctionalClassificationRepository.Update(entity);
                }
                else
                {
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.FunctionalClassificationRepository.Add(entity);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract DeactivateFunctionalClassification(DeactivateFunctionalClassificationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                if (_unitOfWork.FunctionalClassificationRepository.HasActiveChildren(request.Id))
                {
                    response.Errors.Add(new Error { ErrorCode = "FC-HAS-CHILDREN", ErrorMessage = "Không thể vô hiệu hoá — vẫn còn mục con đang hoạt động bên dưới." });
                    return response;
                }

                FunctionalClassification entity = _unitOfWork.FunctionalClassificationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "FC-NOT-FOUND", ErrorMessage = "Không tìm thấy bản ghi." });
                    return response;
                }

                entity.IndActivo = false;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.FunctionalClassificationRepository.Update(entity);
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
