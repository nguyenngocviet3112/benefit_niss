using System;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class IntegrationConfigDataManager : IIntegrationConfigDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public IntegrationConfigDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IntegrationConfigResponse Get()
        {
            var response = new IntegrationConfigResponse();
            try
            {
                var config = _unitOfWork.IntegrationConfigRepository.Get();
                response.item = ToDataContract(config);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Save(SaveIntegrationConfigRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                var entity = _unitOfWork.IntegrationConfigRepository.Get();
                entity.BenefitApiEnabled = request.BenefitApiEnabled;
                entity.UtilizadorAlteracao = request.UserId;
                entity.DataAlteracao = DateTime.Now;

                _unitOfWork.IntegrationConfigRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public bool IsBenefitApiEnabled()
        {
            return _unitOfWork.IntegrationConfigRepository.Get().BenefitApiEnabled;
        }

        private static IntegrationConfigDataContract ToDataContract(IntegrationConfig entity)
        {
            return new IntegrationConfigDataContract
            {
                benefitApiEnabled = entity.BenefitApiEnabled
            };
        }
    }
}
