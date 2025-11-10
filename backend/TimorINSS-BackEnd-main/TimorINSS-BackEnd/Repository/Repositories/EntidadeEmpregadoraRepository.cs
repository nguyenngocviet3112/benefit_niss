using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class EntidadeEmpregadoraRepository : IEntidadeEmpregadoraRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public EntidadeEmpregadoraRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Entidadeempregadora> GetAll()
        {
            _moduloContribuicoesContext.Entidadeempregadora
               .Include(u => u.EntidadeNatJuridicaFkNavigation)
               .Include(u => u.EntidadeActEconomicaFkNavigation)
               .Include(u => u.EntidadeSectorActFkNavigation)
               .ToList();

            return _moduloContribuicoesContext.Entidadeempregadora;
        }

        public Entidadeempregadora Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var entidadeempregadora = _moduloContribuicoesContext.Entidadeempregadora
                .Include(u => u.EntidadeNatJuridicaFkNavigation)
                .Include(u => u.EntidadeActEconomicaFkNavigation)
                .Include(u => u.EntidadeSectorActFkNavigation)
                .SingleOrDefault(u => u.IdEntidadeEmpreg == id);

            return entidadeempregadora;
        }

        public EntidadeempregadoraDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var entidadeempregadora = _moduloContribuicoesContext.Entidadeempregadora
                .SingleOrDefault(u => u.IdEntidadeEmpreg == id);

            EntidadeempregadoraDto entidadeempregadoraDto = Utils.MappClassToDto<Entidadeempregadora, EntidadeempregadoraDto>(entidadeempregadora);
            return entidadeempregadoraDto;
        }

        public void Add(Entidadeempregadora entity)
        {
            _moduloContribuicoesContext.Entidadeempregadora.Add(entity);
        }

        public void Update(Entidadeempregadora entity)
        {
            Entidadeempregadora entityToUpdate = _moduloContribuicoesContext.Entidadeempregadora
                .Single(d => d.IdEntidadeEmpreg == entity.IdEntidadeEmpreg);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Update(EntidadeEmpregadoraUpsertRequest entity)
        {
            Entidadeempregadora entityToUpdate = _moduloContribuicoesContext.Entidadeempregadora
                .Single(d => d.IdEntidadeEmpreg == entity.EntidadeEmpregadora.Id);

            entityToUpdate.Nome = entity.EntidadeEmpregadora.Nome;
            entityToUpdate.SituacInscricao = entity.EntidadeEmpregadora.SituacInscricao;
        }

        public void Create(EntidadeEmpregadoraUpsertRequest entity)
        {
            Entidadeempregadora newEntity = new Entidadeempregadora()
            {
                Nome = entity.EntidadeEmpregadora.Nome,
                Tin = entity.EntidadeEmpregadora.Tin,
                Niss = entity.EntidadeEmpregadora.Niss,
                SituacInscricao = entity.EntidadeEmpregadora.SituacInscricao,
                UtilizadorCriacao = entity.UserId,
                NumTrabalhador = entity.EntidadeEmpregadora.NumTrabalhador.Value,
                DataInicioActiv = entity.EntidadeEmpregadora.DataInicioActiv.Value,
                DataInicioTrabServico = entity.EntidadeEmpregadora.DataInicioTrabServico.Value,
                EntidadeNatJuridicaFk = entity.EntidadeEmpregadora.IdNaturezaJuridica.Value,
                EntidadeActEconomicaFk = entity.EntidadeEmpregadora.IdActividadeEconomica.Value,
                EntidadeSectorActFk = entity.EntidadeEmpregadora.IdSectorActividade.Value,
                DtInscricao = entity.EntidadeEmpregadora.DataInscricao.Value,
                FlagImportado = false,
                DataCriacao = DateTime.Now,
                DataAlteracao = DateTime.Now,
                DtHoraUltimoAcesso = DateTime.Now,
                Contacto = new List<Contacto>()
                {
                    new Contacto()
                    {
                        Email = entity.EntidadeEmpregadora.Email,
                        Telemovel = entity.EntidadeEmpregadora.Telemovel,
                        UtilizadorCriacao = entity.UserId,
                        DataCriacao = DateTime.Now,
                    }
                }
            };

            _moduloContribuicoesContext.Entidadeempregadora.Add(newEntity);
        }

        public void Delete(Entidadeempregadora entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public EntidadeEmpregadoraConsultaResponse GetByIdEntidade(int id)
        {
            var queryEntidade = _moduloContribuicoesContext.Entidadeempregadora
                .Where(u => u.IdEntidadeEmpreg == id)

                .Include(u => u.EntidadeNatJuridicaFkNavigation)
                .Include(u => u.EntidadeActEconomicaFkNavigation)
                .Include(u => u.EntidadeSectorActFkNavigation)
                .Select(entidadeempregadora => new EntidadeEmpregadoraConsultaResponse
                {
                    IdEntidadeEmpreg = entidadeempregadora.IdEntidadeEmpreg,
                    Nome = entidadeempregadora.Nome,
                    Niss = entidadeempregadora.Niss,
                    Tin = entidadeempregadora.Tin,
                    SituacInscricao = entidadeempregadora.SituacInscricao,
                    NumTrabalhador = entidadeempregadora.NumTrabalhador,
                    DataInicioActiv = entidadeempregadora.DataInicioActiv,
                    IdNaturezaJuridica = entidadeempregadora.EntidadeNatJuridicaFkNavigation.IdNatJuridica,
                    IdActividadeEconomica = entidadeempregadora.EntidadeActEconomicaFkNavigation.IdActivEconomica,
                    IdSectorActividade = entidadeempregadora.EntidadeSectorActFkNavigation.IdSectorActividade,
                    DataInicioTrabServico = entidadeempregadora.DataInicioTrabServico,
                    DtInscricao = entidadeempregadora.DtInscricao,
                    DataFimActiv = entidadeempregadora.DataFimActiv,
                    DtHoraUltimoAcesso = entidadeempregadora.DtHoraUltimoAcesso
                });

            EntidadeEmpregadoraConsultaResponse entidade = queryEntidade
            .SingleOrDefault();

            return entidade;
        }

        public Entidadeempregadora GetByNiss(string niss)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var entidadeempregadora = _moduloContribuicoesContext.Entidadeempregadora
                .SingleOrDefault(u => u.Niss == niss);

            return entidadeempregadora;
        }

        public Entidadeempregadora GetByTin(string tin)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var entidadeempregadora = _moduloContribuicoesContext.Entidadeempregadora
                .SingleOrDefault(u => u.Tin == tin);

            return entidadeempregadora;
        }

        public EntidadeEmpregadoraDeclaracaoViewResponse GetEntidadeInfoForDeclaracao(EntidadeEmpregadoraIdRequest request)
        {
            const int SECRET_A = 999;
            const int SECRET_B = 123456789;
            long decodedId = (request.IdEntidade - SECRET_B) / SECRET_A;

            return _moduloContribuicoesContext.Entidadeempregadora
                .Where(u => u.IdEntidadeEmpreg == decodedId)
                .Select(u => new EntidadeEmpregadoraDeclaracaoViewResponse
                {
                    idEntidadeEmpreg = u.IdEntidadeEmpreg,
                    niss = u.Niss,
                    nome = u.Nome,
                    tin = u.Tin,
                    dataInicioDeclaracao = u.DataInicioActiv,
                    dataFimActiv = u.DataFimActiv
                }).FirstOrDefault();
        }

        public EntidadeEmpregadoraConsultaResponse GetEntidadeByNiss(string Niss)
        {
            var queryEntidade = _moduloContribuicoesContext.Entidadeempregadora
                .Where(u => u.Niss == Niss)

                .Include(u => u.EntidadeNatJuridicaFkNavigation)
                .Include(u => u.EntidadeActEconomicaFkNavigation)
                .Include(u => u.EntidadeSectorActFkNavigation)
                .Select(entidadeempregadora => new EntidadeEmpregadoraConsultaResponse
                {
                    IdEntidadeEmpreg = entidadeempregadora.IdEntidadeEmpreg,
                    Nome = entidadeempregadora.Nome,
                    Niss = entidadeempregadora.Niss,
                    Tin = entidadeempregadora.Tin,
                    SituacInscricao = entidadeempregadora.SituacInscricao,
                    NumTrabalhador = entidadeempregadora.NumTrabalhador,
                    DataInicioActiv = entidadeempregadora.DataInicioActiv,
                    IdNaturezaJuridica = entidadeempregadora.EntidadeNatJuridicaFkNavigation.IdNatJuridica,
                    IdActividadeEconomica = entidadeempregadora.EntidadeActEconomicaFkNavigation.IdActivEconomica,
                    IdSectorActividade = entidadeempregadora.EntidadeSectorActFkNavigation.IdSectorActividade,
                    DataInicioTrabServico = entidadeempregadora.DataInicioTrabServico,
                    DtInscricao = entidadeempregadora.DtInscricao,
                    DataFimActiv = entidadeempregadora.DataFimActiv,
                    DtHoraUltimoAcesso = entidadeempregadora.DtHoraUltimoAcesso
                });

            EntidadeEmpregadoraConsultaResponse entidade = queryEntidade
            .SingleOrDefault();

            return entidade;
        }

        public DestinatarioDataContract GetDestinatarioByNiss(string niss)
        {
            return _moduloContribuicoesContext.Entidadeempregadora
                .Where(u => u.Niss == niss)
                .Include(u => u.Morada)
                .Select(u => new DestinatarioDataContract
                {
                    EntidadeFk = u.IdEntidadeEmpreg,
                    Nome = u.Nome,
                    Niss = u.Niss,
                    Tin = u.Tin
                })
                .FirstOrDefault();
        }

        public DestinatarioDataContract GetDestinatarioByTin(string tin)
        {
            return _moduloContribuicoesContext.Entidadeempregadora
                .Where(u => u.Tin == tin)
               .Select(u => new DestinatarioDataContract
               {
                   EntidadeFk = u.IdEntidadeEmpreg,
                   Nome = u.Nome,
                   Niss = u.Niss,
                   Tin = u.Tin,
               })
                .FirstOrDefault();
        }

        public Destinatario GetDestinatarioByIdEntidade(int id)
        {
            return _moduloContribuicoesContext.Entidadeempregadora
                .Where(u => u.IdEntidadeEmpreg == id)
               .Select(u => new Destinatario
               {
                   EntidadeFk = u.IdEntidadeEmpreg,
                   Nome = u.Nome,
                   Niss = u.Niss,
                   Tin = u.Tin,
               })
                .FirstOrDefault();
        }

        public List<Entidadeempregadora> GetEntidadesByNissOrTin(List<string> niss, List<string> tin)
        {
            return _moduloContribuicoesContext.Entidadeempregadora
                .Where(u => niss.Contains(u.Niss) || tin.Contains(u.Tin))
                .ToList();
        }
    }
}