using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ICodigoContaRepository : IDataRepository<Codigoconta, CodigoContaDto>
    {
        public ValueCampoEditavelListagemResponse GetAllActiveTipoDeContaNivel1(SearchFilter filter);

        public List<SelectDescription> GetAllCodigoDeContaFirstLevelByParent(int parent);

        public ValueCampoEditavelListagemResponse GetAllActiveTipoDeContaSubNivel(SearchFilter filter);

        public List<SelectDescription> GetAllCodigoDeContaByParent(int parent);

        public bool IsCodeValid(Codigoconta codigoConta);

        public List<CodigoContaDataContract> GetAllActivCodigoContaByOrcamentoConfig(int orcamentoId);

        public string GetFullCodigoTransactionless(int id, Dictionary<int, Codigoconta> codigosConta);

        public string GetFullCodigo(int id);

        public string GetFullDesignacao(int id);

        public bool IsCodigoContaInUse(Codigoconta codigoConta);

        public bool CodigoContaHasChilds(Codigoconta codigoConta);

        public List<CodigoContaDataContract> GetAllActiveCodigoConta();
    }
}