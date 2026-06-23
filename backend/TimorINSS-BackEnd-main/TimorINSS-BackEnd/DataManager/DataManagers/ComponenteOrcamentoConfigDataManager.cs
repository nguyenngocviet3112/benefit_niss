using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ComponenteOrcamentoConfigDataManager : IComponenteOrcamentoConfigDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ComponenteOrcamentoConfigDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public GetComponenteOrcamentoConfigReponse GetComponenteOrcamentoConfigByTarefaAtivoId(GetComponenteOrcamentoConfigRequest request)

        {
            GetComponenteOrcamentoConfigReponse response = new GetComponenteOrcamentoConfigReponse();

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
                var orcamentoConfig = _unitOfWork.ComponenteOrcamentoRepository.GetByIdTarefa(tarefaativo.TarefaconfigFk);

                if (orcamentoConfig != null)
                {
                    response.componenteOrcamentoConfig = BuildComponenteOrcamentoConfigDataContract(orcamentoConfig);
                }
            }
            return response;
        }

        private ComponenteOrcamentoDataContract BuildComponenteOrcamentoConfigDataContract(Componenteorcamento componenteOrcamento)
        {
            ComponenteOrcamentoDataContract componenteOrcamentoConfig = new ComponenteOrcamentoDataContract
            {
                Id = componenteOrcamento.Id,
                TarefaFk = componenteOrcamento.TarefaFk,
                PermissaoDatas = componenteOrcamento.PermissaoDatas,
                PermissaoInsercoes = componenteOrcamento.PermissaoInsercoes,
                PermissaoDetalhes = componenteOrcamento.PermissaoDetalhes,
                PermissaoAprovacao = componenteOrcamento.PermissaoAprovacao,
            };
            return componenteOrcamentoConfig;
        }
    }
}