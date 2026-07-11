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
    // New DataManager for managing OrcamentoConfig itself (kỳ/năm ngân sách) —
    // separate from IOrcamentoDataManager/OrcamentoController, which manages
    // OrcamentoBatch/OrcamentoLinha (rúbrica orçamental) and is left untouched.
    public class OrcamentoConfigDataManager : IOrcamentoConfigDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public OrcamentoConfigDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public OrcamentoConfigListResponse GetAll()
        {
            OrcamentoConfigListResponse response = new OrcamentoConfigListResponse();
            try
            {
                response.Items = _unitOfWork.OrcamentoConfigRepository.GetAll()
                    .OrderByDescending(a => a.Ano)
                    .Select(ToDataContract)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Save(SaveOrcamentoConfigRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                if (request.DataFim.HasValue && request.DataFim.Value < request.DataInicio)
                {
                    response.Errors.Add(new Error { ErrorCode = "OC-INVALID-RANGE", ErrorMessage = "Ngày kết thúc phải sau ngày bắt đầu." });
                    return response;
                }

                Orcamentoconfig entity = new Orcamentoconfig
                {
                    Id = request.Id,
                    Ano = request.Ano,
                    // Only Principal (kỳ chính) creation is supported by this screen —
                    // Suplementar (điều chỉnh ngân sách giữa năm) is deferred, see
                    // db_migrations/2026-07-11i_system_settings.sql.
                    Tipo = "PRINCIPAL",
                    DataInicio = request.DataInicio,
                    DataFim = request.DataFim,
                    IndActivo = true
                };

                if (!_unitOfWork.OrcamentoConfigRepository.IsAnoTipoValid(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "OC-DUP-ANO", ErrorMessage = $"Đã có kỳ ngân sách Principal cho năm {request.Ano}." });
                    return response;
                }

                Error overlapError = _unitOfWork.OrcamentoConfigRepository.IsOrcamentoValid(entity);
                if (overlapError != null)
                {
                    response.Errors.Add(overlapError);
                    return response;
                }

                if (entity.Id > 0)
                {
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.OrcamentoConfigRepository.Update(entity);
                }
                else
                {
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.OrcamentoConfigRepository.Add(entity);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Deactivate(DeactivateOrcamentoConfigRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Orcamentoconfig entity = _unitOfWork.OrcamentoConfigRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OC-NOT-FOUND", ErrorMessage = "Không tìm thấy kỳ ngân sách." });
                    return response;
                }

                if (_unitOfWork.OrcamentoConfigRepository.HasOrcamentoBatch(entity.Id))
                {
                    response.Errors.Add(new Error { ErrorCode = "OC-HAS-BATCH", ErrorMessage = "Không thể xoá — đã có dữ liệu Orçamento (batch) gắn với kỳ này." });
                    return response;
                }

                if (!_unitOfWork.OrcamentoConfigRepository.IsOrcamentoDeleteValid(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "OC-HAS-DATA", ErrorMessage = "Không thể xoá — vẫn còn dữ liệu liên quan tới kỳ ngân sách này." });
                    return response;
                }

                entity.IndActivo = false;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.OrcamentoConfigRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        private static OrcamentoConfigDataContract ToDataContract(Orcamentoconfig entity)
        {
            return new OrcamentoConfigDataContract
            {
                Id = entity.Id,
                Ano = entity.Ano,
                Tipo = entity.Tipo,
                DataInicio = entity.DataInicio,
                DataFim = entity.DataFim,
                IndActivo = entity.IndActivo
            };
        }
    }
}
