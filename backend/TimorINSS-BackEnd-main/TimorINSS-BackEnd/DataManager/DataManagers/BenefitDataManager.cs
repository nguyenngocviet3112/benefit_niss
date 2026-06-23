using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.ExcelReaderService.Models;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class BenefitDataManager : IBenefitDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public BenefitDataManager(IUnitOfWork unitOfWork,
                                      IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }
        public TrabalhadorViewResponse GetSingleByNiss(TrabalhadorListagemNissRequest request)
        {
            var response = new TrabalhadorViewResponse();
            var decoded = request.niss.Trim();
            //try
            //{
            //    var b64 = request.niss.ToString().Replace('-', '+').Replace('_', '/');
            //    switch (b64.Length % 4) { case 2: b64 += "=="; break; case 3: b64 += "="; break; }
            //    decoded = Encoding.UTF8.GetString(Convert.FromBase64String(b64)).Trim();

            //}
            //catch { /* tuỳ chọn: trả 400 nếu muốn fail cứng */ }

            Trabalhador trabalhador = _unitOfWork.TrabalhadoresRepository.GetByNiss(decoded);

            if (trabalhador == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.TrabalhadorDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.TrabalhadorDoesNotExist.ToString()
                });
            }
            else
            {
                response.Trabalhador = BuildObjectTrabalhadorDataContract(trabalhador);
                Relentidadetrabalhador rel = _unitOfWork.RelEntidadeTrabalhadorRepository.Get(trabalhador.IdTrabalhador);
                if (rel != null)
                {
                    response.RelEntidadeTrabalhador = BuildObjectRelEntidadeTrabalhadorDataContract(rel);
                }
                //List<INSSEstrangeiroDataContract> inssEstrangeiros = new List<INSSEstrangeiroDataContract>();
                //foreach (Inssestrangeiro inssEstrangeiro in trabalhador.Inssestrangeiro)
                //{
                //    inssEstrangeiros.Add(BuildObjectRelEntidadeTrabalhadorDataContract(inssEstrangeiro));
                //}
                //response.InssEstrangeiro = inssEstrangeiros;
                List<INSSCompanyStaffData> lstData = new List<INSSCompanyStaffData>();
                List<INSSCompanyStaffModel> lstCompany = _unitOfWork.TrabalhadoresRepository.GetCompanyStaffByNiss(decoded);
                double totalMonth = 0;
                double totalMoney = 0;
                foreach (INSSCompanyStaffModel inssEstrangeiro in lstCompany)
                {
                    lstData.Add(BuildObjectRelEntidadeTrabalhadorDataCompany(inssEstrangeiro));
                    totalMonth += inssEstrangeiro.ContributeMonth ?? 0;   // nếu null thì dùng 0
                    totalMoney += inssEstrangeiro.ContributeMoney ?? 0;   // nếu null thì dùng 0
                }
                response.INSSCompanyStaff = lstData;
             

                int years = (int)(totalMonth / 12);
                int months = (int)(totalMonth % 12);

                response.TotalMonth = months;
                response.TotalYear = years;
                response.TotalAmount = totalMoney;

            }
            return response;
        }

        public INSSCompanyStaffData BuildObjectRelEntidadeTrabalhadorDataCompany(INSSCompanyStaffModel inssEstrangeiro)
        {
            INSSCompanyStaffData inssEstrangeiroDataContract = new INSSCompanyStaffData();
  
            {
                inssEstrangeiroDataContract.Name = inssEstrangeiro.Name;
                inssEstrangeiroDataContract.StarDate = inssEstrangeiro.StartDate;
                inssEstrangeiroDataContract.EndDate = inssEstrangeiro.EndDate;
                inssEstrangeiroDataContract.ContributeMonth = inssEstrangeiro.ContributeMonth;
                inssEstrangeiroDataContract.ContributeMoney = inssEstrangeiro.ContributeMoney;
            };


            return inssEstrangeiroDataContract;
        }

        public INSSEstrangeiroDataContract BuildObjectRelEntidadeTrabalhadorDataContract(Inssestrangeiro inssEstrangeiro)
        {
            INSSEstrangeiroDataContract inssEstrangeiroDataContract = new INSSEstrangeiroDataContract();
            inssEstrangeiroDataContract = new INSSEstrangeiroDataContract
            {
                IdInssestrang = inssEstrangeiro.IdInssestrang,
                IdEntidade = inssEstrangeiro.EstrangeiroEntidadeFk,
                IdTrabalhador = inssEstrangeiro.EstrangeiroTrabalhadorFk,
                NomeSSEstrangeiro = inssEstrangeiro.NomeSsestrangeiro,
                EstrangeiroPaisFk = inssEstrangeiro.EstrangeiroPaisFk,
                IndDecontAtualmente = inssEstrangeiro.IndDecontAtualmente,
                IndBenfAtualmente = inssEstrangeiro.IndBenfAtualmente,
                Nissestrangeiro = inssEstrangeiro.Nissestrangeiro,
            };

            if (inssEstrangeiro.Documento?.Any() == true)
            {
                string doc = Convert.ToBase64String(inssEstrangeiro.Documento);
                inssEstrangeiroDataContract.NomeDocumento = inssEstrangeiro.NomeDocumento;
                inssEstrangeiroDataContract.Documento = doc;
            }

            return inssEstrangeiroDataContract;
        }

        public RelEntidadeTrabalhadorDataContract BuildObjectRelEntidadeTrabalhadorDataContract(Relentidadetrabalhador rel)
        {
            RelEntidadeTrabalhadorDataContract trabalhadorDataContract = new RelEntidadeTrabalhadorDataContract();
            if (rel != null)
                trabalhadorDataContract = new RelEntidadeTrabalhadorDataContract
                {
                    IdRelEntidadeTrabalhador = rel.IdRel,
                    EntidadeFk = rel.EntidadeFk,
                    TrabalhadorFk = rel.TrabalhadorFk,
                    TipoContrato = rel.TipoContrato,
                    NaturezaContrato = rel.NaturezaContrato,
                    LeiLabAplicavel = rel.LeiLabAplicavel,
                    Profissao = rel.Profissao,
                    HorasSemana = rel.HorasSemana,
                    DiasSemana = rel.DiasSemana,
                    DtIniVincTrabalhador = rel.DtIniVincTrabalhador,
                    DtIniFimTrabalhador = rel.DtIniFimTrabalhador,
                    FuncPublico = rel.FuncPublico,
                    NumFuncPublico = rel.NumFuncPublico,
                    RegimeFk = rel.RegimeFk,
                    EscalaoFk = rel.EscalaoFk,
                    ProfissaoOutro = rel.ProfissaoOutro
                };
            return trabalhadorDataContract;
        }
        public TrabalhadorDataContract BuildObjectTrabalhadorDataContract(Trabalhador trabalhador)
        {
            TrabalhadorDataContract trabalhadorDataContract = new TrabalhadorDataContract();
            if (trabalhador != null)
                trabalhadorDataContract = new TrabalhadorDataContract
                {
                    IdTrabalhador = trabalhador.IdTrabalhador,
                    Nome = trabalhador.Nome,
                    Tin = trabalhador.Tin,
                    DataNasc = trabalhador.DataNasc,
                    Sexo = trabalhador.SexoTrabalhador,
                    Nacionalidade = trabalhador.NacionalidadeTrabalhador,
                    Naturalidade = trabalhador.Naturalidade,
                    Niss = trabalhador.Niss,
                    NomeMae = trabalhador.NomeMae,
                    IndDescNomeMae = trabalhador.IndDescNomeMae,
                    NomePai = trabalhador.NomePai,
                    IndDescNomePai = trabalhador.IndDescNomePai,
                    EstadoCivil = trabalhador.EstadoCivil,
                    NumInscProvisoria = trabalhador.NumInscProvisoria
                };
            return trabalhadorDataContract;
        }
    }
}