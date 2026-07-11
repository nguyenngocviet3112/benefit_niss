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
                    OrcamentoConfigFk = request.OrcamentoConfigFk,
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
                            OrcamentoConfigFk = request.TargetOrcamentoConfigFk,
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
    }
}
