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
    public class CabimentoDataManager : ICabimentoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        private const string ESTADO_DRAFT = "DRAFT";
        private const string ESTADO_PENDING_APPROVAL = "PENDING_APPROVAL";
        private const string ESTADO_APPROVED = "APPROVED";

        public CabimentoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private CabimentoDataContract MapEntity(Cabimento entity, decimal valorComprometido = 0)
        {
            OrcamentoLinha rubrica = entity.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation;
            return new CabimentoDataContract
            {
                Id = entity.Id,
                Numero = entity.Numero,
                Mes = entity.Mes,
                Ano = entity.Ano,
                ExpenditureAuthorizationFk = entity.ExpenditureAuthorizationFk,
                ExpenditureAuthorizationNumero = entity.ExpenditureAuthorizationFkNavigation?.Numero ?? 0,
                AtividadeCodigo = rubrica?.AtividadeFkNavigation?.Codigo,
                AtividadeDesignacao = rubrica?.AtividadeFkNavigation?.Designacao,
                EconomicClassificationCodigo = rubrica?.EconomicClassificationFkNavigation?.Codigo,
                EconomicClassificationDesignacao = rubrica?.EconomicClassificationFkNavigation?.Designacao,
                OrganizationNome = rubrica?.OrganizationFkNavigation?.Nome,
                ValorAutorizadoAd = entity.ExpenditureAuthorizationFkNavigation != null
                    ? entity.ExpenditureAuthorizationFkNavigation.ValorAutorizado + entity.ExpenditureAuthorizationFkNavigation.Regularizacao
                    : 0,
                Descritivo = entity.Descritivo,
                ValorCabimentado = entity.ValorCabimentado,
                ValorComprometido = valorComprometido,
                SaldoDisponivel = entity.ValorCabimentado - valorComprometido,
                ProcessoAprovisionamentoPrevio = entity.ProcessoAprovisionamentoPrevio,
                Estado = entity.Estado,
                SubmittedAt = entity.SubmittedAt,
                ApprovedAt = entity.ApprovedAt,
                LastRejectComment = entity.LastRejectComment,
                LastRejectAt = entity.LastRejectAt
            };
        }

        public CabimentoListResponse GetByAno(GetCabimentoListRequest request)
        {
            CabimentoListResponse response = new CabimentoListResponse();
            try
            {
                List<Cabimento> items = _unitOfWork.CabimentoRepository.GetByAno(request.Ano);
                Dictionary<int, decimal> comprometidoPorCabimento = _unitOfWork.CabimentoRepository
                    .GetComprometidoByCabimentoIds(items.Select(c => c.Id).ToList());

                response.Items = items
                    .Select(c => MapEntity(c, comprometidoPorCabimento.TryGetValue(c.Id, out var v) ? v : 0))
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public AdsDisponiveisParaCabimentoResponse GetAdsDisponiveis(GetAdsDisponiveisParaCabimentoRequest request)
        {
            AdsDisponiveisParaCabimentoResponse response = new AdsDisponiveisParaCabimentoResponse();
            try
            {
                List<ExpenditureAuthorization> approvedAds = _unitOfWork.ExpenditureAuthorizationRepository
                    .GetByAno(request.Ano)
                    .Where(a => a.Estado == "APPROVED")
                    .ToList();

                response.Items = approvedAds
                    .Where(a => !_unitOfWork.CabimentoRepository.HasCabimentoForAd(a.Id))
                    .Select(a => new AdDisponivelParaCabimentoDataContract
                    {
                        ExpenditureAuthorizationId = a.Id,
                        Numero = a.Numero,
                        AtividadeCodigo = a.OrcamentoLinhaFkNavigation?.AtividadeFkNavigation?.Codigo,
                        AtividadeDesignacao = a.OrcamentoLinhaFkNavigation?.AtividadeFkNavigation?.Designacao,
                        EconomicClassificationCodigo = a.OrcamentoLinhaFkNavigation?.EconomicClassificationFkNavigation?.Codigo,
                        EconomicClassificationDesignacao = a.OrcamentoLinhaFkNavigation?.EconomicClassificationFkNavigation?.Designacao,
                        OrganizationNome = a.OrcamentoLinhaFkNavigation?.OrganizationFkNavigation?.Nome,
                        ValorRevisto = a.ValorAutorizado + a.Regularizacao
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public CabimentoResponse Create(CreateCabimentoRequest request)
        {
            CabimentoResponse response = new CabimentoResponse { RequestId = request.RequestId };
            try
            {
                ExpenditureAuthorization ad = _unitOfWork.ExpenditureAuthorizationRepository.Get(request.ExpenditureAuthorizationFk);
                if (ad == null || ad.Estado != "APPROVED")
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-AD-NOT-APPROVED", ErrorMessage = "AD phải ở trạng thái đã duyệt trước khi tạo Cabimento." });
                    return response;
                }

                if (_unitOfWork.CabimentoRepository.HasCabimentoForAd(request.ExpenditureAuthorizationFk))
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-DUP-AD", ErrorMessage = "AD này đã có Cabimento." });
                    return response;
                }

                decimal valorRevistoAd = ad.ValorAutorizado + ad.Regularizacao;
                if (request.ValorCabimentado > valorRevistoAd)
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-EXCEEDS-AD", ErrorMessage = $"Valor Cabimentado vượt quá saldo còn lại của AD ({valorRevistoAd:N2})." });
                    return response;
                }

                int numero = _unitOfWork.CabimentoRepository.GetNextNumero(request.Mes, request.Ano);

                Cabimento entity = new Cabimento
                {
                    Numero = numero,
                    Mes = request.Mes,
                    Ano = request.Ano,
                    ExpenditureAuthorizationFk = request.ExpenditureAuthorizationFk,
                    Descritivo = request.Descritivo,
                    ValorCabimentado = request.ValorCabimentado,
                    ProcessoAprovisionamentoPrevio = request.ProcessoAprovisionamentoPrevio,
                    Estado = ESTADO_DRAFT,
                    IndActivo = true
                };
                entity = _utils.SetDetailsToEntity(entity);
                _unitOfWork.CabimentoRepository.Add(entity);
                _unitOfWork.Commit();

                Cabimento created = _unitOfWork.CabimentoRepository.Get(entity.Id);
                response.Item = MapEntity(created);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Save(SaveCabimentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Cabimento entity = _unitOfWork.CabimentoRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-NOT-FOUND", ErrorMessage = "Không tìm thấy Cabimento." });
                    return response;
                }
                if (entity.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-NOT-DRAFT", ErrorMessage = "Cabimento đang chờ duyệt, không thể sửa." });
                    return response;
                }

                ExpenditureAuthorization ad = entity.ExpenditureAuthorizationFkNavigation;
                decimal valorRevistoAd = ad != null ? ad.ValorAutorizado + ad.Regularizacao : 0;
                if (request.ValorCabimentado > valorRevistoAd)
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-EXCEEDS-AD", ErrorMessage = $"Valor Cabimentado vượt quá saldo còn lại của AD ({valorRevistoAd:N2})." });
                    return response;
                }

                entity.Descritivo = request.Descritivo;
                entity.ValorCabimentado = request.ValorCabimentado;
                entity.ProcessoAprovisionamentoPrevio = request.ProcessoAprovisionamentoPrevio;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.CabimentoRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Submit(SubmitCabimentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Cabimento entity = _unitOfWork.CabimentoRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-NOT-FOUND", ErrorMessage = "Không tìm thấy Cabimento." });
                    return response;
                }
                if (entity.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-NOT-DRAFT", ErrorMessage = "Cabimento không ở trạng thái nháp." });
                    return response;
                }
                var attachmentConfig = _unitOfWork.AttachmentConfigRepository.Get();
                if (attachmentConfig.CabimentoObrigatorio && !_unitOfWork.AttachmentRepository.ExistsForEntity("CABIMENTO", entity.Id))
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-ATTACHMENT-REQUIRED", ErrorMessage = "Bắt buộc đính kèm file trước khi submit Cabimento." });
                    return response;
                }

                entity.Estado = ESTADO_PENDING_APPROVAL;
                entity.SubmittedBy = request.UserId;
                entity.SubmittedAt = DateTime.Now;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.CabimentoRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Approve(ApproveCabimentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Cabimento entity = _unitOfWork.CabimentoRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-NOT-FOUND", ErrorMessage = "Không tìm thấy Cabimento." });
                    return response;
                }
                if (entity.Estado != ESTADO_PENDING_APPROVAL)
                {
                    response.Errors.Add(new Error { ErrorCode = "CAB-WRONG-STATE", ErrorMessage = "Cabimento không ở trạng thái chờ phê duyệt." });
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
                _unitOfWork.CabimentoRepository.Update(entity);
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
