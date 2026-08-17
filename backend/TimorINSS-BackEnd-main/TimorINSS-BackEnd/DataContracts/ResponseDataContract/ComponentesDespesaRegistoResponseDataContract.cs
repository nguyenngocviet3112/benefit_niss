using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GetComponenteDespesaRegistoReponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<DespesaRegistadaDataContract> ComponenteDespesaRegisto;
    }

    // Resposta do registo de despesa. Quando DespesasEmCurso vem preenchida, NADA foi gravado:
    // são as despesas que já existem para a mesma combinação de 5 parâmetros e que o utilizador
    // deve ver antes de confirmar (aviso, não bloqueio).
    // [VI] Response của màn đăng ký despesa. Khi DespesasEmCurso có dữ liệu nghĩa là CHƯA lưu gì:
    // đó là các despesa đã tồn tại cùng tổ hợp 5 tham số, để người dùng xem trước khi xác nhận
    // (cảnh báo, không phải chặn).
    [DataContract]
    public class AddEditDespesaRegistoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<DespesaEmCursoDataContract> DespesasEmCurso;
    }

    [DataContract]
    public class GetValoresDespesaByIdCodigoOrcamentoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public ValoresDespesaRegistadaDataContract ValoresDespesa;
    }

    [DataContract]
    public class GetComponenteDespesaCabimentadaParaExecucaoReponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<DespesaCabimentadasParaExecucaoDataContract> DespesasParaExecucao;
    }

    [DataContract]
    public class GetDespesasRelatoriosReponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<DespesasRelatoriosDataContract> Despesas;
    }

    [DataContract]
    public class GetDespesasCompromissoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<DespesaCompromissoDataContract> ComponenteDespesaObrigacao;
    }
}