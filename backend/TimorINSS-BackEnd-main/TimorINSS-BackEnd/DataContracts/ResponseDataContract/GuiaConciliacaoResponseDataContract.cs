using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GuiaComprovativoDataContract
    {
        [DataMember]
        public int IdGuia { get; set; }

        [DataMember]
        public string NumDocumento { get; set; }

        // Ngày upload chứng từ — DataAlteracao (không phải DataCriacao, đó là ngày tạo
        // Guia ban đầu) vì insertComprovativoPagamento chỉ UPDATE dòng Guiapagamento
        // đã có, không tạo dòng mới; DataAlteracao là dấu vết gần nhất khớp với
        // thời điểm doanh nghiệp nộp chứng từ.
        [DataMember]
        public DateTime? DataUpload { get; set; }

        // Ngày trả tiền doanh nghiệp tự khai (DataComprovPag).
        [DataMember]
        public DateTime? DataPagamento { get; set; }

        // Số tiền doanh nghiệp tự khai đã trả (ValorComprovPag).
        [DataMember]
        public decimal? ValorPago { get; set; }

        [DataMember]
        public string BankCode { get; set; }

        [DataMember]
        public byte[] ComprovativoPag { get; set; }
    }

    [DataContract]
    public class GuiaComprovativoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public GuiaComprovativoDataContract Guia { get; set; }
    }
}
