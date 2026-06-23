using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ComponenteDespesaRegistoDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int IdOrcamentoRegistoAprovado { get; set; }

        [DataMember]
        public int TarefaAtivoFK { get; set; }

        [DataMember]
        public int? DepartamentoFk { get; set; }

        [DataMember]
        public int CentroCustoFk { get; set; }

        [DataMember]
        public int TipoContaFk { get; set; }

        [DataMember]
        public int CodigoContaFk { get; set; }

        [DataMember]
        public int AgrupamentoConfigFk { get; set; }

        [DataMember]
        public int InstitutionId { get; set; }

        [DataMember]
        public int ActidadeFk { get; set; }

        [DataMember]
        public int FuncionalFk { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Valor { get; set; }
    }

    [DataContract]
    public class DespesaRegistadaDataContract
    {
        [DataMember]
        public int Id { get; set; }

        //[DataMember]
        //public int IdContabilidade { get; set; }

        //[DataMember]
        //public string codigoContabilidade { get; set; }

        //[DataMember]
        //public string descricaoContabilidade { get; set; }

        [DataMember]
        public IEnumerable<int> compromissos { get; set; }

        [DataMember]
        public int idOrcamento { get; set; }

        [DataMember]
        public string codigoOrcamento { get; set; }

        [DataMember]
        public string descricaoOrcamento { get; set; }

        [DataMember]
        public string descricaoDespesa { get; set; }

        [DataMember]
        public decimal valorRegistado { get; set; }

        [DataMember]
        public string estado { get; set; }

        [DataMember]
        public int? idDepartamento { get; set; }

        [DataMember]
        public int idCentroCusto { get; set; }

        [DataMember]
        public int idTipoConta { get; set; }
        [DataMember]
        public int? idInstitution { get; set; }
        [DataMember]
        public int? idActidade { get; set; }
        //[DataMember]
        //public int? idEconomic { get; set; }
        [DataMember]
        public int? idFuncional { get; set; }
    }

    [DataContract]
    public class ValoresDespesaRegistadaDataContract
    {
        [DataMember]
        public decimal ValorOrcamentado { get; set; }

        [DataMember]
        public decimal ValorExecutado { get; set; }

        [DataMember]
        public decimal ValorCabimentado { get; set; }

        [DataMember]
        public decimal ValorAutorizado { get; set; }
    }

    public class DespesaCabimentadasParaExecucaoDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string DescricaoDespesa { get; set; }

        [DataMember]
        public decimal ValorCabimentado { get; set; }

        [DataMember]
        public decimal ValorExecutado { get; set; }

        [DataMember]
        public decimal FaltaExecutar { get; set; }
    }

    public class DespesasRelatoriosDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string DepartamentoINSS { get; set; }

        [DataMember]
        public string CentroCusto { get; set; }

        [DataMember]
        public string TipoConta { get; set; }

        [DataMember]
        public Agrupamentoconfig _ContaOSS
        {
            set
            {
                string conta = value.Codigo + " - " + value.Designacao;
                Agrupamentoconfig current = value.ParentFkNavigation;

                while (current != null)
                {
                    conta = current.Codigo + conta;
                    current = current.ParentFkNavigation;
                }

                ContaOSS = conta;
            }
        }

        [DataMember]
        public string ContaOSS { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public DateTime Data { get; set; }

        [DataMember]
        public string NumeroProcesso { get; set; }

        [DataMember]
        public string UtilizadorAlteracao { get; set; }


    }

    [DataContract]
    public class DespesaCompromissoDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string descricaoDespesa { get; set; }

        [DataMember]
        public string nomeCompromisso { get; set; }

        [DataMember]
        public decimal valorCompromisso { get; set; }
        [DataMember]
        public DateTime dataCompromisso { get; set; }
        [DataMember]
        public int despesaRegistadaFk { get; set; }
    }
}