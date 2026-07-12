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
                response.Items = _unitOfWork.CodigoContaRepository.GetAll()
                    .Where(c => c.IndActivo)
                    .OrderBy(c => c.Codigo)
                    .Select(c => new CodigoContaOpeningBalanceDataContract
                    {
                        Id = c.Id,
                        Codigo = c.Codigo,
                        Designacao = c.Designacao,
                        InitialValue = c.InitialValue,
                        IsCredit = c.IsCredit,
                        InitialValueDate = c.InitialValueDate
                    })
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
