using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteDocumentosRegistoRepository : IDataRepository<ComponentedocumentoRegisto, ComponentedocumentoRegistoDto>
    {
        public DocumentosTarefaListagemResponse GetListagemByIdProcesso(DocumentosListagemRequest request);

        public List<ComponentedocumentoRegisto> GetByTarefaIdAndType(long tarefaId, long docType);
    }
}