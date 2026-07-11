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
    public class CodigoContaTreeDataManager : ICodigoContaTreeDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public CodigoContaTreeDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public CodigoContaTreeResponse GetTreeByOrcamentoConfig(GetCodigoContaTreeRequest request)
        {
            CodigoContaTreeResponse response = new CodigoContaTreeResponse();
            try
            {
                List<Codigoconta> items = _unitOfWork.CodigoContaRepository.GetTreeByOrcamentoConfig(request.OrcamentoConfigFk);
                HashSet<int> parentIds = items.Where(a => a.ParentFk.HasValue).Select(a => a.ParentFk.Value).ToHashSet();

                response.Items = items.Select(a => new CodigoContaTreeItemDataContract
                {
                    Id = a.Id,
                    Codigo = a.Codigo,
                    Designacao = a.Designacao,
                    ParentFk = a.ParentFk,
                    OrcamentoConfigFk = a.OrcamentoConfigFk,
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

        public ResponseBaseDataContract SaveCodigoConta(SaveCodigoContaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Codigoconta entity = new Codigoconta
                {
                    Id = request.Id,
                    Codigo = request.Codigo,
                    Designacao = request.Designacao,
                    ParentFk = request.ParentFk,
                    OrcamentoConfigFk = request.OrcamentoConfigFk,
                    IndActivo = true
                };

                if (!_unitOfWork.CodigoContaRepository.IsCodeValid(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "CC-DUP-CODE", ErrorMessage = $"Código '{request.Codigo}' já existe neste kỳ ngân sách." });
                    return response;
                }

                if (entity.Id > 0)
                {
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.CodigoContaRepository.Update(entity);
                }
                else
                {
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.CodigoContaRepository.Add(entity);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract DeactivateCodigoConta(DeactivateCodigoContaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Codigoconta entity = _unitOfWork.CodigoContaRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "CC-NOT-FOUND", ErrorMessage = "Không tìm thấy bản ghi." });
                    return response;
                }

                if (_unitOfWork.CodigoContaRepository.CodigoContaHasChilds(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "CC-HAS-CHILDREN", ErrorMessage = "Không thể xoá — vẫn còn mã con đang hoạt động bên dưới." });
                    return response;
                }

                if (_unitOfWork.CodigoContaRepository.IsCodigoContaInUse(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "CC-IN-USE", ErrorMessage = "Không thể xoá — mã này đã được sử dụng trong dữ liệu chi tiêu." });
                    return response;
                }

                entity.IndActivo = false;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.CodigoContaRepository.Update(entity);
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
