using System;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class CicloDespesaDataManager : ICicloDespesaDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public CicloDespesaDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CicloDespesaListResponse GetByAno(GetCicloDespesaListRequest request)
        {
            var response = new CicloDespesaListResponse { RequestId = request.RequestId };

            try
            {
                response.items = _unitOfWork.CicloDespesaRepository.GetByAno(request.Ano, request.Institution);
            }
            catch (Exception e)
            {
                response.Errors.Add(new DataContracts.ResponseDataContract.Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            return response;
        }
    }
}
