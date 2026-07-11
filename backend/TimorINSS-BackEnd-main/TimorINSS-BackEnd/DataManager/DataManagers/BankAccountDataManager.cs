using System;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    // Dedicated new-mode CRUD for the existing Contabancaria table (reused
    // as-is — no new table). Lets Cấu hình hệ thống manage the bank-account
    // list that Receita/Pagamento pick from, without going through the old
    // generic "Gerir Campos Editáveis" engine.
    public class BankAccountDataManager : IBankAccountDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public BankAccountDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public BankAccountListResponse GetAll()
        {
            BankAccountListResponse response = new BankAccountListResponse();
            try
            {
                response.Items = _unitOfWork.ContaBancariaRepository.GetAllDto(false)
                    .Select(c => new BankAccountDataContract
                    {
                        Id = c.Id,
                        EntidadeBancaria = c.EntidadeBancaria,
                        Descricao = c.Descricao,
                        Swift = c.Swift,
                        Iban = c.Iban,
                        Numero = c.Numero
                    })
                    .OrderBy(c => c.EntidadeBancaria)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public SaveBankAccountResponse Save(SaveBankAccountRequest request)
        {
            SaveBankAccountResponse response = new SaveBankAccountResponse { RequestId = request.RequestId };
            try
            {
                if (string.IsNullOrWhiteSpace(request.EntidadeBancaria) || string.IsNullOrWhiteSpace(request.Iban))
                {
                    response.Errors.Add(new Error { ErrorCode = "BANKACC-MISSING-FIELDS", ErrorMessage = "Tên ngân hàng và IBAN là bắt buộc." });
                    return response;
                }

                Contabancaria entity;
                if (request.Id > 0)
                {
                    entity = _unitOfWork.ContaBancariaRepository.Get(request.Id);
                    if (entity == null)
                    {
                        response.Errors.Add(new Error { ErrorCode = "BANKACC-NOT-FOUND", ErrorMessage = "Không tìm thấy tài khoản ngân hàng." });
                        return response;
                    }
                }
                else
                {
                    entity = new Contabancaria { Id = 0 };
                }

                entity.EntidadeBancaria = request.EntidadeBancaria;
                entity.Descricao = request.Descricao;
                entity.Swift = request.Swift;
                entity.Iban = request.Iban;
                entity.Numero = request.Numero;

                if (!_unitOfWork.ContaBancariaRepository.IsIbanValid(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "BANKACC-DUP-IBAN", ErrorMessage = "IBAN đã tồn tại ở tài khoản khác." });
                    return response;
                }

                if (request.Id > 0)
                {
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.ContaBancariaRepository.Update(entity);
                }
                else
                {
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.ContaBancariaRepository.Add(entity);
                }
                _unitOfWork.Commit();

                response.Id = entity.Id;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }
    }
}
