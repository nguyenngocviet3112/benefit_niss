using System;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    // Cấu hình tài khoản Phải trả dùng làm tài khoản trung gian cho bút toán kép
    // của chu trình Despesa/Pagamento — xem PaymentDataManager.Approve/Execute.
    // 5 dòng cố định theo Obligation.BeneficiarioCategoria, chỉ Update, không Add.
    public class LiquidacaoContaConfigDataManager : ILiquidacaoContaConfigDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public LiquidacaoContaConfigDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private static LiquidacaoContaConfigDataContract MapEntity(Models.LiquidacaoContaConfig entity)
        {
            return new LiquidacaoContaConfigDataContract
            {
                Id = entity.Id,
                Categoria = entity.Categoria,
                CodigoContaFk = entity.CodigoContaFk,
                CodigoContaDesignacao = entity.CodigoContaFkNavigation?.Designacao
            };
        }

        public LiquidacaoContaConfigListResponse GetAll()
        {
            LiquidacaoContaConfigListResponse response = new LiquidacaoContaConfigListResponse();
            try
            {
                response.Items = _unitOfWork.LiquidacaoContaConfigRepository.GetAll()
                    .Select(MapEntity)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public LiquidacaoContaConfigListResponse Save(SaveLiquidacaoContaConfigRequest request)
        {
            LiquidacaoContaConfigListResponse response = new LiquidacaoContaConfigListResponse { RequestId = request.RequestId };
            try
            {
                if (_unitOfWork.CodigoContaRepository.Get(request.CodigoContaFk) == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "LCC-CONTA-NOT-FOUND", ErrorMessage = "Không tìm thấy tài khoản đã chọn." });
                    return response;
                }

                Models.LiquidacaoContaConfig entity = _unitOfWork.LiquidacaoContaConfigRepository.GetByCategoria(request.Categoria);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "LCC-CATEGORIA-NOT-FOUND", ErrorMessage = "Categoria không hợp lệ." });
                    return response;
                }

                entity.CodigoContaFk = request.CodigoContaFk;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.LiquidacaoContaConfigRepository.Update(entity);
                _unitOfWork.Commit();

                response.Items = _unitOfWork.LiquidacaoContaConfigRepository.GetAll()
                    .Select(MapEntity)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }
    }
}
