using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IKhachHangRepository : IDataRepository<KhachHang, KhachHangDto>
    {
        public List<SelectKhachHang> GetAllKhachHang();

        //public ValueCampoEditavelListagemResponse GetAllActiveActividadeEconomica(SearchFilter filter);

        //public bool DoesCodeExists(int id, string code);
    }
}