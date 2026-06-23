using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class CodigoContaRepository : ICodigoContaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public CodigoContaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Codigoconta> GetAll()
        {
            return _moduloContribuicoesContext.Codigoconta.ToList();
        }

        public Codigoconta Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var codigoconta = _moduloContribuicoesContext.Codigoconta
                .SingleOrDefault(u => u.Id == id);

            return codigoconta;
        }

        public CodigoContaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var codigoconta = _moduloContribuicoesContext.Codigoconta
                .SingleOrDefault(u => u.Id == id);

            CodigoContaDto codigocontaDto = Utils.MappClassToDto<Codigoconta, CodigoContaDto>(codigoconta);
            return codigocontaDto;
        }

        public void Add(Codigoconta entity)
        {
            _moduloContribuicoesContext.Codigoconta.Add(entity);
        }

        public void Update(Codigoconta entity)
        {
            Codigoconta entityToUpdate = _moduloContribuicoesContext.Codigoconta
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Codigoconta entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ValueCampoEditavelListagemResponse GetAllActiveTipoDeContaNivel1(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Codigoconta> queryCodigoContaAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            int parent = 0;
            if (int.TryParse(filter.filterField, out parent))
                queryCodigoContaAux = _moduloContribuicoesContext.Codigoconta
                    .Where(a => a.IndActivo && a.Designacao.Contains(filter.filterBy) && a.OrcamentoConfigFk == parent && a.ParentFk == null);
            else
                throw new Exception("No id was found for the entity parent");

            var queryCodigoConta = queryCodigoContaAux
                .Include(x => x.ParentFkNavigation)
                .Include(x => x.InverseParentFkNavigation)
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.Id,
                   Nome = u.Designacao,
                   ParentId = u.OrcamentoConfigFk,
                   HasKids = u.InverseParentFkNavigation.Count > 0,
                   Parametros = new List<ParametrosAdicionais>()
                   {
                        new ParametrosAdicionais
                        {
                            Nome = "CodigoTotal",
                            Size = "16",
                            Type = "number",
                            Valor = u.Codigo,
                            NaoVisivel = true
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "Codigo",
                            Size = "3",
                            Type = "string",
                            Valor = u.Codigo
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "ValorInicial",
                            Type = "currency",
                            Optional = true,
                            Valor = u.InitialValue.ToString()
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "DataValorInicial",
                            Type = "date",
                            Optional = true,
                            DateValor = u.InitialValueDate
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "Checkbox",
                            Type = "checkbox",
                            Optional = true,
                            Credit = u.IsCredit.GetValueOrDefault()
                        },
                   }
               });

            var codigoConta = queryCodigoConta
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryCodigoConta.Count();

            response.ValuesCampo = codigoConta;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public ValueCampoEditavelListagemResponse GetAllActiveTipoDeContaSubNivel(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Codigoconta> queryCodigoContaAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            int parent = 0;
            if (int.TryParse(filter.filterField, out parent))
                queryCodigoContaAux = _moduloContribuicoesContext.Codigoconta
                    .Include(c => c.ParentFkNavigation)
                    .Include(c => c.Relcodigocontaagrupamentoconfig)
                    .Where(a => a.IndActivo &&
                                // Filtrar por designação
                                a.Designacao.Contains(filter.filterBy) &&
                                a.ParentFk == parent);
            else
                throw new Exception("No id was found for the entity parent");

            var codigoConta = queryCodigoContaAux
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            List<ValorCamposEditaveis> result = new List<ValorCamposEditaveis>();
            int i = 0;
            foreach (Codigoconta c in codigoConta)
            {
                i++;
                StringBuilder nome = new StringBuilder();
                nome.Append(c.Codigo);
                Codigoconta r = c;
                // Agregar o código dos pais. ex: se o pai tiver o código 01 e o filho 02, vai ficar 0102 no filho.
                while (r.ParentFk != null)
                {
                    nome.Insert(0, r.ParentFkNavigation.Codigo);
                    r = _moduloContribuicoesContext.Codigoconta
                        .Include(c => c.ParentFkNavigation)
                        .Include(c => c.InverseParentFkNavigation)
                        .Where(c => c.Id == r.ParentFk).FirstOrDefault();
                }
                result.Add(new ValorCamposEditaveis
                {
                    Id = c.Id,
                    Nome = c.Designacao,
                    ParentId = c.ParentFk,
                    ParentHasInitialValue = c.ParentFkNavigation.InitialValue.GetValueOrDefault() > 0,
                    HasKids = c.InverseParentFkNavigation.Count > 0,
                    Parametros = new List<ParametrosAdicionais>()
                    {
                        new ParametrosAdicionais
                        {
                            Nome = "CodigoTotal",
                            Size = "16",
                            Type = "number",
                            Valor = nome.ToString(),
                            NaoVisivel = true
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "Codigo",
                            Size = "1",
                            Type = "number",
                            Valor = c.Codigo
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "ValorInicial",
                            Type = "currency",
                            Optional = true,
                            Valor = c.InitialValue.ToString()
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "DataValorInicial",
                            Type = "date",
                            Optional = true,
                            DateValor = c.InitialValueDate
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "Checkbox",
                            Type = "checkbox",
                            Optional = true,
                            Credit = c.IsCredit.GetValueOrDefault()
                        },
                    }
                });
            }

            response.ValuesCampo = result;
            response.CountValuesCampo = queryCodigoContaAux.Count();

            return response;
        }

        public List<SelectDescription> GetAllCodigoDeContaFirstLevelByParent(int parent)
        {
            int? parentOfParent = _moduloContribuicoesContext.Codigoconta
                .Where(u => u.Id == parent).Select(u => u.OrcamentoConfigFk).FirstOrDefault();

            if (!parentOfParent.HasValue)
                throw new Exception("No id was found for the entity parent of parent");

            return _moduloContribuicoesContext.Codigoconta
                .Where(u => u.IndActivo && u.OrcamentoConfigFk == parentOfParent.Value)
                .Select(u => new SelectDescription
                {
                    id = u.Id,
                    nome = u.Codigo + " " + u.Designacao,
                    parentId = u.OrcamentoConfigFk,
                    indActivo = u.IndActivo,
                    hasInitialValue = u.InitialValue.GetValueOrDefault() > 0,
                })
                .ToList();
        }

        public List<SelectDescription> GetAllCodigoDeContaByParent(int parent)
        {
            int? parentOfParent = _moduloContribuicoesContext.Codigoconta
                .Where(u => u.Id == parent).Select(u => u.ParentFk).FirstOrDefault();

            if (!parentOfParent.HasValue)
                throw new Exception("No id was found for the entity parent of parent");

            var codigoConta = _moduloContribuicoesContext.Codigoconta
                .Include(c => c.ParentFkNavigation)
                .Where(u => u.IndActivo && u.ParentFk == parentOfParent.Value)
                .ToList();

            List<SelectDescription> result = new List<SelectDescription>();
            int i = 0;
            foreach (Codigoconta c in codigoConta)
            {
                i++;
                StringBuilder nome = new StringBuilder();
                nome.Append(c.Codigo);
                Codigoconta r = c;
                // Agregar o código dos pais. ex: se o pai tiver o código 01 e o filho 02, vai ficar 0102 no filho.
                while (r.ParentFk != null)
                {
                    nome.Insert(0, r.ParentFkNavigation.Codigo);
                    r = _moduloContribuicoesContext.Codigoconta
                        .Include(c => c.ParentFkNavigation)
                        .Where(c => c.Id == r.ParentFk).FirstOrDefault();
                }
                result.Add(new SelectDescription
                {
                    id = c.Id,
                    nome = nome.ToString(),
                    parentId = c.ParentFk.Value,
                    indActivo = c.IndActivo,
                    hasInitialValue = c.InitialValue.GetValueOrDefault() > 0,
                });
            }

            return result;
        }

        public bool IsCodeValid(Codigoconta codigoConta)
        {
            int countSameCode = _moduloContribuicoesContext.Codigoconta
                 .Where(a => a.ParentFk == codigoConta.ParentFk 
                 && a.Codigo == codigoConta.Codigo 
                 && a.Id != codigoConta.Id && a.IndActivo == true
                 )
                 .Count();

            return countSameCode == 0;
        }

        public List<CodigoContaDataContract> GetAllActivCodigoContaByOrcamentoConfig(int orcamentoId)
        {
            Dictionary<int, Codigoconta> objectDictionary = _moduloContribuicoesContext.Codigoconta
            .Where(a => a.IndActivo)
            .Include(a => a.ParentFkNavigation)
            .Where(a => a.OrcamentoConfigFk == orcamentoId)
            .ToDictionary(a => a.Id);

            List<CodigoContaDataContract> result = new List<CodigoContaDataContract>();
            Codigoconta a;
            foreach (int key in objectDictionary.Keys)
            {
                a = objectDictionary[key];
                StringBuilder nome = new StringBuilder();
                nome.Append(GetFullCodigoTransactionless(a.Id, objectDictionary));
                nome.Append(" - ");
                nome.Append(a.Designacao);
                result.Add(
                    new CodigoContaDataContract
                    {
                        Id = a.Id,
                        Designacao = nome.ToString(),
                        ParentFk = a.ParentFk
                    });
            }

            return result;
        }

        public List<CodigoContaDataContract> GetAllActiveCodigoConta()
        {
            Dictionary<int, Codigoconta> objectDictionary = _moduloContribuicoesContext.Codigoconta
            .Where(a => a.IndActivo)
            .Include(a => a.ParentFkNavigation)
            .ToDictionary(a => a.Id);

            List<CodigoContaDataContract> result = new List<CodigoContaDataContract>();
            Codigoconta a;
            foreach (int key in objectDictionary.Keys)
            {
                a = objectDictionary[key];
                StringBuilder nome = new StringBuilder();
                nome.Append(GetFullCodigoTransactionless(a.Id, objectDictionary));
                nome.Append(" - ");
                nome.Append(a.Designacao);
                result.Add(
                    new CodigoContaDataContract
                    {
                        Id = a.Id,
                        Designacao = nome.ToString(),
                        ParentFk = a.ParentFk
                    });
            }

            return result;
        }

        public string GetFullCodigoTransactionless(int id, Dictionary<int, Codigoconta> codigosConta)
        {
            Codigoconta codigoConta = codigosConta[id];
            if (codigoConta != null)
            {
                StringBuilder nome = new StringBuilder();
                nome.Append(codigoConta.Codigo);
                // Agregar o código dos pais. ex: se o pai tiver o código 01 e o filho 02, vai ficar 0102 no filho.
                while (codigoConta.ParentFkNavigation != null)
                {
                    nome.Insert(0, codigoConta.ParentFkNavigation.Codigo);
                    codigoConta = codigosConta[codigoConta.ParentFk.Value];
                }
                return nome.ToString();
            }
            else
                return "";
        }

        public string GetFullCodigo(int id)
        {
            Codigoconta codigoConta = _moduloContribuicoesContext.Codigoconta
                        .Include(a => a.ParentFkNavigation)
                        .Where(a => a.Id == id).FirstOrDefault();
            if (codigoConta != null)
            {
                StringBuilder nome = new StringBuilder();
                nome.Append(codigoConta.Codigo);
                // Agregar o código dos pais. ex: se o pai tiver o código 01 e o filho 02, vai ficar 0102 no filho.
                while (codigoConta.ParentFkNavigation != null)
                {
                    nome.Insert(0, codigoConta.ParentFkNavigation.Codigo);
                    codigoConta = _moduloContribuicoesContext.Codigoconta
                        .Include(a => a.ParentFkNavigation)
                        .Where(a => a.Id == codigoConta.ParentFk).FirstOrDefault();
                }
                return nome.ToString();
            }
            else
                return "";
        }

        public string GetFullDesignacao(int id)
        {
            Codigoconta codigoConta = _moduloContribuicoesContext.Codigoconta
                        .Include(a => a.ParentFkNavigation)
                        .Where(a => a.Id == id).FirstOrDefault();
            if (codigoConta != null)
            {
                StringBuilder nome = new StringBuilder();
                nome.Append(codigoConta.Codigo);
                nome.Append("-");
                nome.Append(codigoConta.Designacao);
                nome.Append(" ");
                // Agregar o código e nome dos pais. ex: se o pai tiver o código e nome "01 - Nome1" e o filho "02 - Nome2", vai ficar "01-Nome1 02-Nome2" no filho.
                while (codigoConta.ParentFkNavigation != null)
                {
                    nome.Insert(0, codigoConta.ParentFkNavigation.Codigo + "-" + codigoConta.ParentFkNavigation.Designacao + " ");
                    codigoConta = _moduloContribuicoesContext.Codigoconta
                        .Include(a => a.ParentFkNavigation)
                        .Where(a => a.Id == codigoConta.ParentFk).FirstOrDefault();
                }
                return nome.ToString();
            }
            else
                return "";
        }

        public bool IsCodigoContaInUse(Codigoconta codigoConta)
        {
            var countUtilizados = _moduloContribuicoesContext.ComponentedespesaRegisto
                .Where(c => c.IndActivo && c.CodigoContaFk == codigoConta.Id).Count();

            return countUtilizados > 0;
        }

        public bool CodigoContaHasChilds(Codigoconta codigoConta)
        {
            var countFilhos = _moduloContribuicoesContext.Codigoconta
                .Where(c => c.IndActivo && c.ParentFk == codigoConta.Id).Count();

            return countFilhos > 0;
        }
    }
}