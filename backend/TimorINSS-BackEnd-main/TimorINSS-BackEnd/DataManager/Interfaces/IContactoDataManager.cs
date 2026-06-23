using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IContactoDataManager
    {
        public ContactoDto GetDto(int id);

        public ContatoListagemResponse GetContatosByIdEntidadeEmpregadora(ContatoListagemRequest request);

        public ContatoListagemResponse GetContatosByIdTrabalhador(ContatoListagemRequest request);

        public ContatoListagemResponse SaveContato(ContatoRequest contato);

        public ContatoListagemResponse UpdateContato(ContatoRequest contato);

        public ResponseBaseDataContract DeleteContato(ContatoDeleteRequest request);
    }
}