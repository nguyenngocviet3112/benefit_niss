using TimorINSSBackEnd.DataContracts.ResponseDataContract;


namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IKhachHangDataManager
    {
        public SelectKhachHangResponse GetAllKhachHang();

    }
}