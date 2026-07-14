using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetBankStatementLinesRequest : RequestBaseDataContract
    {
        [DataMember]
        public int? ContaBancariaFk { get; set; }

        [DataMember]
        public DateTime? DataInicio { get; set; }

        [DataMember]
        public DateTime? DataFim { get; set; }
    }

    [DataContract]
    public class GetReceitasDisponiveisParaConciliacaoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class AddBankStatementLineRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ContaBancariaFk { get; set; }

        [DataMember]
        public DateTime DataValor { get; set; }

        [DataMember]
        public DateTime? DataTransacao { get; set; }

        [DataMember]
        public string CodigoTransacaoBancaria { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Credito { get; set; }

        [DataMember]
        public decimal Debito { get; set; }
    }

    [DataContract]
    public class DeleteBankStatementLineRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class MatchReceitaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ReceitaPacFk { get; set; }
    }

    [DataContract]
    public class MatchPagamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int PaymentExecutionFk { get; set; }
    }

    [DataContract]
    public class UnmatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    // Bước 1/2 (CLAUDE.md §6) — chỉ đọc file Excel + đối chiếu trùng lặp,
    // KHÔNG ghi DB.
    [DataContract]
    public class ImportBankStatementLinePreviewRequest : RequestBaseDataContract
    {
        public IFormFile File { get; set; }

        [DataMember]
        public int ContaBancariaFk { get; set; }
    }

    [DataContract]
    public class BankStatementLineImportRowConfirmRequest
    {
        [DataMember]
        public int RowNum { get; set; }

        [DataMember]
        public DateTime DataValor { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Credito { get; set; }

        [DataMember]
        public decimal Debito { get; set; }

        // "Insert" | "Skip" — do người dùng chọn ở bước preview.
        [DataMember]
        public string Action { get; set; }
    }

    // Bước 2/2 — chỉ nhận lại danh sách dòng đã preview (không nhận lại file),
    // áp dụng đúng Action người dùng đã chọn cho từng dòng.
    [DataContract]
    public class ConfirmBankStatementLineImportRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ContaBancariaFk { get; set; }

        [DataMember]
        public List<BankStatementLineImportRowConfirmRequest> Rows { get; set; }
    }
}
