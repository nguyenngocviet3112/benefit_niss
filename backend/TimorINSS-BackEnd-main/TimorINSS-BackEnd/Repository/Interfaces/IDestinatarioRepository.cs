using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IDestinatarioRepository : IDataRepository<Destinatario, DestinatarioDto>
    {
        public DestinatarioDataContract GetDestinatarioByNiss(string niss);

        public DestinatarioDataContract GetDestinatarioByTin(string tin);

        public Destinatario GetDestinatarioByIdEntidade(int idEntidade);

        public List<Destinatario> GetDestinatarioByNissOrTin(List<string> niss, List<string> tin);

        public Destinatario GetDestinatarioByIdTrabalhador(int idTrabalhador);

        public List<ExcelImporterRel> GetImportRows(Guid importId);
    }
}