using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GetComponenteOrcamentoRegistoReponse : ResponseBaseDataContract
    {
        [DataMember]
        public ComponenteOrcamentoRegistoDataContract ComponenteOrcamentoRegisto;

        [DataMember]
        public List<AgrupamentoConfigDataContract> Agrupamentos { get; set; }

        [DataMember]
        public List<AgrupamentoConfigDataContract> Actidades { get; set; }

        //[DataMember]
        //public List<AgrupamentoConfigDataContract> Economics { get; set; }

        [DataMember]
        public List<AgrupamentoConfigDataContract> Functionals { get; set; }

        [DataMember]
        public List<SelectDescription> CentrosCusto { get; set; }

        [DataMember]
        public List<DominioDescricaoString> TiposDeConta { get; set; }

        [DataMember]
        public List<ComponenteOrcamentoValorFullDataContract> ValoresCorrentes { get; set; }
    }

    [DataContract]
    public class UpdateComponenteOrcamentoRegistoDatesResponse : ResponseBaseDataContract
    {
        [DataMember]
        public bool UpdateValues { get; set; }

        [DataMember]
        public List<AgrupamentoConfigDataContract> Agrupamentos { get; set; }

        [DataMember]
        public List<SelectDescription> CentrosCusto { get; set; }

        [DataMember]
        public List<DominioDescricaoString> TiposDeConta { get; set; }
    }

    [DataContract]
    public class GetComponenteOrcamentoAprovadoRegistoReponse : ResponseBaseDataContract
    {
        [DataMember]
        public int IdOrcamentoRegisto { get; set; }

        [DataMember]
        public List<SelectDescription> CentrosCusto { get; set; }

        [DataMember]
        public List<DominioDescricaoString> TiposDeConta { get; set; }

        [DataMember]
        public List<CodigoContaDataContract> CodigoConta { get; set; }

        [DataMember]
        public bool existeOrcamentoAprovado { get; set; }

        [DataMember]
        public string NumPagamento { get; set; }

        [DataMember]
        public List<PagamentoExecutadoDestinatrioDataContract> ListaPagamentosDestinatario { get; set; }

        [DataMember]
        public int ProcessoId { get; set; }
    }

    [DataContract]
    public class OrcamentoExtractToExcelReponse : ResponseBaseDataContract
    {
        [DataMember]
        public string ExcelExtraido { get; set; }
    }

    [DataContract]
    public class OrcamentoExtractToPDFReponse : ResponseBaseDataContract
    {
        [DataMember]
        public string PDFExtraido { get; set; }
    }
}