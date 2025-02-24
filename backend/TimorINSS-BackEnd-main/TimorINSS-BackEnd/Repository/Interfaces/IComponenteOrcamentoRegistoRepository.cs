using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteOrcamentoRegistoRepository : IDataRepository<ComponenteorcamentoRegisto, ComponenteOrcamentoRegistoDto>
    {
        public ComponenteorcamentoRegisto GetByIdTarefaActivo(int id, bool checkInactive = true);

        public ComponenteorcamentoRegisto GetOrcamentoAprovadoByDataPInicioProcesso(DateTime dataInicioProcesso);

        public List<ComponenteorcamentoRegisto> GetComponentesOrcamentoRegistoAprovado(DateTime inicio, DateTime fim);

        public ComponenteorcamentoRegisto GetByIdProcessoActivo(int idProcessoAtivo);
    }
}