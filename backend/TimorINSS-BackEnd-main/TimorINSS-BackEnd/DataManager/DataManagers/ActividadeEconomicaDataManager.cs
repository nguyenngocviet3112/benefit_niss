using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ActividadeEconomicaDataManager : IActividadeEconomicaDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActividadeEconomicaDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SelectKhachHangResponse GetAllKhachHang()
        {
            SelectKhachHangResponse response = new SelectKhachHangResponse();
            try
            {
                // vai buscar todas as actividades econónicas ativas, existentes na Base de dados. tabela - KHACH HANG
                List<SelectKhachHang> selects = _unitOfWork.KhachHangRepository.GetAllKhachHang();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public SelectDescriptionResponse GetAllActividadeEconomica()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                // vai buscar todas as actividades econónicas ativas, existentes na Base de dados. tabela - ACTIVIDADEECONOMICA
                List<SelectDescription> selects = _unitOfWork.ActividadeEconomicaRepository.GetAllActividadeEconomica();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ActividadeeconomicaDto GetDto(int id)
        {
            return _unitOfWork.ActividadeEconomicaRepository.GetDto(id);
        }
    }
}