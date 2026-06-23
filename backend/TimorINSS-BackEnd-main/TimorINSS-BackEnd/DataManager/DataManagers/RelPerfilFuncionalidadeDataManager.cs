using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class RelPerfilFuncionalidadeDataManager : IRelPerfilFuncionalidadeDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public RelPerfilFuncionalidadeDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public FuncionalidadeListagemResponse GetRelPerfilFuncionalidadeByIdPerfil(int idPerfil)
        {
            FuncionalidadeListagemResponse response = new FuncionalidadeListagemResponse();
            try
            {
                response.perfilFuncionalidade = _unitOfWork.RelPerfilFuncionalidadeRepository.GetRelPerfilFuncionalidadeByIdPerfil(idPerfil);
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