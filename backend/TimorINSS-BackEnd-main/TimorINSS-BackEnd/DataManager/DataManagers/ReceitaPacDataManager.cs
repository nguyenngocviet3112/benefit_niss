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
    public class ReceitaPacDataManager : IReceitaPacDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;
        private readonly ILancamentoDataManager _lancamentoDataManager;

        public ReceitaPacDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils, ILancamentoDataManager lancamentoDataManager)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _lancamentoDataManager = lancamentoDataManager;
        }

        private ReceitaPacDataContract MapEntity(ReceitaPac entity)
        {
            decimal total = entity.ValorCobradoBanco + entity.ValorCobradoCaixa;
            return new ReceitaPacDataContract
            {
                Id = entity.Id,
                Numero = entity.Numero,
                Mes = entity.Mes,
                Ano = entity.Ano,
                Niss = entity.Niss,
                RegimeFk = entity.RegimeFk,
                RegimeDesignacao = entity.RegimeFkNavigation?.Designacao,
                AtividadeFk = entity.AtividadeFk,
                AtividadeCodigo = entity.AtividadeFkNavigation?.Codigo,
                AtividadeDesignacao = entity.AtividadeFkNavigation?.Designacao,
                EconomicClassificationFk = entity.EconomicClassificationFk,
                EconomicClassificationCodigo = entity.EconomicClassificationFkNavigation?.Codigo,
                EconomicClassificationDesignacao = entity.EconomicClassificationFkNavigation?.Designacao,
                OrganizationFk = entity.OrganizationFk,
                OrganizationNome = entity.OrganizationFkNavigation?.Nome,
                Descritivo = entity.Descritivo,
                ValorPac = entity.ValorPac,
                ValorCobradoBanco = entity.ValorCobradoBanco,
                ValorCobradoCaixa = entity.ValorCobradoCaixa,
                ContaBancariaFk = entity.ContaBancariaFk,
                ContaBancariaNome = entity.ContaBancariaFkNavigation == null
                    ? null
                    : $"{entity.ContaBancariaFkNavigation.EntidadeBancaria} - {entity.ContaBancariaFkNavigation.Descricao}",
                CodigoContaDebitoFk = entity.CodigoContaDebitoFk,
                CodigoContaDebitoDesignacao = entity.CodigoContaDebitoFkNavigation?.Designacao,
                CodigoContaCreditoFk = entity.CodigoContaCreditoFk,
                CodigoContaCreditoDesignacao = entity.CodigoContaCreditoFkNavigation?.Designacao,
                ValorCobradoTotal = total,
                SaldoPorCobrar = entity.ValorPac - total
            };
        }

        public ReceitaPacListResponse GetByAno(GetReceitaPacListRequest request)
        {
            ReceitaPacListResponse response = new ReceitaPacListResponse();
            try
            {
                response.Items = _unitOfWork.ReceitaPacRepository.GetByAno(request.Ano)
                    .Select(MapEntity)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        // Walk ParentFk up to the root — never trust a fixed-depth Include(), load flat + walk in
        // memory (see memory tracked-bin-obj-stale-build-gotcha / ce-inss-global-impl-status lesson).
        private static ProgramActivity GetRoot(ProgramActivity node, List<ProgramActivity> all)
        {
            while (node.ParentFk.HasValue)
            {
                ProgramActivity parent = all.FirstOrDefault(a => a.Id == node.ParentFk.Value);
                if (parent == null) break;
                node = parent;
            }
            return node;
        }

        private static EconomicClassification GetRoot(EconomicClassification node, List<EconomicClassification> all)
        {
            while (node.ParentFk.HasValue)
            {
                EconomicClassification parent = all.FirstOrDefault(e => e.Id == node.ParentFk.Value);
                if (parent == null) break;
                node = parent;
            }
            return node;
        }

        public ReceitaPacResponse Save(SaveReceitaPacRequest request)
        {
            ReceitaPacResponse response = new ReceitaPacResponse { RequestId = request.RequestId };
            try
            {
                EconomicClassification ec = _unitOfWork.EconomicClassificationRepository.Get(request.EconomicClassificationFk);
                if (ec == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "REC-EC-NOT-FOUND", ErrorMessage = "Không tìm thấy Classificação Económica." });
                    return response;
                }
                List<EconomicClassification> ecTree = _unitOfWork.EconomicClassificationRepository.GetTreeByOrcamentoConfig(ec.BudgetPeriodFk);
                EconomicClassification ecRoot = GetRoot(ec, ecTree);
                if (ecRoot.Tipo != "Receita")
                {
                    response.Errors.Add(new Error { ErrorCode = "REC-EC-NOT-RECEITA", ErrorMessage = "Classificação Económica đã chọn không thuộc nhóm Receita." });
                    return response;
                }

                ProgramActivity regime = _unitOfWork.ProgramActivityRepository.Get(request.RegimeFk);
                if (regime == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "REC-REGIME-NOT-FOUND", ErrorMessage = "Không tìm thấy Regime." });
                    return response;
                }
                if (regime.ParentFk != null)
                {
                    response.Errors.Add(new Error { ErrorCode = "REC-REGIME-NOT-ROOT", ErrorMessage = "Regime phải là Programa cấp cao nhất (A04-A08)." });
                    return response;
                }

                if (request.AtividadeFk.HasValue && _unitOfWork.ProgramActivityRepository.Get(request.AtividadeFk.Value) == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "REC-ATIVIDADE-NOT-FOUND", ErrorMessage = "Không tìm thấy Atividade." });
                    return response;
                }

                if (request.ContaBancariaFk.HasValue && _unitOfWork.ContaBancariaRepository.Get(request.ContaBancariaFk.Value) == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "REC-CONTABANCARIA-NOT-FOUND", ErrorMessage = "Không tìm thấy ngân hàng đã chọn." });
                    return response;
                }

                ReceitaPac entity;
                if (request.Id > 0)
                {
                    entity = _unitOfWork.ReceitaPacRepository.Get(request.Id);
                    if (entity == null)
                    {
                        response.Errors.Add(new Error { ErrorCode = "REC-NOT-FOUND", ErrorMessage = "Không tìm thấy Receita." });
                        return response;
                    }
                    entity.Niss = request.Niss;
                    entity.RegimeFk = request.RegimeFk;
                    entity.AtividadeFk = request.AtividadeFk;
                    entity.EconomicClassificationFk = request.EconomicClassificationFk;
                    entity.OrganizationFk = request.OrganizationFk;
                    entity.Descritivo = request.Descritivo;
                    entity.ValorPac = request.ValorPac;
                    entity.ValorCobradoBanco = request.ValorCobradoBanco;
                    entity.ValorCobradoCaixa = request.ValorCobradoCaixa;
                    entity.ContaBancariaFk = request.ContaBancariaFk;
                    entity.CodigoContaDebitoFk = request.CodigoContaDebitoFk;
                    entity.CodigoContaCreditoFk = request.CodigoContaCreditoFk;
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.ReceitaPacRepository.Update(entity);
                }
                else
                {
                    int numero = _unitOfWork.ReceitaPacRepository.GetNextNumero(request.Mes, request.Ano);
                    entity = new ReceitaPac
                    {
                        Numero = numero,
                        Mes = request.Mes,
                        Ano = request.Ano,
                        Niss = request.Niss,
                        RegimeFk = request.RegimeFk,
                        AtividadeFk = request.AtividadeFk,
                        EconomicClassificationFk = request.EconomicClassificationFk,
                        OrganizationFk = request.OrganizationFk,
                        Descritivo = request.Descritivo,
                        ValorPac = request.ValorPac,
                        ValorCobradoBanco = request.ValorCobradoBanco,
                        ValorCobradoCaixa = request.ValorCobradoCaixa,
                        ContaBancariaFk = request.ContaBancariaFk,
                        CodigoContaDebitoFk = request.CodigoContaDebitoFk,
                        CodigoContaCreditoFk = request.CodigoContaCreditoFk,
                        IndActivo = true
                    };
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.ReceitaPacRepository.Add(entity);
                }
                _unitOfWork.Commit(); // entity.Id (identity) chỉ có giá trị thật sau Commit

                // Chỉ phần Caixa (thu tiền mặt) ghi sổ NGAY ở đây — không có sao kê
                // ngân hàng nào để đối chiếu tiền mặt, nên không có cách xác minh độc
                // lập nào khác ngoài chính bước nhập liệu này.
                // Phần Banco (thu qua ngân hàng) KHÔNG ghi sổ ở đây nữa (2026-07-12,
                // theo cùng nguyên tắc đã áp dụng cho Guia Pagamento — xem memory
                // guia-pagamento-lancamento-wiring): ValorCobradoBanco là số cán bộ tự
                // gõ, chưa được kiểm chứng với dữ liệu ngân hàng thật. Bút toán phần
                // Banco giờ chỉ sinh khi BankStatementLineDataManager.MatchReceita xác
                // nhận khớp với 1 dòng sao kê ngân hàng thật (origemTipo="ReceitaPacBanco").
                // Idempotent theo GerarSeChuaCo's ExistsForOrigem — sửa lại 1 Receita đã
                // có Lançamento sẽ không tạo dòng thứ 2, nhưng cũng không cập nhật giá trị
                // dòng cũ nếu Valor thay đổi sau đó — cùng giới hạn với bên Pagamento.
                var lancResult = _lancamentoDataManager.GerarSeChuaCo(
                    origemTipo: "ReceitaPacCaixa",
                    origemId: entity.Id,
                    data: new DateTime(request.Ano, request.Mes, 1),
                    codigoContaDebitoFk: request.CodigoContaDebitoFk,
                    codigoContaCreditoFk: request.CodigoContaCreditoFk,
                    valor: request.ValorCobradoCaixa,
                    descricao: $"Receita PAC Nº {entity.Numero}/{entity.Ano} - {entity.Descritivo} (Caixa)");

                if (lancResult.FaltaConfiguracao)
                {
                    response.Warnings.Add($"Chưa ghi được bút toán kế toán cho phần Caixa của Receita Nº {entity.Numero}/{entity.Ano} vì thiếu Tài khoản Nợ/Có — bổ sung 2 trường này ngay trên dòng Receita rồi lưu lại.");
                }
                else if (lancResult.Gerado)
                {
                    response.Warnings.Add($"Đã tự động ghi bút toán kế toán cho phần Caixa của Receita Nº {entity.Numero}/{entity.Ano}. Kiểm tra tại Registo de Lançamentos nếu cần điều chỉnh.");
                }

                _unitOfWork.Commit();

                ReceitaPac saved = _unitOfWork.ReceitaPacRepository.Get(entity.Id);
                response.Item = MapEntity(saved);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Deactivate(DeactivateReceitaPacRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ReceitaPac entity = _unitOfWork.ReceitaPacRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "REC-NOT-FOUND", ErrorMessage = "Không tìm thấy Receita." });
                    return response;
                }
                entity.IndActivo = false;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.ReceitaPacRepository.Update(entity);
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
