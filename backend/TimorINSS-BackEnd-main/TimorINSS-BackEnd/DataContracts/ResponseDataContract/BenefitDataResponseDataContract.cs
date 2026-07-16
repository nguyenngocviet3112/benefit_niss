using System;
using System.Collections.Generic;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    // Data contracts cho bộ API "api/benefit-data" — đặc tả theo BRD Benefit module §18
    // (TÍCH HỢP API CONTRIBUTION), dùng cho module Benefit (inss-benefit-app) consume qua
    // Connector CONTRIB_API. File này độc lập với BenefitController/BenefitDataManager cũ
    // (api/benefit/getCitizenByInss) — không sửa/xoá gì ở đó.

    // Hàm 1: GET /company/{nissCompany}
    public class BenefitCompanyMasterResponse
    {
        public bool found { get; set; }
        public string nome { get; set; }
        public string niss { get; set; }
        public string tin { get; set; }
        public DateTime? dtInscricao { get; set; }
        public DateTime? dataInicioActiv { get; set; }
        public DateTime? dataFimActiv { get; set; }
        public int? numTrabalhador { get; set; }
        public string naturezaJuridicaDesc { get; set; }
        public string sectorActividadeDesc { get; set; }
        public string actividadeEconomicaDesc { get; set; }
        public List<BenefitMoradaDataContract> moradas { get; set; } = new List<BenefitMoradaDataContract>();
        public List<BenefitContactoDataContract> contactos { get; set; } = new List<BenefitContactoDataContract>();
    }

    // Hàm 2: GET /worker/{niss}
    public class BenefitWorkerProfileResponse
    {
        public bool found { get; set; }
        public string nome { get; set; }
        public DateTime? dataNasc { get; set; }
        public int? sexoFk { get; set; }
        public string sexoDesc { get; set; }
        public string naturalidade { get; set; }
        public int? estadoCivilFk { get; set; }
        public string estadoCivilDesc { get; set; }
        public int? nacionalidadeFk { get; set; }
        public string nacionalidadeDesc { get; set; }
        public string TIN { get; set; }
        public string numInscProvisoria { get; set; }
        public bool indDescNomeMae { get; set; }
        public string nomeMae { get; set; }
        public bool indDescNomePai { get; set; }
        public string nomePai { get; set; }
        public List<object> inssEstrangeiro { get; set; } = new List<object>();
        public List<BenefitMoradaDataContract> moradas { get; set; } = new List<BenefitMoradaDataContract>();
        public List<BenefitContactoDataContract> contactos { get; set; } = new List<BenefitContactoDataContract>();
        public List<BenefitDocumentoDataContract> documentos { get; set; } = new List<BenefitDocumentoDataContract>();
    }

    public class BenefitMoradaDataContract
    {
        public bool moradaPrincipal { get; set; }
        public string rua { get; set; }
        public string numPorta { get; set; }
        public string aldeia { get; set; }
        public string suco { get; set; }
        public string posto { get; set; }
        public string municipio { get; set; }
        public string pais { get; set; }
    }

    public class BenefitContactoDataContract
    {
        public bool indActivo { get; set; }
        public string telemovel { get; set; }
        public string email { get; set; }
    }

    public class BenefitDocumentoDataContract
    {
        public int idDoc { get; set; }
        public int tipoFk { get; set; }
        public string tipoDesc { get; set; }
        public string numero { get; set; }
        public DateTime? dataEmissao { get; set; }
        public DateTime? dataValidade { get; set; }
        public string localEmissao { get; set; }
        public string fileName { get; set; }
        public bool hasFile { get; set; }
    }

    // Hàm 3: GET /contributions/{niss}
    public class BenefitContributionsHistoryResponse
    {
        public bool found { get; set; }
        public string niss { get; set; }
        public List<BenefitCompanyContributions> companies { get; set; } = new List<BenefitCompanyContributions>();
    }

    public class BenefitCompanyContributions
    {
        public string nomeCompany { get; set; }
        public string nissCompany { get; set; }
        public List<BenefitContractContributions> contracts { get; set; } = new List<BenefitContractContributions>();
    }

    public class BenefitContractContributions
    {
        public int idRel { get; set; }
        public DateTime dtIniVincTrabalhador { get; set; }
        public DateTime? dtIniFimTrabalhador { get; set; }
        public int? regimeFk { get; set; }
        public string regimeDesc { get; set; }
        public List<BenefitMonthContribution> months { get; set; } = new List<BenefitMonthContribution>();
        public List<BenefitSuspensionDataContract> suspensions { get; set; } = new List<BenefitSuspensionDataContract>();
    }

    public class BenefitMonthContribution
    {
        public DateTime mesAno { get; set; }
        public decimal remunDeclarada { get; set; }
        public decimal decimoTerceiro { get; set; }
        public int? regimeFk { get; set; }
        public string regimeDesc { get; set; }
        // Taxas vêm do Regime real ligado a esta declaração mensal (Declaracaoremuneracao.RegimeFk
        // -> Regime), não do Relentidadetrabalhador.RegimeFk (que aponta para Dominio, só um
        // "tipo" descritivo, sem taxas) — taxas podem mudar ao longo do tempo, por mês é mais correto.
        public decimal? taxaEntidade { get; set; }
        public decimal? taxaTrabalhador { get; set; }
        // Declaracaoremuneracao não tem coluna de status próprio — todo registo aqui é uma
        // declaração real ("CONTRIBUTED"); os meses SUSPENSO/DISPENSA vêm à parte via
        // suspensions[] (tabela Suspensoes), como o §18.3 da BRD já prevê.
        public string status { get; set; } = "CONTRIBUTED";
    }

    public class BenefitSuspensionDataContract
    {
        public DateTime dataInicio { get; set; }
        public DateTime? dataFim { get; set; }
    }
}
