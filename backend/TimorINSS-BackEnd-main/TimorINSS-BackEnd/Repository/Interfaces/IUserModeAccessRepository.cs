using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IUserModeAccessRepository
    {
        bool HasAccess(int utilizadorFk);

        // Dùng cho màn "Đồng bộ User từ hệ thống cũ" — biết user nào đã có
        // cổng vào mode mới rồi để không hiện lại là "chưa đồng bộ".
        List<int> GetActiveUtilizadorFks();

        void Add(UserModeAccess entity);
    }
}
