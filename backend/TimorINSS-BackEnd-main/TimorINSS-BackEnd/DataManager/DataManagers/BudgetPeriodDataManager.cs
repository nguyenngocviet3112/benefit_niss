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
    // Kỳ ngân sách (System Settings), new-mode-only entity — split off from the
    // pre-existing [Orcamentoconfig] on 2026-07-12 so new-mode never has to touch
    // a table old-mode also depends on (Codigoconta/CentroCusto/ComponenteOrcamentoRegisto
    // year-versioning). See db_migrations/2026-07-12f_budget_period.sql.
    public class BudgetPeriodDataManager : IBudgetPeriodDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public BudgetPeriodDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public BudgetPeriodListResponse GetAll()
        {
            BudgetPeriodListResponse response = new BudgetPeriodListResponse();
            try
            {
                response.Items = _unitOfWork.BudgetPeriodRepository.GetAll()
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

        public ResponseBaseDataContract Save(SaveBudgetPeriodRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                if (request.DataFim.HasValue && request.DataFim.Value < request.DataInicio)
                {
                    response.Errors.Add(new Error { ErrorCode = "BP-INVALID-RANGE", ErrorMessage = "Ngày kết thúc phải sau ngày bắt đầu." });
                    return response;
                }

                BudgetPeriod entity = new BudgetPeriod
                {
                    Id = request.Id,
                    Ano = request.Ano,
                    // Only Principal (kỳ chính) creation is supported by this screen —
                    // Suplementar (điều chỉnh ngân sách giữa năm) is deferred, matches
                    // the behaviour this was split from (2026-07-11i_system_settings.sql).
                    Tipo = "PRINCIPAL",
                    DataInicio = request.DataInicio,
                    DataFim = request.DataFim,
                    IndActivo = true
                };

                if (!_unitOfWork.BudgetPeriodRepository.IsAnoTipoValid(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "BP-DUP-ANO", ErrorMessage = $"Đã có kỳ ngân sách Principal cho năm {request.Ano}." });
                    return response;
                }

                if (entity.Id > 0)
                {
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.BudgetPeriodRepository.Update(entity);
                }
                else
                {
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.BudgetPeriodRepository.Add(entity);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Deactivate(DeactivateBudgetPeriodRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                BudgetPeriod entity = _unitOfWork.BudgetPeriodRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "BP-NOT-FOUND", ErrorMessage = "Không tìm thấy kỳ ngân sách." });
                    return response;
                }

                if (_unitOfWork.BudgetPeriodRepository.HasOrcamentoBatch(entity.Id) || _unitOfWork.BudgetPeriodRepository.HasDependents(entity.Id))
                {
                    response.Errors.Add(new Error { ErrorCode = "BP-HAS-DATA", ErrorMessage = "Không thể xoá — vẫn còn dữ liệu (Orçamento, Atividade, Classificação Económica...) gắn với kỳ này." });
                    return response;
                }

                entity.IndActivo = false;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.BudgetPeriodRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        private static BudgetPeriodDataContract ToDataContract(BudgetPeriod entity)
        {
            return new BudgetPeriodDataContract
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
