using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    public class MovimentosUpsertData
    {
        [DataMember]
        public int? Id { get; set; }

        [DataMember]
        public int TarefaAtivoId { get; set; }

        [DataMember]
        public int? CaixaId { get; set; }

        [DataMember]
        public int? BancoId { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public DateTime Data { get; set; }

        [DataMember]
        public decimal Valor { get; set; }
    }

    public class MovimentosData
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int? TarefaAtivoId { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public DateTime DataValor { get; set; }

        [DataMember]
        public decimal? Credito { get; set; }

        [DataMember]
        public decimal? Debito { get; set; }

        [DataMember]
        public bool Conciliado { get; set; }

        [DataMember]
        public decimal? Saldo { get; set; }
    }
}