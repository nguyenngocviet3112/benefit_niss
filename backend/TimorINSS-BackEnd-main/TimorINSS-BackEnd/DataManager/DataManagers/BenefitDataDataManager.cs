using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    // DataManager độc lập cho bộ API "api/benefit-data" (BRD Benefit module §18 —
    // TÍCH HỢP API CONTRIBUTION). Mọi field JSON đặt tên đúng theo những gì
    // inss-benefit-app/api/src/contribution/contribution.service.ts đọc (getWorkerPrefill/
    // toMatrix/toCompanies) để module Benefit không cần sửa gì khi trỏ Connector CONTRIB_API
    // vào base URL này.
    public class BenefitDataDataManager : IBenefitDataDataManager
    {
        private readonly IBenefitDataRepository _repository;

        public BenefitDataDataManager(IBenefitDataRepository repository)
        {
            _repository = repository;
        }

        public BenefitCompanyMasterResponse GetCompanyMasterByNiss(string nissCompany)
        {
            Entidadeempregadora entidade = _repository.GetCompanyMasterByNiss(nissCompany);
            if (entidade == null)
            {
                return new BenefitCompanyMasterResponse { found = false };
            }

            return new BenefitCompanyMasterResponse
            {
                found = true,
                nome = entidade.Nome,
                niss = entidade.Niss,
                tin = entidade.Tin,
                dtInscricao = entidade.DtInscricao,
                dataInicioActiv = entidade.DataInicioActiv,
                dataFimActiv = entidade.DataFimActiv,
                numTrabalhador = entidade.NumTrabalhador,
                naturezaJuridicaDesc = entidade.EntidadeNatJuridicaFkNavigation?.Descricao,
                sectorActividadeDesc = entidade.EntidadeSectorActFkNavigation?.Descricao,
                actividadeEconomicaDesc = entidade.EntidadeActEconomicaFkNavigation?.Descricao,
                moradas = BuildMoradas(entidade.Morada),
                contactos = BuildContactos(entidade.Contacto),
            };
        }

        public BenefitWorkerProfileResponse GetWorkerFullByNiss(string niss)
        {
            Trabalhador trabalhador = _repository.GetWorkerFullByNiss(niss);
            if (trabalhador == null)
            {
                return new BenefitWorkerProfileResponse { found = false };
            }

            return new BenefitWorkerProfileResponse
            {
                found = true,
                nome = trabalhador.Nome,
                dataNasc = trabalhador.DataNasc,
                sexoFk = trabalhador.SexoTrabalhador,
                sexoDesc = trabalhador.SexoTrabalhadorNavigation?.Descricao,
                naturalidade = trabalhador.Naturalidade,
                estadoCivilFk = trabalhador.EstadoCivil,
                estadoCivilDesc = trabalhador.EstadoCivilNavigation?.Descricao,
                nacionalidadeFk = trabalhador.NacionalidadeTrabalhador,
                nacionalidadeDesc = trabalhador.NacionalidadeTrabalhadorNavigation?.Descricao,
                TIN = trabalhador.Tin,
                numInscProvisoria = trabalhador.NumInscProvisoria,
                indDescNomeMae = trabalhador.IndDescNomeMae,
                nomeMae = trabalhador.NomeMae,
                indDescNomePai = trabalhador.IndDescNomePai,
                nomePai = trabalhador.NomePai,
                inssEstrangeiro = (trabalhador.Inssestrangeiro ?? new List<Inssestrangeiro>())
                    .Where(e => e.IndActivo)
                    .Select(e => (object)new
                    {
                        nomeSsestrangeiro = e.NomeSsestrangeiro,
                        nissestrangeiro = e.Nissestrangeiro,
                        indDecontAtualmente = e.IndDecontAtualmente,
                        indBenfAtualmente = e.IndBenfAtualmente,
                    }).ToList(),
                moradas = BuildMoradas(trabalhador.Morada),
                contactos = BuildContactos(trabalhador.Contacto),
                documentos = (trabalhador.Documentoidentificacao ?? new List<Documentoidentificacao>())
                    .Where(d => d.IndActivo)
                    .Select(d => new BenefitDocumentoDataContract
                    {
                        idDoc = d.IdDocIdentificacao,
                        tipoFk = d.TpDocIdentificacao,
                        tipoDesc = d.TpDocIdentificacaoNavigation?.Descricao,
                        numero = d.Numero,
                        dataEmissao = d.DataEmissao,
                        dataValidade = d.DataValidade,
                        localEmissao = d.LocalEmissao,
                        fileName = d.NomeDocumento,
                        hasFile = d.Documento != null && d.Documento.Length > 0,
                    }).ToList(),
            };
        }

        public Documentoidentificacao GetDocumentoById(int idDoc)
        {
            return _repository.GetDocumentoById(idDoc);
        }

        public BenefitContributionsHistoryResponse GetContributionHistoryByNiss(string niss)
        {
            (Trabalhador worker, List<Relentidadetrabalhador> contratos, List<Suspensoes> suspensoes) = _repository.GetContributionHistoryByNiss(niss);
            if (worker == null)
            {
                return new BenefitContributionsHistoryResponse { found = false, niss = niss };
            }

            List<BenefitCompanyContributions> companies = contratos
                .GroupBy(c => new { c.EntidadeFk, Nome = c.EntidadeFkNavigation?.Nome, Niss = c.EntidadeFkNavigation?.Niss })
                .Select(g => new BenefitCompanyContributions
                {
                    nomeCompany = g.Key.Nome,
                    nissCompany = g.Key.Niss,
                    contracts = g.Select(c => BuildContract(c, suspensoes)).ToList(),
                })
                .ToList();

            return new BenefitContributionsHistoryResponse
            {
                found = true,
                niss = niss,
                companies = companies,
            };
        }

        private BenefitContractContributions BuildContract(Relentidadetrabalhador contrato, List<Suspensoes> suspensoes)
        {
            return new BenefitContractContributions
            {
                idRel = contrato.IdRel,
                dtIniVincTrabalhador = contrato.DtIniVincTrabalhador,
                dtIniFimTrabalhador = contrato.DtIniFimTrabalhador,
                regimeFk = contrato.RegimeFk,
                regimeDesc = contrato.RegimeFkNavigation?.Descricao,
                months = (contrato.Declaracaoremuneracao ?? new List<Declaracaoremuneracao>())
                    .Where(d => d.IndActivo)
                    .OrderBy(d => d.MesAno)
                    .Select(d => new BenefitMonthContribution
                    {
                        mesAno = d.MesAno,
                        remunDeclarada = d.RemunDeclarada,
                        decimoTerceiro = d.DecimoTerceiro,
                        regimeFk = d.RegimeFk,
                        regimeDesc = d.RegimeFkNavigation?.NomeRegime,
                        taxaEntidade = d.RegimeFkNavigation?.PercentEntidadeEmpreg,
                        taxaTrabalhador = d.RegimeFkNavigation?.PercentTrabalhador,
                        status = "CONTRIBUTED",
                    }).ToList(),
                // Suspensoes liga-se por (trabalhador, entidade) — não por contrato específico
                // (a tabela não tem FK para Relentidadetrabalhador) — associa aqui pela mesma entidade.
                suspensions = suspensoes
                    .Where(s => s.EntidadeSuspensaoFk == contrato.EntidadeFk)
                    .Select(s => new BenefitSuspensionDataContract
                    {
                        dataInicio = s.DataInicioSuspensao,
                        dataFim = s.DataFimSuspensao,
                    }).ToList(),
            };
        }

        private List<BenefitMoradaDataContract> BuildMoradas(ICollection<Morada> moradas)
        {
            return (moradas ?? new List<Morada>())
                .Select(m => new BenefitMoradaDataContract
                {
                    moradaPrincipal = m.MoradaPrincipal,
                    rua = m.Rua,
                    numPorta = m.NumPorta,
                    aldeia = m.MoradaAldeiaFkNavigation?.Nome,
                    suco = m.MoradaAldeiaFkNavigation?.AldeiaSucoFkNavigation?.Nome,
                    posto = m.MoradaAldeiaFkNavigation?.AldeiaSucoFkNavigation?.SucoPostoAdminFkNavigation?.Nome,
                    municipio = m.MoradaAldeiaFkNavigation?.AldeiaSucoFkNavigation?.SucoPostoAdminFkNavigation?.PostoAdminMunicipioFkNavigation?.Nome,
                    pais = m.MoradaPaisFkNavigation?.Nome,
                }).ToList();
        }

        private List<BenefitContactoDataContract> BuildContactos(ICollection<Contacto> contactos)
        {
            return (contactos ?? new List<Contacto>())
                .Select(c => new BenefitContactoDataContract
                {
                    indActivo = c.IndActivo,
                    telemovel = c.Telemovel,
                    email = c.Email,
                }).ToList();
        }
    }
}
