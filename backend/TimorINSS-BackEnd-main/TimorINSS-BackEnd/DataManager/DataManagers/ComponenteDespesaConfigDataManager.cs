using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ComponenteDespesaConfigDataManager : IComponenteDespesaConfigDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ComponenteDespesaConfigDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public GetComponenteDespesaConfigReponse GetComponenteDespesaConfigByTarefaAtivoId(GetComponenteDespesaConfigRequest request)
        {
            GetComponenteDespesaConfigReponse response = new GetComponenteDespesaConfigReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            var tarefaativo = _unitOfWork.TarefaAtivoRepository.Get(request.TarefaAtivoId);

            if (tarefaativo != null)
            {
                var despesaConfig = _unitOfWork.ComponenteDespesaRepository.GetByIdTarefa(tarefaativo.TarefaconfigFk);

                if (despesaConfig != null)
                {
                    response.componenteDespesaConfig = BuildComponenteDespesaConfigDataContract(despesaConfig);
                }
            }
            return response;
        }

        private ComponenteDespesaDataContract BuildComponenteDespesaConfigDataContract(Componentedespesa componenteDespesa)
        {
            ComponenteDespesaDataContract componenteDespesaConfig = new ComponenteDespesaDataContract
            {
                id = componenteDespesa.Id,
                tarefaFk = componenteDespesa.TarefaFk,
                registarDespesa = componenteDespesa.RegistarDespesa,
                visualizarDespesaRParaA = componenteDespesa.VisualizarDespesaRparaA,
                visualizarDespesaAParaC = componenteDespesa.VisualizarDespesaAparaC,
                visualizarDespesaR = componenteDespesa.VisualizarDespesaR,
                visualizarDespesaA = componenteDespesa.VisualizarDespesaA,
                visualizarDespesaComCompromisso = componenteDespesa.VisualizarDespesaComCompromisso,
                visualiazarDespesaC = componenteDespesa.VisualiazarDespesaC,
                executarPagamentos = componenteDespesa.ExecutarPagamentos,
                visualizarExecucaoDespesaCabimentada = componenteDespesa.VisualizarExecucaoDespesaCabimentada,
                emitirOrdemPagamento = componenteDespesa.EmitirOrdemPagamento
            };
            return componenteDespesaConfig;
        }
    }
}