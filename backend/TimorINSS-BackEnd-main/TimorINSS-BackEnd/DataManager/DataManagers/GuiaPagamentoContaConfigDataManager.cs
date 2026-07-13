using System;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    // Cấu hình tài khoản Có (Crédito) dùng để tự sinh Lançamento khi 1 Guia
    // Pagamento được đối chiếu ngân hàng (xem GuiaConciliacaoDataManager). Đây
    // là bản cấu hình 1 dòng (singleton), nhưng có 2 TÀI KHOẢN CRÉDITO khác nhau
    // theo Setor (Público/Privado) của Entidade đóng góp — xác nhận qua sổ sách
    // thật SCFSSTL2024_VF.xlsm "Lançamentos": "2133114 Setor Privado - Guias
    // Emitidas" (đa số) và "2133112 Setor Público - Guias Emitidas" (thiểu số,
    // nhưng có thật). Phân loại Público/Privado lấy từ Entidadeempregadora.
    // EntidadeSectorActFk -> Sectoractividade.Descricao (field có sẵn, bắt buộc
    // nhập khi đăng ký công ty — 2026-07-13, sửa lại kết luận sai trước đó rằng
    // DB không có field này).
    // KHÔNG có Débito ở đây (2026-07-13, sửa sau khi đối chiếu với sổ sách thật
    // SCFSSTL2024_VF.xlsm "Lançamentos" — mọi bút toán thu GP đều Nợ vào ĐÚNG
    // tài khoản ngân hàng đã thực nhận tiền, không phải 1 tài khoản cố định).
    // Débito được GuiaConciliacaoDataManager tự tra động từ ContaBancaria.
    // CodigoContaFk của dòng sao kê ngân hàng đã khớp.
    public class GuiaPagamentoContaConfigDataManager : IGuiaPagamentoContaConfigDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public GuiaPagamentoContaConfigDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private static GuiaPagamentoContaConfigDataContract MapEntity(GuiaPagamentoContaConfig entity)
        {
            if (entity == null) return null;
            return new GuiaPagamentoContaConfigDataContract
            {
                Id = entity.Id,
                CodigoContaCreditoPrivadoFk = entity.CodigoContaCreditoPrivadoFk,
                CodigoContaCreditoPrivadoDesignacao = entity.CodigoContaCreditoPrivadoFkNavigation?.Designacao,
                CodigoContaCreditoPublicoFk = entity.CodigoContaCreditoPublicoFk,
                CodigoContaCreditoPublicoDesignacao = entity.CodigoContaCreditoPublicoFkNavigation?.Designacao
            };
        }

        public GuiaPagamentoContaConfigResponse GetConfig()
        {
            GuiaPagamentoContaConfigResponse response = new GuiaPagamentoContaConfigResponse();
            try
            {
                response.Item = MapEntity(_unitOfWork.GuiaPagamentoContaConfigRepository.GetActive());
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public GuiaPagamentoContaConfigResponse SaveConfig(SaveGuiaPagamentoContaConfigRequest request)
        {
            GuiaPagamentoContaConfigResponse response = new GuiaPagamentoContaConfigResponse { RequestId = request.RequestId };
            try
            {
                if (request.CodigoContaCreditoPrivadoFk.HasValue && _unitOfWork.CodigoContaRepository.Get(request.CodigoContaCreditoPrivadoFk.Value) == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "GPCC-CREDITO-PRIVADO-NOT-FOUND", ErrorMessage = "Không tìm thấy tài khoản Có (Setor Privado) đã chọn." });
                    return response;
                }
                if (request.CodigoContaCreditoPublicoFk.HasValue && _unitOfWork.CodigoContaRepository.Get(request.CodigoContaCreditoPublicoFk.Value) == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "GPCC-CREDITO-PUBLICO-NOT-FOUND", ErrorMessage = "Không tìm thấy tài khoản Có (Setor Público) đã chọn." });
                    return response;
                }

                GuiaPagamentoContaConfig entity = _unitOfWork.GuiaPagamentoContaConfigRepository.GetActive();
                if (entity == null)
                {
                    entity = new GuiaPagamentoContaConfig
                    {
                        CodigoContaCreditoPrivadoFk = request.CodigoContaCreditoPrivadoFk,
                        CodigoContaCreditoPublicoFk = request.CodigoContaCreditoPublicoFk,
                        IndActivo = true
                    };
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.GuiaPagamentoContaConfigRepository.Add(entity);
                }
                else
                {
                    entity.CodigoContaCreditoPrivadoFk = request.CodigoContaCreditoPrivadoFk;
                    entity.CodigoContaCreditoPublicoFk = request.CodigoContaCreditoPublicoFk;
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.GuiaPagamentoContaConfigRepository.Update(entity);
                }
                _unitOfWork.Commit();

                response.Item = MapEntity(_unitOfWork.GuiaPagamentoContaConfigRepository.GetActive());
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }
    }
}
