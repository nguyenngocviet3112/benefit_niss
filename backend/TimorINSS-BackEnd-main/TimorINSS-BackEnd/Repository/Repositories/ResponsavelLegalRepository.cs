using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ResponsavelLegalRepository : IResponsavelLegalRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ResponsavelLegalRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Responsavellegal> GetAll()
        {
            return _moduloContribuicoesContext.Responsavellegal.Where(u => u.IndActivo && u.IndActivo == true).ToList();
        }

        public Responsavellegal Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var responsavelLegal = _moduloContribuicoesContext.Responsavellegal
                .SingleOrDefault(u => u.IdResponsavelLegal == id && u.IndActivo == true);

            return responsavelLegal;
        }

        public Responsavellegal GetByTin(string tin)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var responsavelLegal = _moduloContribuicoesContext.Responsavellegal
                .FirstOrDefault(u => u.Tin == tin && u.IndActivo == true);

            return responsavelLegal;
        }

        public Responsavellegal GetByTrabalhadorId(long trabalhadorId)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var responsavelLegal = _moduloContribuicoesContext.Responsavellegal
                .SingleOrDefault(u => u.RespLegalTabalhadorFk == trabalhadorId && u.IndActivo == true);

            return responsavelLegal;
        }

        public ResponsavellegalDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var responsavelLegal = _moduloContribuicoesContext.Responsavellegal
                .SingleOrDefault(u => u.IdResponsavelLegal == id && u.IndActivo == true);

            ResponsavellegalDto responsavelLegalDto = Utils.MappClassToDto<Responsavellegal, ResponsavellegalDto>(responsavelLegal);
            return responsavelLegalDto;
        }

        public void Add(Responsavellegal entity)
        {
            _moduloContribuicoesContext.Responsavellegal.Add(entity);
        }

        public void Update(Responsavellegal entity)
        {
            Responsavellegal entityToUpdate = _moduloContribuicoesContext.Responsavellegal
                .Single(d => d.IdResponsavelLegal == entity.IdResponsavelLegal);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Responsavellegal entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ResponsavelLegalListagemResponse GetByIdEntidadeEmpregadora(ResponsavelLegalListagemRequest request, List<DominioDescricaoString> funcaoDominioList)
        {
            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var query = _moduloContribuicoesContext.Relentidaderesplegal
               .Where(u => u.RelEntidadeRespFk == request.id && u.IndActivo)
               .Join(
                    _moduloContribuicoesContext.Responsavellegal
                    .Where(u => u.IndActivo)
                    .Include(u => u.Documentoidentificacao.Where(c => c.IndActivo)),
                    relEntidadeResponsavel => relEntidadeResponsavel.RelRespLegalEntFk,
                    responsavelLegal => responsavelLegal.IdResponsavelLegal,
                    (relEntidadeResponsavel, responsavelLegal) => new ResponsavelLegalListagem
                    {
                        IdResponsavelLegal = responsavelLegal.IdResponsavelLegal,
                        Nome = responsavelLegal.Nome,
                        Funcao = responsavelLegal.Funcao,
                        FuncaoOutro = responsavelLegal.FuncaoOutro,
                        Tin = responsavelLegal.Tin
                    }
                );
            var respLegal = query
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = query.Count();

            foreach (var resp in respLegal)
            {
                resp.FuncaoString = funcaoDominioList.FirstOrDefault(x => x.id == resp.Funcao).descricao == "Outro" ? resp.FuncaoOutro : funcaoDominioList.FirstOrDefault(x => x.id == resp.Funcao).descricao;
            }

            ResponsavelLegalListagemResponse result = new ResponsavelLegalListagemResponse
            {
                responsavelLegal = respLegal,
                rows = totalNumber
            };
            return result;
        }
    }
}