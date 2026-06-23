using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class EscalaoDataManager : IEscalaoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public EscalaoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public SelectDescriptionResponse GetAllEscaloes()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            List<SelectDescription> dropdowns = _unitOfWork.EscalaoRepository.GetAllEscaloes();
            response.selects = dropdowns;

            return response;
        }
    }
}