using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ICeInssGlobalRepository
    {
        CeInssGlobalResponse GetReport(CeInssGlobalRequest request);

        // Chỉ 2 tổng (Orçamento aprovado, Despesa thực chi) — dùng cho Dashboard, không build
        // toàn bộ breakdown theo Classificação Económica như GetReport (tránh tính thừa
        // Receita PAC/GP/Cabimentos/Compromissos khi chỉ cần 2 con số).
        (decimal Orcamento, decimal Executado) GetTotaisDespesa(int year, int? institution);
    }
}
