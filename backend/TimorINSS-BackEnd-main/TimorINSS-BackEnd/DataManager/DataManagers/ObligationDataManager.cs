using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Extensions;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ObligationDataManager : IObligationDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        private const string ESTADO_DRAFT = "DRAFT";
        private const string ESTADO_PENDING_APPROVAL = "PENDING_APPROVAL";
        private const string ESTADO_APPROVED = "APPROVED";

        public ObligationDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private decimal SaldoDisponivel(CompromissoDespesa compromisso)
        {
            decimal valorRevisto = compromisso.ValorCompromissoAno + compromisso.Regularizacao;
            decimal jaComprometido = _unitOfWork.ObligationRepository.GetTotalCommittedForCompromisso(compromisso.Id);
            return valorRevisto - jaComprometido;
        }

        private ObligationDataContract MapEntity(Obligation entity)
        {
            List<ObligationItemDataContract> items = (entity.ObligationItem ?? new List<ObligationItem>())
                .Where(i => i.IndActivo)
                .Select(i =>
                {
                    CompromissoDespesa comp = i.CompromissoDespesaFkNavigation;
                    OrcamentoLinha rubrica = comp?.CabimentoFkNavigation?.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation;
                    return new ObligationItemDataContract
                    {
                        Id = i.Id,
                        CompromissoDespesaFk = i.CompromissoDespesaFk,
                        CompromissoDespesaNumero = comp?.Numero ?? 0,
                        CompromissoDespesaMes = comp?.Mes ?? 0,
                        AtividadeCodigo = rubrica?.AtividadeFkNavigation?.Codigo,
                        EconomicClassificationCodigo = rubrica?.EconomicClassificationFkNavigation?.Codigo,
                        EconomicClassificationDesignacao = rubrica?.EconomicClassificationFkNavigation?.Designacao,
                        CompromissoValorRevisto = comp != null ? comp.ValorCompromissoAno + comp.Regularizacao : 0,
                        CompromissoSaldoDisponivel = comp != null ? SaldoDisponivel(comp) + i.Value : 0,
                        Value = i.Value
                    };
                })
                .ToList();

            List<ObligationBeneficiaryDataContract> beneficiaries = (entity.ObligationBeneficiary ?? new List<ObligationBeneficiary>())
                .Where(b => b.IndActivo)
                .Select(b => new ObligationBeneficiaryDataContract
                {
                    Id = b.Id,
                    Niss = b.Niss,
                    NomeContribuinte = b.NomeContribuinte,
                    NomeBeneficiario = b.NomeBeneficiario,
                    NomeConta = b.NomeConta,
                    NumeroConta = b.NumeroConta,
                    Iban = b.Iban,
                    Swift = b.Swift,
                    Banco = b.Banco,
                    SalarioIliquido = b.SalarioIliquido,
                    Cotizacao4 = b.Cotizacao4,
                    Imposto10 = b.Imposto10,
                    SalarioLiquido = b.SalarioLiquido,
                    OutrosSuplementos = b.OutrosSuplementos,
                    MontanteAPagar = b.MontanteAPagar
                })
                .ToList();

            return new ObligationDataContract
            {
                Id = entity.Id,
                Numero = entity.Numero,
                Mes = entity.Mes,
                Ano = entity.Ano,
                DescritivoObrigacao = entity.DescritivoObrigacao,
                ValorObrigacao = items.Sum(i => i.Value),
                LiquidacaoTipo = entity.LiquidacaoTipo,
                BeneficiarioNome = entity.BeneficiarioNome,
                BeneficiarioNiss = entity.BeneficiarioNiss,
                BeneficiarioCategoria = entity.BeneficiarioCategoria,
                BeneficiarioNomeConta = entity.BeneficiarioNomeConta,
                BeneficiarioNumeroConta = entity.BeneficiarioNumeroConta,
                BeneficiarioIban = entity.BeneficiarioIban,
                BeneficiarioSwift = entity.BeneficiarioSwift,
                BeneficiarioBanco = entity.BeneficiarioBanco,
                BeneficiarioMontanteAPagar = entity.BeneficiarioMontanteAPagar,
                Estado = entity.Estado,
                SubmittedAt = entity.SubmittedAt,
                ApprovedAt = entity.ApprovedAt,
                LastRejectComment = entity.LastRejectComment,
                LastRejectAt = entity.LastRejectAt,
                Items = items,
                Beneficiaries = beneficiaries
            };
        }

        public ObligationListResponse GetByAno(GetObligationListRequest request)
        {
            ObligationListResponse response = new ObligationListResponse();
            try
            {
                response.Items = _unitOfWork.ObligationRepository.GetByAno(request.Ano)
                    .Select(MapEntity)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public StringFileReponse GetByAnoExcel(GetObligationListRequest request)
        {
            var response = new StringFileReponse { RequestId = request.RequestId };
            try
            {
                var list = GetByAno(request);
                if (list.Errors.Count > 0)
                {
                    response.Errors = list.Errors;
                    return response;
                }

                var headers = new[] { "N.º Obrigação", "Descritivo", "Compromisso relacionado", "Categoria",
                    "Beneficiário", "Valor Obrigação", "Estado" };

                var rows = new List<object[]>();
                foreach (var i in list.Items)
                {
                    string compromissos = string.Join(", ", i.Items.Select(it => $"{it.CompromissoDespesaNumero}/{it.CompromissoDespesaMes}"));
                    rows.Add(new object[] { $"{i.Numero}/{i.Ano}", i.DescritivoObrigacao, compromissos,
                        i.BeneficiarioCategoria, i.BeneficiarioNome, i.ValorObrigacao, i.Estado });
                }

                response.File = ExcelExportHelper.BuildXlsxBase64("Registo Obrigação", headers, rows);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public CompromissosComSaldoResponse GetCompromissosComSaldo(GetCompromissosComSaldoRequest request)
        {
            CompromissosComSaldoResponse response = new CompromissosComSaldoResponse();
            try
            {
                List<CompromissoDespesa> approved = _unitOfWork.CompromissoDespesaRepository.GetByAno(request.Ano)
                    .Where(c => c.Estado == "APPROVED")
                    .ToList();

                response.Items = approved
                    .Select(c =>
                    {
                        OrcamentoLinha rubrica = c.CabimentoFkNavigation?.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation;
                        return new CompromissoComSaldoDataContract
                        {
                            CompromissoDespesaId = c.Id,
                            Numero = c.Numero,
                            Mes = c.Mes,
                            AtividadeCodigo = rubrica?.AtividadeFkNavigation?.Codigo,
                            AtividadeDesignacao = rubrica?.AtividadeFkNavigation?.Designacao,
                            EconomicClassificationCodigo = rubrica?.EconomicClassificationFkNavigation?.Codigo,
                            EconomicClassificationDesignacao = rubrica?.EconomicClassificationFkNavigation?.Designacao,
                            OrganizationNome = rubrica?.OrganizationFkNavigation?.Nome,
                            ValorRevisto = c.ValorCompromissoAno + c.Regularizacao,
                            SaldoDisponivel = SaldoDisponivel(c)
                        };
                    })
                    .Where(c => c.SaldoDisponivel > 0)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ObligationResponse Create(CreateObligationRequest request)
        {
            ObligationResponse response = new ObligationResponse { RequestId = request.RequestId };
            try
            {
                int numero = _unitOfWork.ObligationRepository.GetNextNumero(request.Mes, request.Ano);

                Obligation entity = new Obligation
                {
                    Numero = numero,
                    Mes = request.Mes,
                    Ano = request.Ano,
                    DescritivoObrigacao = request.DescritivoObrigacao,
                    LiquidacaoTipo = request.LiquidacaoTipo,
                    BeneficiarioNome = request.BeneficiarioNome,
                    BeneficiarioNiss = request.BeneficiarioNiss,
                    BeneficiarioCategoria = request.BeneficiarioCategoria,
                    BeneficiarioNomeConta = request.BeneficiarioNomeConta,
                    BeneficiarioNumeroConta = request.BeneficiarioNumeroConta,
                    BeneficiarioIban = request.BeneficiarioIban,
                    BeneficiarioSwift = request.BeneficiarioSwift,
                    BeneficiarioBanco = request.BeneficiarioBanco,
                    BeneficiarioMontanteAPagar = request.BeneficiarioMontanteAPagar,
                    Estado = ESTADO_DRAFT,
                    IndActivo = true
                };
                entity = _utils.SetDetailsToEntity(entity);
                _unitOfWork.ObligationRepository.Add(entity);
                _unitOfWork.Commit();

                Obligation created = _unitOfWork.ObligationRepository.Get(entity.Id);
                response.Item = MapEntity(created);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract AddItem(AddObligationItemRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Obligation obligation = _unitOfWork.ObligationRepository.Get(request.ObligationFk);
                if (obligation == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-FOUND", ErrorMessage = "Không tìm thấy Obrigação." });
                    return response;
                }
                if (obligation.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-DRAFT", ErrorMessage = "Obrigação đang chờ duyệt, không thể sửa." });
                    return response;
                }

                CompromissoDespesa compromisso = _unitOfWork.CompromissoDespesaRepository.Get(request.CompromissoDespesaFk);
                if (compromisso == null || compromisso.Estado != "APPROVED")
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-COMP-NOT-APPROVED", ErrorMessage = "Compromisso phải ở trạng thái đã duyệt." });
                    return response;
                }

                decimal saldo = SaldoDisponivel(compromisso);
                if (request.Value > saldo)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-EXCEEDS-SALDO", ErrorMessage = $"Giá trị vượt quá saldo còn lại của Compromisso ({saldo:N2})." });
                    return response;
                }
                if (request.Value <= 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-INVALID-VALUE", ErrorMessage = "Giá trị phải lớn hơn 0." });
                    return response;
                }

                ObligationItem item = new ObligationItem
                {
                    ObligationFk = request.ObligationFk,
                    CompromissoDespesaFk = request.CompromissoDespesaFk,
                    Value = request.Value,
                    IndActivo = true
                };
                item = _utils.SetDetailsToEntity(item);
                _unitOfWork.ObligationRepository.AddItem(item);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract RemoveItem(RemoveObligationItemRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ObligationItem item = _unitOfWork.ObligationRepository.GetItem(request.Id);
                if (item == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-ITEM-NOT-FOUND", ErrorMessage = "Không tìm thấy dòng." });
                    return response;
                }

                Obligation obligation = _unitOfWork.ObligationRepository.Get(item.ObligationFk);
                if (obligation == null || obligation.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-DRAFT", ErrorMessage = "Obrigação đang chờ duyệt, không thể xoá dòng." });
                    return response;
                }

                item.IndActivo = false;
                item = _utils.UpdateDetailsToEntity(item);
                _unitOfWork.ObligationRepository.UpdateItem(item);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract AddBeneficiary(AddObligationBeneficiaryRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Obligation obligation = _unitOfWork.ObligationRepository.Get(request.ObligationFk);
                if (obligation == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-FOUND", ErrorMessage = "Không tìm thấy Obrigação." });
                    return response;
                }
                if (obligation.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-DRAFT", ErrorMessage = "Obrigação đang chờ duyệt, không thể sửa." });
                    return response;
                }
                if (request.MontanteAPagar <= 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-BEN-INVALID-VALUE", ErrorMessage = "Montante a pagar phải lớn hơn 0." });
                    return response;
                }

                ObligationBeneficiary beneficiary = new ObligationBeneficiary
                {
                    ObligationFk = request.ObligationFk,
                    Niss = request.Niss,
                    NomeContribuinte = request.NomeContribuinte,
                    NomeBeneficiario = request.NomeBeneficiario,
                    NomeConta = request.NomeConta,
                    NumeroConta = request.NumeroConta,
                    Iban = request.Iban,
                    Swift = request.Swift,
                    Banco = request.Banco,
                    SalarioIliquido = request.SalarioIliquido,
                    Cotizacao4 = request.Cotizacao4,
                    Imposto10 = request.Imposto10,
                    SalarioLiquido = request.SalarioLiquido,
                    OutrosSuplementos = request.OutrosSuplementos,
                    MontanteAPagar = request.MontanteAPagar,
                    IndActivo = true
                };
                beneficiary = _utils.SetDetailsToEntity(beneficiary);
                _unitOfWork.ObligationRepository.AddBeneficiary(beneficiary);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract RemoveBeneficiary(RemoveObligationBeneficiaryRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ObligationBeneficiary beneficiary = _unitOfWork.ObligationRepository.GetBeneficiary(request.Id);
                if (beneficiary == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-BEN-NOT-FOUND", ErrorMessage = "Không tìm thấy người thụ hưởng." });
                    return response;
                }

                Obligation obligation = _unitOfWork.ObligationRepository.Get(beneficiary.ObligationFk);
                if (obligation == null || obligation.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-DRAFT", ErrorMessage = "Obrigação đang chờ duyệt, không thể xoá." });
                    return response;
                }

                beneficiary.IndActivo = false;
                beneficiary = _utils.UpdateDetailsToEntity(beneficiary);
                _unitOfWork.ObligationRepository.UpdateBeneficiary(beneficiary);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Submit(SubmitObligationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Obligation entity = _unitOfWork.ObligationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-FOUND", ErrorMessage = "Không tìm thấy Obrigação." });
                    return response;
                }
                if (entity.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-DRAFT", ErrorMessage = "Obrigação không ở trạng thái nháp." });
                    return response;
                }
                if (!entity.ObligationItem.Any(i => i.IndActivo))
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-EMPTY", ErrorMessage = "Chưa có dòng Compromisso nào để gửi duyệt." });
                    return response;
                }
                var attachmentConfig = _unitOfWork.AttachmentConfigRepository.Get();
                if (attachmentConfig.ObrigacaoObrigatorio && !_unitOfWork.AttachmentRepository.ExistsForEntity("OBRIGACAO", entity.Id))
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-ATTACHMENT-REQUIRED", ErrorMessage = "Bắt buộc đính kèm file trước khi submit Obrigação." });
                    return response;
                }

                entity.Estado = ESTADO_PENDING_APPROVAL;
                entity.SubmittedBy = request.UserId;
                entity.SubmittedAt = DateTime.Now;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.ObligationRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Approve(ApproveObligationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Obligation entity = _unitOfWork.ObligationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-FOUND", ErrorMessage = "Không tìm thấy Obrigação." });
                    return response;
                }
                if (entity.Estado != ESTADO_PENDING_APPROVAL)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-WRONG-STATE", ErrorMessage = "Obrigação không ở trạng thái chờ phê duyệt." });
                    return response;
                }

                if (request.Approve)
                {
                    entity.Estado = ESTADO_APPROVED;
                    entity.ApprovedBy = request.UserId;
                    entity.ApprovedAt = DateTime.Now;
                }
                else
                {
                    entity.Estado = ESTADO_DRAFT;
                    entity.LastRejectBy = request.UserId;
                    entity.LastRejectAt = DateTime.Now;
                    entity.LastRejectComment = request.Comment;
                }

                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.ObligationRepository.Update(entity);
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
