using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IUtilizadorDataManager
    {
        public IEnumerable<Utilizador> GetAll();

        public Utilizador Get(long id);

        public UtilizadorDto GetDto(long id);

        public void Add(Utilizador entity);

        public void Update(Utilizador entity);

        public void Delete(Utilizador entity);

        public LoginResponse LoginManager(LoginRequest request);

        public ResponseBaseDataContract RecoverManager(RecoverRequest request);

        public ResponseBaseDataContract FirstAcessManager(RecoverRequest request);

        public ResponseBaseDataContract SetUpPasswordManager(RecoverSetPasswordRequest request);

        public ResponseBaseDataContract CreateUserManager(RecoverSetPasswordRequest request);

        public LoginResponse InternalLoginManager(LoginRequest request);

        public UtilizadorListagemResponse GetAllUtilizadoresInterno(SearchFilterRequest request);

        public DadosUtilizadorResponse GetAllDadosUtilizador(DadosUtilizadorRequest request);

        public ResponseBaseDataContract AddUtilizador(UtilizadorRequest request);

        public ResponseBaseDataContract InternalFirstAcessManager(RecoverRequest request);

        public ResponseBaseDataContract InternalRecoverManager(RecoverRequest request);

        public ResponseBaseDataContract CreateInternalUserManager(RecoverSetPasswordRequest request);

        public UtilizadoresAcessoListagemResponse GetAllAcessoUtilizadores(SearchFilterRequest request);

        public ResponseBaseDataContract SwitchUserBlockState(UserUpdateRequest request);
        public ResponseBaseDataContract ValidToken(ValidTokenRequest request);
    }
}