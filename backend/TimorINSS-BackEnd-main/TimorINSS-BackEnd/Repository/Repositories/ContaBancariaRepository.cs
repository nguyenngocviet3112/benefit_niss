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
    public class ContaBancariaRepository : IContaBancariaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ContaBancariaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Contabancaria> GetAll()
        {
            return _moduloContribuicoesContext.Contabancaria
                .ToList();
        }

        public Contabancaria Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var campo = _moduloContribuicoesContext.Contabancaria
                .SingleOrDefault(u => u.Id == id);

            return campo;
        }

        public ContaBancariaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var conta = _moduloContribuicoesContext.Contabancaria
                .SingleOrDefault(u => u.Id == id);

            ContaBancariaDto contaDto = Utils.MappClassToDto<Contabancaria, ContaBancariaDto>(conta);
            return contaDto;
        }

        public void Add(Contabancaria entity)
        {
            _moduloContribuicoesContext.Contabancaria.Add(entity);
        }

        public void Update(Contabancaria entity)
        {
            Contabancaria entityToUpdate = _moduloContribuicoesContext.Contabancaria
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Contabancaria entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ValueCampoEditavelListagemResponse GetAllContasBancarias(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var queryContaBancaria = _moduloContribuicoesContext.Contabancaria
                .Where(a => (a.Swift + " " + a.Descricao).Contains(filter.filterBy))
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.Id,
                   Nome = u.Swift + " " + u.Descricao,
                   Parametros = new List<ParametrosAdicionais>()
                   {
                    new ParametrosAdicionais
                    {
                        Nome = "SWIFT",
                        Size = "8",
                        Type = "string",
                        Valor = u.Swift
                    },
                    new ParametrosAdicionais
                    {
                        Nome = "EntidadeBancaria",
                        Size = "25",
                        Type = "string",
                        Valor = u.EntidadeBancaria
                    },
                    new ParametrosAdicionais
                    {
                        Nome = "Descricao",
                        Size = "25",
                        Type = "string",
                        Valor = u.Descricao
                    },
                    new ParametrosAdicionais
                    {
                        Nome = "IBAN",
                        Size = "25",
                        Type = "string",
                        Valor = u.Iban
                    },
                    new ParametrosAdicionais
                    {
                        Nome = "NConta",
                        Size = "21",
                        Type = "number",
                        Valor = u.Numero
                    }
                   }
               });

            var contaBancaria = queryContaBancaria
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryContaBancaria.Count();

            response.ValuesCampo = contaBancaria;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public bool IsIbanValid(Contabancaria conta)
        {
            int count = _moduloContribuicoesContext.Contabancaria
                 .Where(c => c.Iban == conta.Iban && c.Id != conta.Id)
                 .Count();
            return count == 0;
        }

        public List<ContaBancariaDto> GetAllDto(bool incluirSaldo)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var movimentos = _moduloContribuicoesContext.Contabancaria
                .Select(x =>
                   new ContaBancariaDto
                   {
                       Id = x.Id,
                       Swift = x.Swift,
                       EntidadeBancaria = x.EntidadeBancaria,
                       Descricao = x.Descricao,
                       Iban = x.Iban,
                       Numero = x.Numero,
                       Saldo = !incluirSaldo ? null : _moduloContribuicoesContext.Movimentosbancarios.Where(a => a.ContaFk == x.Id).Sum(a => a.Credito.HasValue ? a.Credito : a.Debito)
                   }
                ).ToList();

            return movimentos;
        }
    }
}