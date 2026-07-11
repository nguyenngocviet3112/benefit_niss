namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IUserModeAccessDataManager
    {
        bool HasAccess(int utilizadorFk);
    }
}
