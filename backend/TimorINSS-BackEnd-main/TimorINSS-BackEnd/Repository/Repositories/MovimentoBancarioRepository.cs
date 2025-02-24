using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class MovimentoBancarioRepository : IMovimentoBancarioRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public MovimentoBancarioRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Movimentobancario> GetAll()
        {
            return _moduloContribuicoesContext.Movimentobancario
                .ToList();
        }

        public Movimentobancario Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var campo = _moduloContribuicoesContext.Movimentobancario
                .SingleOrDefault(u => u.Id == id);

            return campo;
        }

        public MovimentoBancarioDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var movimento = _moduloContribuicoesContext.Movimentobancario
                .SingleOrDefault(u => u.Id == id);

            MovimentoBancarioDto movimentoDto = Utils.MappClassToDto<Movimentobancario, MovimentoBancarioDto>(movimento);
            return movimentoDto;
        }

        public void Add(Movimentobancario entity)
        {
            _moduloContribuicoesContext.Movimentobancario.Add(entity);
        }

        public void Update(Movimentobancario entity)
        {
            Movimentobancario entityToUpdate = _moduloContribuicoesContext.Movimentobancario
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Movimentobancario entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ValueCampoEditavelListagemResponse GetAllMovimentosBancarios(SearchFilter filter, List<SelectDescription> dropDownSelectionDescription)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var queryMovimentoBancario = _moduloContribuicoesContext.Movimentobancario
                .Where(a => a.Descricao.Contains(filter.filterBy))
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.Id,
                   Nome = u.Descricao + " - " + u.TipoMovimentoNavigation.Descricao,
                   Parametros = new List<ParametrosAdicionais>()
                   {
                       new ParametrosAdicionais
                    {
                        Nome = "TipoMovimento",
                        Type = "dropdown",
                        SelectedDropdown = u.TipoMovimento,
                        DropdownList = dropDownSelectionDescription
                    },
                    new ParametrosAdicionais
                    {
                        Nome = "Descricao",
                        Size = "100",
                        Type = "string",
                        Valor = u.Descricao
                    }
                   }
               });

            var movimento = queryMovimentoBancario
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryMovimentoBancario.Count();

            response.ValuesCampo = movimento;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public bool IsMovimentoValid(Movimentobancario movimento)
        {
            int count = _moduloContribuicoesContext.Movimentobancario
                .Where(m => m.Id != movimento.Id && movimento.TipoMovimento == m.TipoMovimento && movimento.Descricao == m.Descricao)
                .Count();

            return count == 0;
        }

        public List<DominioDescricaoString> GetMovimentosBancariosDominioByFilter(int domainFilterId)
        {
            return _moduloContribuicoesContext.Movimentobancario
                .Where(u => u.TipoMovimento == domainFilterId)
                .Select(u => new DominioDescricaoString
                {
                    id = u.Id,
                    descricao = u.Descricao
                })
                .ToList();
        }
    }
}