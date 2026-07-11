using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.ExcelReaderService;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class EconomicClassificationDataManager : IEconomicClassificationDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public EconomicClassificationDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public EconomicClassificationTreeResponse GetTreeByOrcamentoConfig(GetEconomicClassificationTreeRequest request)
        {
            EconomicClassificationTreeResponse response = new EconomicClassificationTreeResponse();
            try
            {
                List<EconomicClassification> items = _unitOfWork.EconomicClassificationRepository.GetTreeByOrcamentoConfig(request.OrcamentoConfigFk);
                HashSet<int> parentIds = items.Where(a => a.ParentFk.HasValue).Select(a => a.ParentFk.Value).ToHashSet();

                response.Items = items.Select(a => new EconomicClassificationDataContract
                {
                    Id = a.Id,
                    Codigo = a.Codigo,
                    Designacao = a.Designacao,
                    Nivel = a.Nivel,
                    ParentFk = a.ParentFk,
                    OrcamentoConfigFk = a.OrcamentoConfigFk,
                    IndActivo = a.IndActivo,
                    HasKids = parentIds.Contains(a.Id),
                    Tipo = a.Tipo
                }).ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract SaveEconomicClassification(SaveEconomicClassificationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                EconomicClassification entity = new EconomicClassification
                {
                    Id = request.Id,
                    Codigo = request.Codigo,
                    Designacao = request.Designacao,
                    Nivel = request.Nivel,
                    ParentFk = request.ParentFk,
                    OrcamentoConfigFk = request.OrcamentoConfigFk,
                    Tipo = request.Tipo,
                    IndActivo = true
                };

                if (!_unitOfWork.EconomicClassificationRepository.IsCodeValid(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "EC-DUP-CODE", ErrorMessage = $"Código '{request.Codigo}' já existe neste ano/kỳ ngân sách." });
                    return response;
                }

                if (entity.Id > 0)
                {
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.EconomicClassificationRepository.Update(entity);
                }
                else
                {
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.EconomicClassificationRepository.Add(entity);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract DeactivateEconomicClassification(DeactivateEconomicClassificationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                if (_unitOfWork.EconomicClassificationRepository.HasActiveChildren(request.Id))
                {
                    response.Errors.Add(new Error { ErrorCode = "EC-HAS-CHILDREN", ErrorMessage = "Không thể xoá — vẫn còn mã con đang hoạt động bên dưới." });
                    return response;
                }

                EconomicClassification entity = _unitOfWork.EconomicClassificationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "EC-NOT-FOUND", ErrorMessage = "Không tìm thấy bản ghi." });
                    return response;
                }

                entity.IndActivo = false;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.EconomicClassificationRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ImportMasterDataTreeResponse ImportEconomicClassification(ImportMasterDataTreeRequest request)
        {
            ImportMasterDataTreeResponse response = new ImportMasterDataTreeResponse { RequestId = request.RequestId };
            try
            {
                List<Dictionary<string, string>> rows = MasterDataTreeExcelReader.ReadDataSheet(request.File);
                response.Total = rows.Count;

                Dictionary<string, (int Id, int Nivel)> resolved = new Dictionary<string, (int, int)>();
                List<(int RowNum, Dictionary<string, string> Row)> pending = rows
                    .Select((row, idx) => (RowNum: idx + 3, Row: row))
                    .ToList();

                bool progress = true;
                while (pending.Count > 0 && progress)
                {
                    progress = false;
                    List<(int RowNum, Dictionary<string, string> Row)> stillPending = new List<(int, Dictionary<string, string>)>();

                    foreach ((int rowNum, Dictionary<string, string> row) in pending)
                    {
                        row.TryGetValue("Código", out string codigo);
                        row.TryGetValue("Designação", out string designacao);
                        row.TryGetValue("Código Pai", out string codigoPai);
                        row.TryGetValue("Tipo", out string tipo);
                        codigo = codigo?.Trim();
                        codigoPai = string.IsNullOrWhiteSpace(codigoPai) ? null : codigoPai.Trim();

                        if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(designacao))
                        {
                            response.Failed++;
                            response.RowErrors.Add(new ImportRowError { Row = rowNum, Codigo = codigo, Message = "Código hoặc Designação đang trống." });
                            continue;
                        }

                        if (codigoPai != null && !resolved.ContainsKey(codigoPai))
                        {
                            stillPending.Add((rowNum, row));
                            continue;
                        }

                        int? parentId = codigoPai != null ? resolved[codigoPai].Id : (int?)null;
                        int nivel = codigoPai != null ? resolved[codigoPai].Nivel + 1 : 1;

                        EconomicClassification entity = new EconomicClassification
                        {
                            Codigo = codigo,
                            Designacao = designacao,
                            Nivel = nivel,
                            ParentFk = parentId,
                            OrcamentoConfigFk = request.OrcamentoConfigFk,
                            Tipo = string.IsNullOrWhiteSpace(tipo) ? null : tipo.Trim(),
                            IndActivo = true
                        };

                        if (!_unitOfWork.EconomicClassificationRepository.IsCodeValid(entity))
                        {
                            response.Failed++;
                            response.RowErrors.Add(new ImportRowError { Row = rowNum, Codigo = codigo, Message = $"Código '{codigo}' já existe neste kỳ ngân sách." });
                            continue;
                        }

                        entity = _utils.SetDetailsToEntity(entity);
                        _unitOfWork.EconomicClassificationRepository.Add(entity);
                        _unitOfWork.Commit();

                        resolved[codigo] = (entity.Id, nivel);
                        response.Success++;
                        progress = true;
                    }

                    pending = stillPending;
                }

                foreach ((int rowNum, Dictionary<string, string> row) in pending)
                {
                    row.TryGetValue("Código", out string codigo);
                    row.TryGetValue("Código Pai", out string codigoPai);
                    response.Failed++;
                    response.RowErrors.Add(new ImportRowError { Row = rowNum, Codigo = codigo, Message = $"Không tìm thấy mã cha '{codigoPai}'." });
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }
    }
}
