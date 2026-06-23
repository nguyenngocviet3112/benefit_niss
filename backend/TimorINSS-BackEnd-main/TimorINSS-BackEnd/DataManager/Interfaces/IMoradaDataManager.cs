using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IMoradaDataManager
    {
        public IEnumerable<Morada> GetAll();

        public Morada Get(long id);

        public MoradaDto GetDto(long id);

        public void Add(Morada entity);

        public void Update(Morada entity);

        public void Delete(Morada entity);

        public MoradaListagemResponse GetMoradasByIdEntidadeEmpregadora(MoradaListagemRequest request);

        public MoradaListagemResponse GetMoradasByIdTrabalhador(MoradaListagemRequest request);

        public MoradaListagemResponse SaveMorada(MoradaRequest morada);

        public MoradaListagemResponse UpdateMorada(MoradaRequest morada);

        public ResponseBaseDataContract DeleteMorada(MoradaDeleteRequest request);
    }
}