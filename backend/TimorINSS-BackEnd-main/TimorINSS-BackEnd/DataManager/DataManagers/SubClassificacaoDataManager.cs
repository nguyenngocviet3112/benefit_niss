using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class SubClassificacaoDataManager : ISubClassificacaoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public SubClassificacaoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public SelectDescriptionResponse GetAllSubClassificacao()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> selects = _unitOfWork.SubClassificacaoRepository.GetAllSubClassificacao();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public SelectDescriptionResponse GetAllSubClassificacaoByTarefaAtivaId(GetAllSubClassificacaoByTarefaAtivaIdRequest request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                var idTarefa = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.tarefaAtivoId).TarefaconfigFk;
                var subClassificacaoList = _unitOfWork.ComponenteClassificacaoSubClassificRepository.GetByIdTarefa(idTarefa);

                List<SelectDescription> selects = subClassificacaoList.Select(item => new SelectDescription
                {
                    id = item.idSubClassificacao,
                    nome = item.nomeClassificacao + " / " + item.nomeSubClassificacao,
                    indActivo = true
                }).ToList();

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