using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class AldeiaDataManager : IAldeiaDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public AldeiaDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SelectDescriptionResponse getAllAldeia()
        {
            // vai buscar todas as aldeias ativas(indAtivo = 1) existentes na Base de dados
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> dropdowns = _unitOfWork.AldeiaRepository.getAllAldeia();
                response.selects = dropdowns;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public AldeiaDto GetDto(int id)
        {
            return _unitOfWork.AldeiaRepository.GetDto(id);
        }

        public SelectDescriptionResponse getAldeiaByIdSuco(int id)
        {
            // as aldeias existem dentro de um Suco, ou seja, todas as Aldeias registadas na BD estão associadas 
            //a um suco( tabela Aldeia, campo aldeia_suco_fk)
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                // vai buscar todas as aldeias na bd associadas ao id do suco passado como parametro
                List<SelectDescription> dropdowns = _unitOfWork.AldeiaRepository.getAldeiaByIdSuco(id);
                response.selects = dropdowns;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }
    }
}