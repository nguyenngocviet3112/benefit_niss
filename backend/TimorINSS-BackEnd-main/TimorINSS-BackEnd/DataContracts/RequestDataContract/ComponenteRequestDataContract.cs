using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    public class ComponenteTextoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idTarefa { get; set; }

        [DataMember]
        public ComponenteTexto componenteTexto { get; set; }
    }

    public class ComponentePrazoTarefaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idTarefa { get; set; }

        [DataMember]
        public int prazoTarefa { get; set; }
    }

    public class ComponenteAccaoTarefaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idTarefa { get; set; }

        [DataMember]
        public List<ComponenteAccoesTarefa> accaoTarefa { get; set; }
    }

    public class ComponenteDocumentoTarefaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idTarefa { get; set; }

        [DataMember]
        public List<ComponenteDocumentoTarefa> documentoTarefa { get; set; }
    }

    public class ComponenteClassificacaoSubClassificTarefaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idTarefa { get; set; }

        [DataMember]
        public List<ComponenteClassificacaoSubClassificTarefa> classificacaoSubClassific { get; set; }
    }

    public class ComponenteControloAcessoPerfilTarefaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idTarefa { get; set; }

        [DataMember]
        public List<ComponenteAcessoPerfil> controloAcessoPerfil { get; set; }
    }

    public class ComponenteControloAcessoUtilizadorTarefaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idTarefa { get; set; }

        [DataMember]
        public List<ComponenteAcessoUtilizador> controloAcessoUtilizador { get; set; }
    }

    public class ComponenteOrcamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public ComponenteOrcamentoDataContract ComponenteOrcamento { get; set; }
    }

    public class ComponenteDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public ComponenteDespesaDataContract ComponenteDespesa { get; set; }
    }

    public class ComponenteConciliacaoMovimentosRequest : RequestBaseDataContract
    {
        [DataMember]
        public ComponenteConciliacaoMovimentosDataContract ComponenteConciliacaoMovimentos { get; set; }
    }

    public class ComponenteReceitaRequest : RequestBaseDataContract
    {
        [DataMember]
        public ComponenteReceitaDataContract ComponenteReceita { get; set; }
    }
}