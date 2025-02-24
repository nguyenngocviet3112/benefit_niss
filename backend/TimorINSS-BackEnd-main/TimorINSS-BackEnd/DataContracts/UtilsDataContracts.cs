using System;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.RequestDataContract;

namespace TimorINSSBackEnd.DataContracts
{
    [DataContract]
    public class SearchFilterRequest : RequestBaseDataContract
    {
        [DataMember]
        public SearchFilter? filter;
    }

    [DataContract]
    public class SearchFilter
    {
        [DataMember]
        public int? index { get; set; }

        [DataMember]
        public int? rows { get; set; }

        [DataMember]
        public DateTime? dateFilterBegin { get; set; }

        [DataMember]
        public DateTime? dateFilterEnd { get; set; }

        [DataMember]
        public string? filterField { get; set; }

        [DataMember]
        public string? filterBy { get; set; }

        [DataMember]
        public string? orderBy { get; set; }

        [DataMember]
        public OrderDirectionEnum? orderDirection { get; set; }

        [DataMember]
        public SearchFilter? filter { get; set; }
    }

    [DataContract]
    public enum OrderDirectionEnum
    {
        ascending = 1,
        descending = 2,
    }

    [DataContract]
    public enum TiposDominio
    {
        TIPOCONTRACTO = 1,
        NATUREZACONTRACTO = 2,
        LEILABORALAPLICAVEL = 3,
        TIPODOCUMENTO = 4,
        SEXO = 5,
        ESTADOCIVIL = 6,
        NACIONALIDADE = 7,
        TIPODIVIDA = 8,
        SITUACAOPAGAMENTO = 9,
        REGIME = 10,
        SALARIOMINIMO = 11,
        INDPAGO = 12,
        TIPOGUIA = 13,
        DIADECLRACAO = 14,
        PROFISSAO = 15,
        FUNCAO = 16,
        GRUPOCAMPOSEDITAVEIS = 17,
        TIPOREGIME = 18,
        TIPODOCUMENTOTAREFA = 19,
        TIPOCONTA = 20,
        CAIXAS = 21,
        NISSSEGURANCASOCIAL = 22,
        ESTADOPAGAMENTO = 23
    }

    //loging documents atributes
    public class ContainsDocumentAttribute : Attribute
    {
    }

    public class DocumentAttribute : Attribute
    {
    }

    [DataContract]
    public enum CRUD
    {
        CREATE = 1,
        READ = 2,
        UPDATE = 3,
        DELETE = 4
    }

    [DataContract]
    public enum Module
    {
        GESTAO = 1,
        CONTRIBUICOES = 2,
        FINANCEIRO = 3,
        RELATORIOS = 4
    }

    [DataContract]
    public enum ModuleGestao
    {
        GestaoPerfil = 1,
        CamposEditaveis = 3,
        GestaoUtilizador = 4,
        ConfigurarTarefas = 10,
        ConfigurarProcessos = 11,
        PreenchimentoTarefa = 12,
        ControlodeAcessodeUtilizadores = 13
    }

    [DataContract]
    public enum ModuleContribuicoes
    {
        DeclaracaoRenum = 5,
        ContaCorrente = 6,
        GuiasPagamento = 7,
        EntidadeEmpregadora = 8,
        Trabalhadores = 9
    }

    [DataContract]
    public enum ModuleRelatorios
    {
        Relatorios = 14,
        Consultas = 15
    }

    [DataContract]
    public enum ExternalModule
    {
        Auditoria = 2
    }

    [DataContract]
    public enum TarefaDateState
    {
        SemPrazo = 0,
        EmPrazo = 1,
        Pendente = 2,
        EmAtraso = 3
    }
}