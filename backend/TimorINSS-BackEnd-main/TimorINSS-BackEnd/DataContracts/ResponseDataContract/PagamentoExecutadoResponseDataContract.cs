using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class DestinatarioPagamentoExecutadosResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<PagamentoExecutadoDestinatrioDataContract> DestinatarioPagamentosExecutados;

        [DataMember]
        public string ExcelExtraido;
    }

    [DataContract]
    public class RelatorioPagamentosListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<RelatorioPagamentoDataContract> pagamentos;
    }

    [DataContract]
    public class ListagemPagamentosProcessoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ListaPagamentosDoProcessoDataContract> Pagamentos;

    }

    [DataContract]
    public class ClassificacaoContabilisticaRelatorios : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<RelatorioClassificacaoContabilisticaDataContract> movimentos;
    }

    [DataContract]
    public class FornecedoresRelatorios : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<RelatorioFornecedoresDataContract> movimentos;
    }

    [DataContract]
    public class BalancoRelatorios : ResponseBaseDataContract
    {
        [DataMember]
        public List<RelatorioBalancoDataContract> lista;
    }
}