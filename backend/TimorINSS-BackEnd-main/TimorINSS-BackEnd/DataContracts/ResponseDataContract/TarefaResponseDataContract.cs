using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class TarefaListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<TarefaDataContract> tarefa;
    }

    public class ComponenteTarefaConfiguradaRespose : ResponseBaseDataContract
    {
        [DataMember]
        public string nomeTarefa { get; set; }

        [DataMember]
        public int prazoTarefa { get; set; }

        [DataMember]
        public bool listaTexto { get; set; }

        [DataMember]
        public bool listaDocumento { get; set; }

        [DataMember]
        public ComponenteTexto? componenteTexto { get; set; }

        [DataMember]
        public bool componenteCabecalhoProcesso { get; set; }

        [DataMember]
        public List<ComponenteAccoesTarefa> componenteAccoesTarefa { get; set; }

        [DataMember]
        public List<ComponenteClassificacaoSubClassificTarefa> componenteClassificacaoSubClassific { get; set; }

        [DataMember]
        public List<ComponenteAcessoPerfil> componenteControleAcessoPerfil { get; set; }

        [DataMember]
        public List<ComponenteAcessoUtilizador> componenteControleAcessoUtilizador { get; set; }

        [DataMember]
        public List<ComponenteDocumentoTarefa> componenteCarregarDocumentos { get; set; }

        [DataMember]
        public List<Componentes> listaComponente { get; set; }

        [DataMember]
        public bool componenteBotaoArquivar { get; set; }

        [DataMember]
        public ComponenteOrcamentoDataContract? ComponenteOrcamento { get; set; }

        [DataMember]
        public ComponenteDespesaDataContract? componenteDespesa { get; set; }

        [DataMember]
        public ComponenteConciliacaoMovimentosDataContract? componenteConciliacaoMovimentos { get; set; }

        [DataMember]
        public ComponenteReceitaDataContract? componenteReceita { get; set; }
    }

    [DataContract]
    public class TarefasAtivasListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<TarefaAtivoDataContract> tarefas;
    }

    [DataContract]
    public class TarefaDataResponse : ResponseBaseDataContract
    {
        [DataMember]
        public PreencherTarefa data;
    }
}