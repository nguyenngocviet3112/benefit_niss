using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ClassificacaoDataManager : IClassificacaoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ClassificacaoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public SelectDescriptionResponse GetAllClassificacao()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                // As Classificações de uma tarefa são registadas na BD na funcionalidade de gestão dos campos editáveis
                //vai buscar todas as classificações ativas registadas no sistema
                List<SelectDescription> selects = _unitOfWork.ClassificacaoRepository.GetAllClassificacao();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }
    }
}