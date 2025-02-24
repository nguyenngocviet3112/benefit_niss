using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SavePagamentoExecutadoRequest : RequestBaseDataContract
    {
        [DataMember]
        public DestinatarioDataContract destinatario;

        [DataMember]
        public PagamentoExecutadoDataContract pagamento;
        
        [DataMember]
        public Guid? importId;
    }

    [DataContract]
    public class GetDestinatarioPagamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public string NumPagamento;
    }

    [DataContract]
    public class DeletePagamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id;
    }

    [DataContract]
    public class GetPagamentoExecutadoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int IdDestinatario;
    }

    [DataContract]
    public class EditPagamentoExecutadoRequest : RequestBaseDataContract
    {
        [DataMember]
        public List<int> listaIdPagamento;
    }

    [DataContract]
    public class ListagemPagamentosProcessoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ProcessoAtivo;
    }

    [DataContract]
    public class FornecedoresRelatoriosRequest : SearchFilterRequest
    {
        [DataMember]
        public string Niss;

        [DataMember]
        public string Tin;
    }

    [DataContract]
    public class BalancoRelatoriosRequest : SearchFilterRequest
    {
        [DataMember]
        public int Nivel;

        [DataMember]
        public bool CompararAnoAnterior;
    }

    [DataContract]
    public class SaveClassificacaoContabilisticaExecucaoRequest : RequestBaseDataContract
    {
        [DataMember]
        public List<int> DespesasIds;

        [DataMember]
        public int TarefaAtivoId;

        [DataMember]
        public ClassificacaoContabilisticaExecucao Classificacao;
    }
}