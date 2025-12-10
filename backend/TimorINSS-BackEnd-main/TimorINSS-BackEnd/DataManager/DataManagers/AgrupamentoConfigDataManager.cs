using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class AgrupamentoConfigDataManager : IAgrupamentoConfigDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public AgrupamentoConfigDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public AgrupamentoConfigReponse GetAgrupamentoConfigByIdCodigoContaTipoConta(GetAgrupamentoConfigRequest request)
        {
            AgrupamentoConfigReponse response = new AgrupamentoConfigReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações

            // vai buscar todos os agrupamentos registados na BD de acordo com o id's do Código Conta e do Tipo Conta passados no request
            //List<AgrupamentoConfigDataContract> relCodigoAgrupamento = _unitOfWork.RelCodigoContaAgrupamentoConfigRepository.GetAgrupamentoConfigByIdCodigoContaTipoConta(request.ContaCodigoFk, request.TipoContaFK);
            ComponenteorcamentoRegisto componenteOrcamentoRegisto = _unitOfWork.ComponenteOrcamentoRegistoRepository.Get(request.IdOrcamento);
            //if (relCodigoAgrupamento != null && relCodigoAgrupamento.Count > 0)
            //
            //    response.Agrupamentos = relCodigoAgrupamento;
            //}
            //else {
                // no caso de não devolver nada, vai buscar os agrupamentos só com o id tipo de conta
                //ps: os tipos de conta encontram-se na na tabela DOMINIO com o dominio="TIPOCONTA"
                //if (componenteOrcamentoRegisto != null) {
                    response.Agrupamentos = _unitOfWork.AgrupamentoConfigRepository.GetAlllActivAgrupamentoConfigByOrcamentoConfigTipoConta(componenteOrcamentoRegisto?.OrcamentoConfigFk == null ? request.IdOrcamento : componenteOrcamentoRegisto?.OrcamentoConfigFk, request.TipoContaFK);
                //}
            //}

            return response;
        }
    }
}