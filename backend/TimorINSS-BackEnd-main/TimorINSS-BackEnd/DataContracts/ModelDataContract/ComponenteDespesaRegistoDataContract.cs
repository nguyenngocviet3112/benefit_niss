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

    // Uma despesa já existente para a mesma combinação de 5 parâmetros, mostrada ao utilizador
    // no aviso de confirmação. Inclui o número do processo para ele conseguir ir lá ver -- era
    // precisamente isso que faltava quando o registo era simplesmente bloqueado.
    // [VI] Một despesa đã tồn tại cùng tổ hợp 5 tham số, hiển thị trong cảnh báo xác nhận. Có kèm
    // số processo để người dùng lần ra được -- đúng thứ còn thiếu khi trước đây chỉ chặn cứng.
    [DataContract]
    public class DespesaEmCursoDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string NumeroProcesso { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        // Código do estado (ESTADODESPESA: 1 = Registada, 2 = Autorizada) e não a descrição da
        // tabela DOMINIO, que nesta base de dados é só a letra "R"/"A" e não diz nada ao utilizador.
        // A etiqueta legível é resolvida no frontend, para sair traduzida em PT/EN/TET.
        // [VI] Trả về mã trạng thái (ESTADODESPESA: 1 = Registada, 2 = Autorizada) thay vì descricao
        // của bảng DOMINIO — trên DB này descricao chỉ là chữ "R"/"A", người dùng đọc không hiểu.
        // Nhãn đọc được do frontend dịch, để ra đúng PT/EN/TET.
        [DataMember]
        public int EstadoValor { get; set; }
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

        // Só populado no relatório de Receitas. Uma Receita pode reunir movimentos
        // de mais de um banco (1 ComponentereceitaRegisto : N RelMovimentosporconciliarMovimentos);
        // aqui mostramos apenas o PRIMEIRO banco encontrado (decisão de produto — ver
        // ComponenteReceitaRegistoRepository.ReceitasRelatorios). Se no futuro for preciso
        // mostrar todos os bancos, trocar o FirstOrDefault() por um string.Join(", ", ...).
        [DataMember]
        public string BankCode { get; set; }


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