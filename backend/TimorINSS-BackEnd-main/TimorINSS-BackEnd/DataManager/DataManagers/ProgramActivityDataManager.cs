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
    public class ProgramActivityDataManager : IProgramActivityDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ProgramActivityDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public ProgramActivityTreeResponse GetTreeByOrcamentoConfig(GetProgramActivityTreeRequest request)
        {
            ProgramActivityTreeResponse response = new ProgramActivityTreeResponse();
            try
            {
                List<ProgramActivity> items = _unitOfWork.ProgramActivityRepository.GetTreeByOrcamentoConfig(request.OrcamentoConfigFk);
                HashSet<int> parentIds = items.Where(a => a.ParentFk.HasValue).Select(a => a.ParentFk.Value).ToHashSet();

                response.Items = items.Select(a => new ProgramActivityDataContract
                {
                    Id = a.Id,
                    Codigo = a.Codigo,
                    Designacao = a.Designacao,
                    Nivel = a.Nivel,
                    ParentFk = a.ParentFk,
                    OrcamentoConfigFk = a.BudgetPeriodFk,
                    IndActivo = a.IndActivo,
                    IsOssPerimeter = a.IsOssPerimeter,
                    HasKids = parentIds.Contains(a.Id)
                }).ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract SaveProgramActivity(SaveProgramActivityRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ProgramActivity entity = new ProgramActivity
                {
                    Id = request.Id,
                    Codigo = request.Codigo,
                    Designacao = request.Designacao,
                    Nivel = request.Nivel,
                    ParentFk = request.ParentFk,
                    BudgetPeriodFk = request.OrcamentoConfigFk,
                    IndActivo = true
                };

                if (!_unitOfWork.ProgramActivityRepository.IsCodeValid(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "PA-DUP-CODE", ErrorMessage = $"Código '{request.Codigo}' já existe neste ano/kỳ ngân sách." });
                    return response;
                }

                if (entity.Id > 0)
                {
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.ProgramActivityRepository.Update(entity);
                }
                else
                {
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.ProgramActivityRepository.Add(entity);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract DeactivateProgramActivity(DeactivateProgramActivityRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                if (_unitOfWork.ProgramActivityRepository.HasActiveChildren(request.Id))
                {
                    response.Errors.Add(new Error { ErrorCode = "PA-HAS-CHILDREN", ErrorMessage = "Không thể vô hiệu hoá — vẫn còn Subprograma/Atividade con đang hoạt động bên dưới." });
                    return response;
                }

                ProgramActivity entity = _unitOfWork.ProgramActivityRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "PA-NOT-FOUND", ErrorMessage = "Không tìm thấy bản ghi." });
                    return response;
                }

                entity.IndActivo = false;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.ProgramActivityRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract CopyProgramActivityYear(CopyProgramActivityYearRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                List<ProgramActivity> source = _unitOfWork.ProgramActivityRepository
                    .GetTreeByOrcamentoConfig(request.SourceOrcamentoConfigFk);

                Dictionary<int, int> oldIdToNewId = new Dictionary<int, int>();

                foreach (int nivel in source.Select(a => a.Nivel).Distinct().OrderBy(n => n))
                {
                    foreach (ProgramActivity original in source.Where(a => a.Nivel == nivel))
                    {
                        int? newParentFk = original.ParentFk.HasValue ? oldIdToNewId[original.ParentFk.Value] : (int?)null;

                        ProgramActivity copy = new ProgramActivity
                        {
                            Codigo = original.Codigo,
                            Designacao = original.Designacao,
                            Nivel = original.Nivel,
                            ParentFk = newParentFk,
                            BudgetPeriodFk = request.TargetOrcamentoConfigFk,
                            IndActivo = true
                        };
                        copy = _utils.SetDetailsToEntity(copy);
                        _unitOfWork.ProgramActivityRepository.Add(copy);
                        _unitOfWork.Commit();

                        oldIdToNewId[original.Id] = copy.Id;
                    }
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ImportMasterDataTreeResponse ImportProgramActivity(ImportMasterDataTreeRequest request)
        {
            ImportMasterDataTreeResponse response = new ImportMasterDataTreeResponse { RequestId = request.RequestId };
            try
            {
                List<Dictionary<string, string>> rows = MasterDataTreeExcelReader.ReadDataSheet(request.File);
                response.Total = rows.Count;

                // "Nível" no ficheiro é texto (Programa/Subprograma/Atividade) — só usado
                // para gravar o Nivel numérico; a ordem de resolução de pais usa o
                // algoritmo iterativo genérico (não depende deste texto).
                Dictionary<string, int> levelNameToNumber = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Programa", 1 },
                    { "Subprograma", 2 },
                    { "Atividade", 3 }
                };

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
                        row.TryGetValue("Nível", out string nivelTexto);
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
                        if (nivelTexto != null && levelNameToNumber.TryGetValue(nivelTexto.Trim(), out int explicitNivel))
                            nivel = explicitNivel;

                        ProgramActivity entity = new ProgramActivity
                        {
                            Codigo = codigo,
                            Designacao = designacao,
                            Nivel = nivel,
                            ParentFk = parentId,
                            BudgetPeriodFk = request.OrcamentoConfigFk,
                            IndActivo = true
                        };

                        if (!_unitOfWork.ProgramActivityRepository.IsCodeValid(entity))
                        {
                            response.Failed++;
                            response.RowErrors.Add(new ImportRowError { Row = rowNum, Codigo = codigo, Message = $"Código '{codigo}' já existe neste kỳ ngân sách." });
                            continue;
                        }

                        entity = _utils.SetDetailsToEntity(entity);
                        _unitOfWork.ProgramActivityRepository.Add(entity);
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
