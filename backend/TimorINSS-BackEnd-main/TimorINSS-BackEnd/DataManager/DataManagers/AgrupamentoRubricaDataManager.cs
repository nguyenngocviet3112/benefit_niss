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
    // Mapeamento Rubricas (mode mới) — quản lý Agrupamentoconfig CHỈ cho 4 TipoConta phục
    // vụ mapping Codigoconta -> dòng Balanço/DR (Receita/Despesa/Neutro Receita/Neutro
    // Despesa). Tách biệt hoàn toàn khỏi AgrupamentoConfigDataManager cũ (dùng cho màn/
    // mục đích khác) — không sửa file đó.
    public class AgrupamentoRubricaDataManager : IAgrupamentoRubricaDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public AgrupamentoRubricaDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public AgrupamentoRubricaTreeResponse GetTreeByOrcamentoConfig(GetAgrupamentoRubricaTreeRequest request)
        {
            AgrupamentoRubricaTreeResponse response = new AgrupamentoRubricaTreeResponse();
            try
            {
                List<Agrupamentoconfig> items = _unitOfWork.AgrupamentoConfigRepository
                    .GetRubricaTreeByOrcamentoConfig(request.OrcamentoConfigFk, request.TipoConta);

                HashSet<int> parentIds = items.Where(a => a.ParentFk.HasValue).Select(a => a.ParentFk.Value).ToHashSet();

                response.Items = items.Select(a => new AgrupamentoRubricaDataContract
                {
                    Id = a.Id,
                    Codigo = a.Codigo,
                    Designacao = a.Designacao,
                    ParentFk = a.ParentFk,
                    OrcamentoConfigFk = request.OrcamentoConfigFk,
                    TipoConta = request.TipoConta,
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

        public ResponseBaseDataContract SaveAgrupamentoRubrica(SaveAgrupamentoRubricaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                int reltipoFk = _unitOfWork.AgrupamentoConfigRepository
                    .GetOrCreateReltipoDeContaOrcamentoConfig(request.OrcamentoConfigFk, request.TipoConta, request.UserId);

                Agrupamentoconfig entity = new Agrupamentoconfig
                {
                    Id = request.Id,
                    Codigo = request.Codigo,
                    Designacao = request.Designacao,
                    ParentFk = request.ParentFk,
                    ReltipoDeContaOrcamentoConfigFk = reltipoFk,
                    IndActivo = true
                };

                if (!_unitOfWork.AgrupamentoConfigRepository.IsRubricaCodeValid(entity, reltipoFk))
                {
                    response.Errors.Add(new Error { ErrorCode = "AR-DUP-CODE", ErrorMessage = $"Mã '{request.Codigo}' đã tồn tại trong nhóm cha này." });
                    return response;
                }

                if (entity.Id > 0)
                {
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.AgrupamentoConfigRepository.Update(entity);
                }
                else
                {
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.AgrupamentoConfigRepository.Add(entity);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract DeactivateAgrupamentoRubrica(DeactivateAgrupamentoRubricaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Agrupamentoconfig entity = _unitOfWork.AgrupamentoConfigRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "AR-NOT-FOUND", ErrorMessage = "Không tìm thấy bản ghi." });
                    return response;
                }

                if (_unitOfWork.AgrupamentoConfigRepository.AgrupamentoHasChilds(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "AR-HAS-CHILDREN", ErrorMessage = "Không thể xoá — vẫn còn mã con đang hoạt động bên dưới." });
                    return response;
                }

                entity.IndActivo = false;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.AgrupamentoConfigRepository.Update(entity);
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
