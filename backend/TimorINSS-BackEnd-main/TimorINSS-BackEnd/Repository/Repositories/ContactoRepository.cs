using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ContactoRepository : IContactoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ContactoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Contacto> GetAll()
        {
            return _moduloContribuicoesContext.Contacto.Where(u => u.IndActivo).ToList();
        }

        public Contacto Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var contacto = _moduloContribuicoesContext.Contacto
                .SingleOrDefault(u => u.IdContacto == id);

            return contacto;
        }

        public ContactoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var contacto = _moduloContribuicoesContext.Contacto
                .SingleOrDefault(u => u.IdContacto == id);

            ContactoDto contactoDto = Utils.MappClassToDto<Contacto, ContactoDto>(contacto);
            return contactoDto;
        }

        public void Add(Contacto entity)
        {
            _moduloContribuicoesContext.Contacto.Add(entity);
        }

        public void Update(Contacto entity)
        {
            Contacto entityToUpdate = _moduloContribuicoesContext.Contacto
                .Single(d => d.IdContacto == entity.IdContacto);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Contacto entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ContatoListagemResponse GetContactosByFilter(ContatoListagemRequest request)
        {
            ContatoListagemResponse result = new ContatoListagemResponse();
            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;
            var b64 = request.IdStr.ToString().Replace('-', '+').Replace('_', '/');
            switch (b64.Length % 4) { case 2: b64 += "=="; break; case 3: b64 += "="; break; }
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(b64)).Trim();

            int decodedId = int.Parse(decoded);
            const int SECRET_A = 12;
            const int SECRET_B = 123456789;
            decodedId = (decodedId - SECRET_B) / SECRET_A;

            IQueryable<Contacto> queryContatosConditional = _moduloContribuicoesContext.Contacto;
            switch (request.filter.filterField)
            {
                case "ENTIDADEEMPREGADORA":
                    queryContatosConditional = queryContatosConditional.Where(u => u.ContactoEntidadeFk == decodedId && u.IndActivo);
                    break;

                case "TRABALHADOR":
                    queryContatosConditional = queryContatosConditional.Where(u => u.ContactoTrabalhadorFk == decodedId && u.IndActivo);
                    break;

                default:
                    result.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.InvalidFilter).ToString(),
                        ErrorMessage = ErrorsDataContract.InvalidFilter.ToString()
                    });
                    return result;
            }

            var queryContatos = queryContatosConditional
                .Select(contato => new ContatoListagem
                {
                    idContato = contato.IdContacto,
                    idEntidade = contato.ContactoEntidadeFk,
                    idTrabalhador = contato.ContactoTrabalhadorFk,
                    telemovel = contato.Telemovel,
                    email = contato.Email
                });

            var contacto = queryContatos
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryContatos.Count();

            result.contato = contacto;
            result.rows = totalNumber;
            return result;
        }

        public ContactoDto GetDtoByEmail(string email, long trabalhadorId)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var contacto = _moduloContribuicoesContext.Contacto
                .SingleOrDefault(u => u.Email == email && u.ContactoEntidadeFk == trabalhadorId);

            ContactoDto contactoDto = Utils.MappClassToDto<Contacto, ContactoDto>(contacto);
            return contactoDto;
        }

        public ContactoDto GetInternalDtoByEmailAndWorkerId(string email, long workerId)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var contacto = _moduloContribuicoesContext.Contacto
                .SingleOrDefault(u => u.Email == email && u.ContactoTrabalhadorFk == workerId);

            ContactoDto contactoDto = Utils.MappClassToDto<Contacto, ContactoDto>(contacto);
            return contactoDto;
        }

        public List<Contacto> GetByTrabalhadorFkEntidadeFk(int? trabalhadorFk, int? entidadeFk)
        {
            int? trabalhadorId = null;
            int? entidadeId = null;
            if (trabalhadorFk > 0)
            {
                trabalhadorId = trabalhadorFk;
            }
            else if (entidadeFk > 0)
            {
                entidadeId = entidadeFk;
            }
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var contacto = _moduloContribuicoesContext.Contacto
                .Where(u => u.ContactoTrabalhadorFk == trabalhadorId && u.ContactoEntidadeFk == entidadeId
                && u.IndActivo).ToList();

            return contacto;
        }
    }
}