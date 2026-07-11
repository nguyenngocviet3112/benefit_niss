using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class UserModeAccessRepository : IUserModeAccessRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public UserModeAccessRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public bool HasAccess(int utilizadorFk)
        {
            return _moduloContribuicoesContext.UserModeAccess
                .Any(a => a.UtilizadorFk == utilizadorFk && a.IndActivo);
        }
    }
}
