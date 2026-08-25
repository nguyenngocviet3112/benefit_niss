using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class ConfigurarTarefaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idTarefa { get; set; }

        [DataMember(IsRequired = true)]
        public string nomeTarefa { get; set; }

        [DataMember]
        public int prazoTarefa { get; set; }

        [DataMember]
        public bool listaTexto { get; set; }

        [DataMember]
        public bool listaDocumento { get; set; }

        [DataMember]
        public ComponenteTexto componenteTexto { get; set; }

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
        public ComponenteOrcamentoDataContract? componenteOrcamento { get; set; }

        [DataMember]
        public ComponenteDespesaDataContract? componenteDespesa { get; set; }

        [DataMember]
        public ComponenteConciliacaoMovimentosDataContract? componenteConciliacaoMovimentos { get; set; }

        [DataMember]
        public ComponenteReceitaDataContract? componenteReceita { get; set; }
    }

    public class TarefaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idTarefa { get; set; }

        [DataMember]
        public string nomeTarefa { get; set; }

        [DataMember]
        public bool listaTexto { get; set; }

        [DataMember]
        public bool listaDocumento { get; set; }

        [DataMember]
        public bool componenteCabecalhoProcesso { get; set; }

        [DataMember]
        public bool componenteBotaoArquivar { get; set; }
    }

    [DataContract]
    public class ComponenteTexto : RequestBaseDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public bool expandir1 { get; set; }

        [DataMember]
        public bool expandir2 { get; set; }

        [DataMember]
        public bool texto1 { get; set; }

        [DataMember]
        public bool texto2 { get; set; }

        [DataMember]
        public string titulo1 { get; set; }

        [DataMember]
        public string titulo2 { get; set; }

        [DataMember]
        public int quantCaracteres1 { get; set; }

        [DataMember]
        public int quantCaracteres2 { get; set; }

        [DataMember]
        public bool obrigatorio1 { get; set; }

        [DataMember]
        public bool obrigatorio2 { get; set; }

        [DataMember]
        public bool obrigatorioArquivar1 { get; set; }

        [DataMember]
        public bool obrigatorioArquivar2 { get; set; }
    }

    [DataContract]
    public class ComponenteAccoesTarefa : RequestBaseDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int idTarefa { get; set; }

        [DataMember]
        public string nomeTarefa { get; set; }

        [DataMember]
        public string apelidoTarefa { get; set; }
    }

    [DataContract]
    public class ComponenteClassificacaoSubClassificTarefa : RequestBaseDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int idClassificacao { get; set; }

        [DataMember]
        public string nomeClassificacao { get; set; }

        [DataMember]
        public int idSubClassificacao { get; set; }

        [DataMember]
        public string nomeSubClassificacao { get; set; }
    }

    [DataContract]
    public class ComponenteAcessoPerfil : RequestBaseDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int idPerfil { get; set; }

        [DataMember]
        public string nomePerfil { get; set; }
    }

    [DataContract]
    public class ComponenteAcessoUtilizador : RequestBaseDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int idUtilizador { get; set; }

        [DataMember]
        public string nomeUtilizador { get; set; }
    }

    [DataContract]
    public class ComponenteDocumentoTarefa : RequestBaseDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int idDocumento { get; set; }

        [DataMember]
        public string nomeDocumento { get; set; }

        [DataMember]
        public bool obrigatorio { get; set; }
    }

    [DataContract]
    public class SwitchTarefaAtivoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int id { get; set; }
    }

    [DataContract]
    public class GetHistoricoTextoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int tarefaAtivoId { get; set; }
    }

    [DataContract]
    public class GetAllTarefasASeguirRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int tarefaAtivoId { get; set; }
    }

    [DataContract]
    public class GetTarefaDataRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int tarefaAtivoId { get; set; }
    }

    [DataContract]
    public class SaveTarefaDataRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int tarefaAtivoId { get; set; }

        [DataMember(IsRequired = true)]
        public PreencherTarefa data { get; set; }
    }

    [DataContract]
    public class SaveTituloListaPagamentoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int tarefaAtivoId { get; set; }

        [DataMember(IsRequired = true)]
        public string titulo { get; set; }
    }
}