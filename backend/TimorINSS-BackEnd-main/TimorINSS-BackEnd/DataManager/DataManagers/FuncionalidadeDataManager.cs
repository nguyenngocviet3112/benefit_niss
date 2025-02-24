using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class FuncionalidadeDataManager : IFuncionalidadeDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public FuncionalidadeDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public FuncionalidadeListagemResponse GetAllFuncionalidades()
        {
            // vai buscar todas as funcionalidades do módulo interno que existe
            FuncionalidadeListagemResponse response = new FuncionalidadeListagemResponse();
            try
            {
                response.funcionalidade = _unitOfWork.FuncionalidadeRepository.GetAllFuncionalidades();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }
    }
}