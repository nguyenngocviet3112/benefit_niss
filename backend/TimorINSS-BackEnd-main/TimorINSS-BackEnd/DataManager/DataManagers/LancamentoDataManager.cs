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
    // Registo de Lançamentos — chỉ đọc (GetList) từ phía người dùng. Bút toán
    // luôn được TỰ SINH bởi các luồng nghiệp vụ khác (Pagamento thực hiện,
    // Receita xác nhận...) qua GerarSeChuaCo, không có màn nhập tay — đúng
    // nguyên tắc đã chốt (xem memory lancamentos-conciliacao-link-design):
    // tránh việc kế toán phải nhập 2 lần (1 lần ở nghiệp vụ, 1 lần ở sổ cái).
    public class LancamentoDataManager : ILancamentoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public LancamentoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public LancamentoListResponse GetList(GetLancamentoListRequest request)
        {
            LancamentoListResponse response = new LancamentoListResponse();
            try
            {
                response.Items = _unitOfWork.LancamentoRepository
                    .GetByFilter(request.Ano, request.Mes, request.OrigemTipo)
                    .Select(l => new LancamentoDataContract
                    {
                        Id = l.Id,
                        Data = l.Data,
                        Descricao = l.Descricao,
                        CodigoContaDebitoFk = l.CodigoContaDebitoFk,
                        CodigoContaDebitoCodigo = l.CodigoContaDebitoFkNavigation?.Codigo,
                        CodigoContaDebitoDesignacao = l.CodigoContaDebitoFkNavigation?.Designacao,
                        CodigoContaCreditoFk = l.CodigoContaCreditoFk,
                        CodigoContaCreditoCodigo = l.CodigoContaCreditoFkNavigation?.Codigo,
                        CodigoContaCreditoDesignacao = l.CodigoContaCreditoFkNavigation?.Designacao,
                        Valor = l.Valor,
                        OrigemTipo = l.OrigemTipo,
                        OrigemId = l.OrigemId
                    }).ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public LancamentoGerarResult GerarSeChuaCo(string origemTipo, int origemId, DateTime data, int? codigoContaDebitoFk,
            int? codigoContaCreditoFk, decimal valor, string descricao)
        {
            var result = new LancamentoGerarResult();

            // Đã có Lançamento active cho origem này rồi — không phải sự kiện mới,
            // không cần thông báo lại (tránh làm phiền người dùng ở mọi lần load).
            if (_unitOfWork.LancamentoRepository.ExistsForOrigem(origemTipo, origemId))
                return result;

            // Valor <= 0 nghĩa là không có gì để ghi sổ (vd Receita không có phần
            // Caixa) — KHÔNG phải thiếu cấu hình, không nên báo warning.
            if (valor <= 0)
                return result;

            if (!codigoContaDebitoFk.HasValue || !codigoContaCreditoFk.HasValue)
            {
                result.FaltaConfiguracao = true;
                return result;
            }

            Lancamento entity = new Lancamento
            {
                Data = data,
                Descricao = descricao,
                CodigoContaDebitoFk = codigoContaDebitoFk.Value,
                CodigoContaCreditoFk = codigoContaCreditoFk.Value,
                Valor = valor,
                OrigemTipo = origemTipo,
                OrigemId = origemId,
                IndActivo = true
            };
            entity = _utils.SetDetailsToEntity(entity);
            _unitOfWork.LancamentoRepository.Add(entity);
            result.Gerado = true;
            return result;
        }

        public void DesfazerSeExiste(string origemTipo, int origemId)
        {
            _unitOfWork.LancamentoRepository.DeactivateForOrigem(origemTipo, origemId);
        }
    }
}
