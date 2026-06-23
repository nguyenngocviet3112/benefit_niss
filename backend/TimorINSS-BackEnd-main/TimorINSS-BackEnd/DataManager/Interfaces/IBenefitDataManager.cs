using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;


namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IBenefitDataManager
    {
        public TrabalhadorViewResponse GetSingleByNiss(TrabalhadorListagemNissRequest request);

    }
}