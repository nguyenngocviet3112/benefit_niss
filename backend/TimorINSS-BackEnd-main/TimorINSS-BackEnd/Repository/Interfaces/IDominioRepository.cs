using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IDominioRepository : IDataRepository<Dominio, DominioDto>
    {
        public List<DominioDescricaoString> getAllTiposDeDominio(TiposDominio tipo);

        public DominioDescricaoString getTipoDeDominio(TiposDominio tipo);

        public int getIdDominio(string dominio, int valor);

        public List<int> getIdDominios(string dominio, List<int> valores);

        public Dominio getDominioByDescricao(TiposDominio tipo, string descricao);

        public ValueCampoEditavelListagemResponse getAllActiveTiposDeDominio(TiposDominio tipo, SearchFilter filter, bool valor);

        public int getNextValue(string dominio);

        public List<SelectDescription> getAllActiveTiposDeDominio(TiposDominio tipo);
    }
}