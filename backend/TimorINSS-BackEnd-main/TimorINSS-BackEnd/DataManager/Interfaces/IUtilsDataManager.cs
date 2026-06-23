using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IUtilsDataManager
    {
        public T SetDetailsToEntity<T>(T entity);

        public T UpdateDetailsToEntity<T>(T entity);

        public bool IsSuspenso(int idEntidade, int idTrabalhador, IUnitOfWork unitOfWork);

        public bool ValidateRelentidadetrabalhador(Relentidadetrabalhador relEntidadeTrabalhador, IUnitOfWork unitOfWork, out List<Error> error);

        public LogResponse Compress(RequestBaseDataContract request);

        public bool ValidatePermission(int userId, int funcionalidadeId, IUnitOfWork _unitOfWork, CRUD? permissionType = null);

        public string CreateHashPassword(string password, string userSalt, string internalSalt);

        public string GetIPV6();
    }
}