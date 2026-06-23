using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class RelCodigoContaAgrupamentoConfigRepository : IRelCodigoContaAgrupamentoConfigRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelCodigoContaAgrupamentoConfigRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Relcodigocontaagrupamentoconfig> GetAll()
        {
            return _moduloContribuicoesContext.Relcodigocontaagrupamentoconfig.ToList();
        }

        public Relcodigocontaagrupamentoconfig Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relcodigocontaagrupamentoconfig = _moduloContribuicoesContext.Relcodigocontaagrupamentoconfig
                .SingleOrDefault(u => u.Id == id);

            return relcodigocontaagrupamentoconfig;
        }

        public RelCodigoContaAgrupamentoConfigDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var relcodigocontaagrupamentoconfig = _moduloContribuicoesContext.Relcodigocontaagrupamentoconfig
                .SingleOrDefault(u => u.Id == id);

            RelCodigoContaAgrupamentoConfigDto relCodigoContaAgrupamentoConfigDto = Utils.MappClassToDto<Relcodigocontaagrupamentoconfig, RelCodigoContaAgrupamentoConfigDto>(relcodigocontaagrupamentoconfig);
            return relCodigoContaAgrupamentoConfigDto;
        }

        public void Add(Relcodigocontaagrupamentoconfig entity)
        {
            _moduloContribuicoesContext.Relcodigocontaagrupamentoconfig.Add(entity);
        }

        public void Update(Relcodigocontaagrupamentoconfig entity)
        {
            Relcodigocontaagrupamentoconfig entityToUpdate = _moduloContribuicoesContext.Relcodigocontaagrupamentoconfig
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Relcodigocontaagrupamentoconfig entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Relcodigocontaagrupamentoconfig> GetAllByCodigoConta(int id)
        {
            return _moduloContribuicoesContext.Relcodigocontaagrupamentoconfig
                .Where(r => r.IndActivo && r.CodigocontaFk == id).ToList();
        }

        public List<AgrupamentoConfigDataContract> GetAgrupamentoConfigByIdCodigoContaTipoConta(int codigoContaFK, int tipoContaFk)
        {
            Dictionary<int, Agrupamentoconfig> objectDictionary = _moduloContribuicoesContext.Relcodigocontaagrupamentoconfig
                .Include(r => r.AgrupamentoConfigFkNavigation)
                .Include(r => r.AgrupamentoConfigFkNavigation.ParentFkNavigation)
                .Include(r => r.AgrupamentoConfigFkNavigation.InverseParentFkNavigation)
                .Include(r => r.AgrupamentoConfigFkNavigation.ReltipoDeContaOrcamentoConfigFkNavigation)
                .Where(r => r.IndActivo &&
                            r.CodigocontaFk == codigoContaFK &&
                            r.AgrupamentoConfigFkNavigation.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk == tipoContaFk
                )
                .Select(agrupamento => new Agrupamentoconfig
                {
                    Id = agrupamento.AgrupamentoConfigFkNavigation.Id,
                    Codigo = agrupamento.AgrupamentoConfigFkNavigation.Codigo,
                    Designacao = agrupamento.AgrupamentoConfigFkNavigation.Designacao,
                    ParentFk = agrupamento.AgrupamentoConfigFkNavigation.ParentFk,
                    ParentFkNavigation = agrupamento.AgrupamentoConfigFkNavigation.ParentFkNavigation
                })
                .ToDictionary(r => r.Id);

            List<AgrupamentoConfigDataContract> result = new List<AgrupamentoConfigDataContract>();
            Agrupamentoconfig a;
            foreach (int key in objectDictionary.Keys)
            {
                a = objectDictionary[key];
                StringBuilder nome = new StringBuilder();
                nome.Append(GetFullCodigoTransactionless(a.Id, objectDictionary));
                nome.Append(" - ");
                nome.Append(a.Designacao);
                result.Add(
                    new AgrupamentoConfigDataContract
                    {
                        Id = a.Id,
                        Designacao = nome.ToString(),
                        ParentFk = a.ParentFk
                    });
            }

            return result;
        }

        public string GetFullCodigoTransactionless(int id, Dictionary<int, Agrupamentoconfig> agrupamentos)
        {
            Agrupamentoconfig agrupamento = agrupamentos[id];
            if (agrupamento != null)
            {
                StringBuilder nome = new StringBuilder();
                nome.Append(agrupamento.Codigo);
                // Agregar o código dos pais. ex: se o pai tiver o código 01 e o filho 02, vai ficar 0102 no filho.
                while (agrupamento.ParentFkNavigation != null)
                {
                    nome.Insert(0, agrupamento.ParentFkNavigation.Codigo);
                    agrupamento = agrupamentos[agrupamento.ParentFk.Value];
                }
                return nome.ToString();
            }
            else
                return "";
        }

        public bool IsRelBeingUsed(Relcodigocontaagrupamentoconfig rel)
        {
            return _moduloContribuicoesContext.ComponentedespesaRegisto
                .Any(c => c.IndActivo &&
                            c.CodigoContaFk == rel.CodigocontaFk &&
                            c.AgrupamentoConfigFk == rel.AgrupamentoConfigFk
                );
        }
    }
}