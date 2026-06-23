using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ComponenteReceitaConfigDataManager : IComponenteReceitaConfigDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ComponenteReceitaConfigDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public GetComponenteReceitaConfigReponse GetComponenteReceitaConfigByTarefaAtivoId(GetComponenteReceitaConfigRequest request)
        {
            GetComponenteReceitaConfigReponse response = new GetComponenteReceitaConfigReponse();

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
                // vai buscar as configurações do componente de receita para uma determinada tarefa( tabela - COMPONENTERECEITA)
                var receitaConfig = _unitOfWork.ComponenteReceitaRepository.GetByIdTarefa(tarefaativo.TarefaconfigFk);

                if (receitaConfig != null)
                {
                    response.componenteReceitaConfig = BuildComponenteReceitaConfigDataContract(receitaConfig);
                }
            }
            return response;
        }

        private ComponenteReceitaDataContract BuildComponenteReceitaConfigDataContract(Componentereceita componenteReceita)
        {
            ComponenteReceitaDataContract componenteReceitaConfig = new ComponenteReceitaDataContract
            {
                id = componenteReceita.Id,
                tarefaFk = componenteReceita.TarefaFk,
                classificarMovSelecionados = componenteReceita.ClassificarMovSelecionados,
                selecionarMovRecebidosParaRegisto = componenteReceita.SelecionarMovRecebidosParaRegisto,
                verificarExecucaoOrcamentoEditarSelecao = componenteReceita.VerificarExecucaoOrcamentoEditarSelecao,
            };
            return componenteReceitaConfig;
        }
    }
}