using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class RelatorioBalancoDataContract
    {
        //[DataMember]
        //public ICollection<Codigoconta> _filhos
        //{
        //    set
        //    {
        //        filhos = ConvertFilhos(value);
        //    }
        //}

        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int? parentId { get; set; }

        //[DataMember]
        //public List<RelatorioBalancoDataContract> filhos { get; set; }

        [DataMember]
        public string codigo { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public decimal credito { get; set; }

        [DataMember]
        public decimal debito { get; set; }

        [DataMember]
        public decimal creditoAntes { get; set; }

        [DataMember]
        public decimal debitoAntes { get; set; }

        //public List<RelatorioBalancoDataContract> ConvertFilhos(ICollection<Codigoconta> lista)
        //{

        //    var filhos = new List<RelatorioBalancoDataContract>();

        //    foreach (var item in lista)
        //    {
        //        filhos.Add(new RelatorioBalancoDataContract()
        //        {
        //            id = item.Id,
        //            nome = item.Designacao,
        //            codigo = item.Codigo,
        //            credito = item.PagamentosexecutadosCodigoContaCreditoFkNavigation.Select(e => e.ValorExecutado).Sum(),
        //            debito = item.PagamentosexecutadosCodigoContaDebitoFkNavigation.Select(e => e.ValorExecutado).Sum(),
        //            filhos = ConvertFilhos(item.InverseParentFkNavigation),
        //        });
        //    }
        //    return filhos;
        //}
    }

}