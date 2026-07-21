using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ClassificacaoEconomicaExecucaoDataContract
    {
        [DataMember]
        public string codigoCE { get; set; }

        [DataMember]
        public string designacaoCE { get; set; }

        [DataMember]
        public int nivel { get; set; }

        [DataMember]
        public decimal valorOrcamentoInicial { get; set; }

        [DataMember]
        public decimal valorOrcamentado { get; set; }

        [DataMember]
        public decimal janeiro { get; set; }

        [DataMember]
        public decimal fevereiro { get; set; }

        [DataMember]
        public decimal marco { get; set; }

        [DataMember]
        public decimal abril { get; set; }

        [DataMember]
        public decimal maio { get; set; }

        [DataMember]
        public decimal junho { get; set; }

        [DataMember]
        public decimal julho { get; set; }

        [DataMember]
        public decimal agosto { get; set; }

        [DataMember]
        public decimal setembro { get; set; }

        [DataMember]
        public decimal outubro { get; set; }

        [DataMember]
        public decimal novembro { get; set; }

        [DataMember]
        public decimal dezembro { get; set; }

        [DataMember]
        public decimal totalExecucao { get; set; }

        [DataMember]
        public decimal taxaExecucao { get; set; }

        // [EN] Despesa-only fields (0 for Receita rows) -- see sheet CE_OSS_Global, "OSS GLOBAL" (Despesa) block,
        // columns D/E/F + U-Z, in fin and contrib doc/3. Customer Send/OSS_Global_2026_FINAL_livro.xlsx.
        // [VI] Chỉ dùng cho Despesa (Receita luôn = 0) -- xem sheet CE_OSS_Global, khối "OSS GLOBAL" (Despesa),
        // cột D/E/F + U-Z, trong file OSS_Global_2026_FINAL_livro.xlsx của khách.
        [DataMember]
        public decimal cabimentos { get; set; }

        [DataMember]
        public decimal compromissos { get; set; }

        [DataMember]
        public decimal obrigacoes { get; set; }

        [DataMember]
        public decimal saldoDisponivel { get; set; } // (10) = OSS corrigido - Cabimentos

        [DataMember]
        public decimal saldoNaoComprometido { get; set; } // (11) = OSS corrigido - Compromissos

        [DataMember]
        public decimal valorCabimentadoNaoComprometido { get; set; } // (12) = Cabimentos - Compromissos

        [DataMember]
        public decimal valorComprometidoNaoLiquidado { get; set; } // (13) = Compromissos - Obrigações

        [DataMember]
        public decimal valorLiquidadoNaoPago { get; set; } // (14) = Obrigações - Total Execução (DÍVIDA)

        // [EN] Receita-only field (0 for Despesa rows).
        // [VI] Chỉ dùng cho Receita (Despesa luôn = 0).
        [DataMember]
        public decimal receitaLiquidada { get; set; }

        [DataMember]
        public decimal saldoReceitaLiquidadaNaoCobrada { get; set; } // Receita: Receita Liquidada - Total Execução (DÍVIDA)

        // [EN] Shared field -- Despesa: OSS corrigido - Total Execução. Receita: same formula, same meaning.
        // [VI] Dùng chung cho cả 2 -- Despesa: OSS corrigido - Total Execução. Receita: cùng công thức, cùng ý nghĩa.
        [DataMember]
        public decimal saldoExecucao { get; set; }
    }
}
