using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class TarefaDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string numero { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public bool indActivo { get; set; }
    }

    [DataContract]
    public class PreencherTarefa
    {
        [DataMember]
        public CabecalhoComponent cabecalho { get; set; }

        [DataMember]
        public int? classificacaoSubClass { get; set; }

        [DataMember]
        public TextosComponent textos { get; set; }

        [DataMember]
        public PrazoTarefaComponent prazoTarefa { get; set; }

        [DataMember]
        public bool hasArchive { get; set; }

        [DataMember]
        public int? nextTarefaNumber { get; set; }
    }

    public class CabecalhoComponent
    {
        [DataMember]
        public string numProcesso { get; set; }

        [DataMember]
        public string nomeProcesso { get; set; }

        [DataMember]
        public string nomeTarefa { get; set; }
    }

    public class TextosComponent
    {
        [DataMember]
        public bool isExpanded { get; set; }

        [DataMember]
        public string? textTitle { get; set; }

        [DataMember]
        public string text { get; set; }

        [DataMember]
        public int? charLimit { get; set; }

        [DataMember]
        public bool obrigatorio { get; set; }

        [DataMember]
        public bool? obrigatorioAoArquivar { get; set; }

        [DataMember]
        public bool isExpanded2 { get; set; }

        [DataMember]
        public string? textTitle2 { get; set; }

        [DataMember]
        public string text2 { get; set; }

        [DataMember]
        public int? charLimit2 { get; set; }

        [DataMember]
        public bool obrigatorio2 { get; set; }

        [DataMember]
        public bool? obrigatorioAoArquivar2 { get; set; }

        [DataMember]
        public bool? hasArquivar { get; set; }
    }

    public class PrazoTarefaComponent
    {
        [DataMember]
        public int deadline { get; set; }

        [DataMember]
        public DateTime limitDate { get; set; }
    }
}