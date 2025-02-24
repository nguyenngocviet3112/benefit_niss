using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IResponsavelLegalDataManager
    {
        public ResponseBaseDataContract AddResponsavelLegal(ResponsavelLegalRequest request);

        public SingleResponsavelLegalResponse GetById(ResponsavelLegalListagemRequest request);

        public ResponsavelLegalListagemResponse GetByIdEntidadeEmpregadora(ResponsavelLegalListagemRequest request);

        public ResponseBaseDataContract UpdateResponsavelLegal(ResponsavelLegalRequest request);

        public ResponseBaseDataContract DeleteResponsavelLegal(ResponsavelLegalDeleteRequest request);
    }
}