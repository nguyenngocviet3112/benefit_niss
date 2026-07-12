using System;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class AttachmentConfigDataManager : IAttachmentConfigDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttachmentConfigDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AttachmentConfigResponse Get()
        {
            var response = new AttachmentConfigResponse();
            try
            {
                var config = _unitOfWork.AttachmentConfigRepository.Get();
                response.item = ToDataContract(config);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Save(SaveAttachmentConfigRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                var entity = _unitOfWork.AttachmentConfigRepository.Get();
                entity.MaxFileSizeMb = request.MaxFileSizeMb;
                entity.AdObrigatorio = request.AdObrigatorio;
                entity.CabimentoObrigatorio = request.CabimentoObrigatorio;
                entity.CompromissoObrigatorio = request.CompromissoObrigatorio;
                entity.ObrigacaoObrigatorio = request.ObrigacaoObrigatorio;
                entity.PagamentoObrigatorio = request.PagamentoObrigatorio;
                entity.UtilizadorAlteracao = request.UserId;
                entity.DataAlteracao = DateTime.Now;

                _unitOfWork.AttachmentConfigRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        private static AttachmentConfigDataContract ToDataContract(AttachmentConfig entity)
        {
            return new AttachmentConfigDataContract
            {
                maxFileSizeMb = entity.MaxFileSizeMb,
                adObrigatorio = entity.AdObrigatorio,
                cabimentoObrigatorio = entity.CabimentoObrigatorio,
                compromissoObrigatorio = entity.CompromissoObrigatorio,
                obrigacaoObrigatorio = entity.ObrigacaoObrigatorio,
                pagamentoObrigatorio = entity.PagamentoObrigatorio
            };
        }
    }
}
