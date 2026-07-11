namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IUserModeAccessRepository
    {
        bool HasAccess(int utilizadorFk);
    }
}
