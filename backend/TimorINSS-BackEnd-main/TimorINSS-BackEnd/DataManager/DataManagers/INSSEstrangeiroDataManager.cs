using System;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class INSSEstrangeiroDataManager : IINSSEstrangeiroDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public INSSEstrangeiroDataManager(IUnitOfWork unitOfWork,
                                          IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private Inssestrangeiro BuildINSSEstrangeiroObject(INSSEstrangeiroDataContract inssEstrangeiro)
        {
            InssestrangeiroDto newEntity = new InssestrangeiroDto
            {
                IdInssestrang = inssEstrangeiro.IdInssestrang,
                EstrangeiroEntidadeFk = inssEstrangeiro.IdEntidade,
                EstrangeiroTrabalhadorFk = inssEstrangeiro.IdTrabalhador,
                NomeSsestrangeiro = inssEstrangeiro.NomeSSEstrangeiro,
                EstrangeiroPaisFk = inssEstrangeiro.EstrangeiroPaisFk,
                IndDecontAtualmente = inssEstrangeiro.IndDecontAtualmente,
                IndBenfAtualmente = inssEstrangeiro.IndBenfAtualmente,
                Nissestrangeiro = inssEstrangeiro.Nissestrangeiro,
                IndActivo = true,
                UtilizadorCriacao = 0,
                DataCriacao = DateTime.Now,
                Ipv6 = ""
            };

            if (inssEstrangeiro.Documento?.Length > 0)
            {
                byte[] doc = Convert.FromBase64String(inssEstrangeiro.Documento);
                newEntity.NomeDocumento = inssEstrangeiro.NomeDocumento;
                newEntity.Documento = doc;
            }

            if (newEntity.IdInssestrang > 0)
            {
                Inssestrangeiro original = _unitOfWork.INSSEstrangeiroRepository.Get(newEntity.IdInssestrang);
                newEntity.UtilizadorCriacao = original.UtilizadorCriacao;
                newEntity.DataCriacao = original.DataCriacao;
                newEntity = _utils.UpdateDetailsToEntity<InssestrangeiroDto>(newEntity);
            }
            else
                newEntity = _utils.SetDetailsToEntity<InssestrangeiroDto>(newEntity);
            return Utils.MappClassFromDto<InssestrangeiroDto, Inssestrangeiro>(newEntity);
        }

        public ResponseBaseDataContract EditINSSEstrangeiro(INSSEstrangeiroRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //Build Objects
            Inssestrangeiro iNSSEstrangeiro = BuildINSSEstrangeiroObject(request.INSSEstrangeiro);

            //Insert in DB
            try
            {
                _unitOfWork.INSSEstrangeiroRepository.Update(iNSSEstrangeiro);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        public ResponseBaseDataContract SaveINSSEstrangeiro(INSSEstrangeiroRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //Build Objects
            Inssestrangeiro iNSSEstrangeiro = BuildINSSEstrangeiroObject(request.INSSEstrangeiro);

            //Insert in DB
            try
            {
                _unitOfWork.INSSEstrangeiroRepository.Add(iNSSEstrangeiro);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }
    }
}