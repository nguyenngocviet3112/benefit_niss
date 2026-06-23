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
    public class KhachHangRepository : IKhachHangRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public KhachHangRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<KhachHang> GetAll()
        {
            return _moduloContribuicoesContext.KhachHang.ToList();
        }



        public KhachHang Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var actividadeeconomica = _moduloContribuicoesContext.KhachHang
                .SingleOrDefault(u => u.Id == id);

            return actividadeeconomica;
        }

        public KhachHangDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var actividadeeconomica = _moduloContribuicoesContext.KhachHang
                .SingleOrDefault(u => u.Id == id);

            KhachHangDto actividadeeconomicaDto = Utils.MappClassToDto<KhachHang, KhachHangDto>(actividadeeconomica);
            return actividadeeconomicaDto;
        }

        public void Add(KhachHang entity)
        {
            _moduloContribuicoesContext.KhachHang.Add(entity);
        }

        public void Update(KhachHang entity)
        {
            KhachHang entityToUpdate = _moduloContribuicoesContext.KhachHang
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(KhachHang entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectKhachHang> GetAllKhachHang()
        {
            return _moduloContribuicoesContext.KhachHang
               .Select(u => new SelectKhachHang{
                   id = u.Id,
                   HoTen = u.HoTen,
                   Email = u.Email,
                   DiemTichLuy = u.DiemTichLuy,
                   NgaySinh = u.NgaySinh
               })
                .ToList();
        }
    }
}