using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class MovimentosListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<MovimentosData> movimentos;
    }

    [DataContract]
    public class ContasBancariasListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ContaBancariaDto> contas;
    }

    [DataContract]
    public class SaldoMovimentosResponse : ResponseBaseDataContract
    {
        [DataMember]
        public decimal SaldoConciliadoCredit;

        [DataMember]
        public decimal SaldoConciliadoDebit;

        [DataMember]
        public decimal SaldoConciliadoTotal;

        [DataMember]
        public decimal SaldoPorConciliarCredit;

        [DataMember]
        public decimal SaldoPorConciliarDebit;

        [DataMember]
        public decimal SaldoPorConciliarTotal;

        [DataMember]
        public decimal SaldoTotalCredit;

        [DataMember]
        public decimal SaldoTotalDebit;

        [DataMember]
        public decimal SaldoTotal;
    }

    [DataContract]
    public class ConciliarMovimentosPermissionsListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int selectMovimentosTypePermission;

        [DataMember]
        public int addEditMovimentosPermission;

        [DataMember]
        public int addEditMovimentosBancariosPermission;

        [DataMember]
        public int viewSelectedToConciliatePermission;

        [DataMember]
        public int conciliatePermission;

        [DataMember]
        public int undoConciliationPermission;
    }
}