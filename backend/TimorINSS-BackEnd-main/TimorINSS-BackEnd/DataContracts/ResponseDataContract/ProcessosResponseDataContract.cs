using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ProcessosListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<ProcessoDataContract> processos;
    }

    [DataContract]
    public class ProcessoConfigListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int Id;

        [DataMember]
        public string Nome;

        [DataMember]
        public List<RelProcessoConfigTarefaDataContract> Tarefas;

        [DataMember]
        public List<int> Perfis;
    }

    [DataContract]
    public class ProcessosArquivadosListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<ProcessoArquivadoDataContract> processos;
    }

    [DataContract]
    public class ProcessoDataResponse : ResponseBaseDataContract
    {
        [DataMember]
        public ProcessoDetalhe data;
    }

    [DataContract]
    public class RelatorioProcessosListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<RelatorioProcessoDataContract> processos;
    }

    [DataContract]
    public class GetTipoProcessosRelatoriosResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<RelatorioTipoProcessoDataContract> tipoProcessos;
    }
}