using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IGuiaPagamentoContaConfigRepository
    {
        GuiaPagamentoContaConfig GetActive();
        void Add(GuiaPagamentoContaConfig entity);
        void Update(GuiaPagamentoContaConfig entity);
    }
}
