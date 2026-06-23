using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IDestinatarioDataManager
    {
        public GetDestinatarioResponse GetDestinatarioByNissTin(GetDestinatarioRequest request);

        public ResponseBaseDataContract SaveDestinatario(SaveDestinatarioRequest request);
        public ExcelImportReponse ImportDestinatarios(ExcelImporterRequest request);
    }
}