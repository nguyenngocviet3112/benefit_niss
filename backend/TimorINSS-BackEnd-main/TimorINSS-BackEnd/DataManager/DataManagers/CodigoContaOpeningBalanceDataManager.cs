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
    // Cut-over — Saldos de Abertura, phần (a): giá trị đầu kỳ cho tài khoản
    // GL (Codigoconta.InitialValue/IsCredit). Cột này đã có sẵn trên bảng
    // Codigoconta từ trước (không phải bảng mới) — chỉ thiếu 1 màn hình mới
    // để nhập/sửa, tái dùng ICodigoContaRepository.GetAll()/Get()/Update()
    // đã có sẵn, không thêm method repository mới.
    public class CodigoContaOpeningBalanceDataManager : ICodigoContaOpeningBalanceDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public CodigoContaOpeningBalanceDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public CodigoContaOpeningBalanceListResponse GetAll()
        {
            CodigoContaOpeningBalanceListResponse response = new CodigoContaOpeningBalanceListResponse();
            try
            {
                var all = _unitOfWork.CodigoContaRepository.GetAll().ToList();
                var byId = all.ToDictionary(c => c.Id);

                // Codigoconta.Codigo là mã cục bộ theo từng cấp (vd "1"), lặp lại giống
                // hệt nhau ở nhiều nhánh khác nhau — mã tài khoản thật là ghép các mã
                // cục bộ từ gốc xuống, không dấu phân cách (xem memory
                // codigoconta-local-vs-full-code, đã áp dụng cho Plano de Contas).
                string BuildFullCodigo(Codigoconta node)
                {
                    var segments = new System.Collections.Generic.List<string>();
                    var current = node;
                    var guard = 0;
                    while (current != null && guard++ < 20)
                    {
                        segments.Insert(0, current.Codigo);
                        current = current.ParentFk.HasValue && byId.ContainsKey(current.ParentFk.Value)
                            ? byId[current.ParentFk.Value]
                            : null;
                    }
                    return string.Concat(segments);
                }

                response.Items = all
                    .Where(c => c.IndActivo)
                    .Select(c => new CodigoContaOpeningBalanceDataContract
                    {
                        Id = c.Id,
                        Codigo = c.Codigo,
                        FullCodigo = BuildFullCodigo(c),
                        Designacao = c.Designacao,
                        InitialValue = c.InitialValue,
                        IsCredit = c.IsCredit,
                        InitialValueDate = c.InitialValueDate
                    })
                    .OrderBy(c => c.FullCodigo)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Update(UpdateCodigoContaOpeningBalanceRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Codigoconta entity = _unitOfWork.CodigoContaRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OB-NOT-FOUND", ErrorMessage = "Không tìm thấy tài khoản." });
                    return response;
                }

                entity.InitialValue = request.InitialValue;
                entity.IsCredit = request.IsCredit;
                entity.InitialValueDate = request.InitialValueDate;
                entity = (Codigoconta)_utils.UpdateDetailsToEntity(entity);
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
