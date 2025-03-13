using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class KhachHangDataManager : IKhachHangDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public KhachHangDataManager(IUnitOfWork unitOfWork)
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


        public KhachHangDto GetDto(int id)
        {
            return _unitOfWork.KhachHangRepository.GetDto(id);
        }
    }
}