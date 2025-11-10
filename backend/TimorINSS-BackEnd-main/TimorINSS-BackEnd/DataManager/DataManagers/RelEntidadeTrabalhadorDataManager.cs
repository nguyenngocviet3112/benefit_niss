using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class RelEntidadeTrabalhadorDataManager : IRelEntidadeTrabalhadorDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public RelEntidadeTrabalhadorDataManager(IUnitOfWork unitOfWork,
                                                 IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private Relentidadetrabalhador BuildRelEntidadeTrabalhadorObject(RelEntidadeTrabalhadorDataContract relEntidadeTrabalhador)
        {
            RelentidadetrabalhadorDto newEntity = new RelentidadetrabalhadorDto
            {
                IdRel = relEntidadeTrabalhador.IdRelEntidadeTrabalhador,
                EntidadeFk = relEntidadeTrabalhador.EntidadeFk,
                TrabalhadorFk = relEntidadeTrabalhador.TrabalhadorFk,
                TipoContrato = relEntidadeTrabalhador.TipoContrato,
                NaturezaContrato = relEntidadeTrabalhador.NaturezaContrato,
                LeiLabAplicavel = relEntidadeTrabalhador.LeiLabAplicavel,
                Profissao = relEntidadeTrabalhador.Profissao,
                HorasSemana = relEntidadeTrabalhador.HorasSemana,
                DiasSemana = relEntidadeTrabalhador.DiasSemana,
                DtIniVincTrabalhador = relEntidadeTrabalhador.DtIniVincTrabalhador,
                DtIniFimTrabalhador = relEntidadeTrabalhador.DtIniFimTrabalhador,
                FuncPublico = relEntidadeTrabalhador.FuncPublico,
                NumFuncPublico = relEntidadeTrabalhador.NumFuncPublico,
                RegimeFk = relEntidadeTrabalhador.RegimeFk,
                EscalaoFk = relEntidadeTrabalhador.EscalaoFk,
                FlagImportado = false,
                ProfissaoOutro = relEntidadeTrabalhador.ProfissaoOutro
            };
            newEntity = _utils.SetDetailsToEntity<RelentidadetrabalhadorDto>(newEntity);
            return Utils.MappClassFromDto<RelentidadetrabalhadorDto, Relentidadetrabalhador>(newEntity);
        }

        public ResponseBaseDataContract SaveRelEntidadeTrabalhador(RelEntidadeTrabalhadorRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            Relentidadetrabalhador relEntidadeTrabalhador = BuildRelEntidadeTrabalhadorObject(request.relEntidadeTrabalhador);
            if (!_utils.ValidateRelentidadetrabalhador(relEntidadeTrabalhador, _unitOfWork, out List<Error> errors))
            {
                response.Errors = errors;
                return response;
            }

            // A data de início de vinculo do trabalhador não pode ser inferior à data de ínicio de actividade da entidade Empregadora a que está vinculado
            bool insertRelEntidadeTrabalhador = true;
            var entidade = _unitOfWork.EntidadeEmpregadoraRepository.Get(request.relEntidadeTrabalhador.EntidadeFk);
            if (entidade != null && entidade.DataInicioActiv != null && relEntidadeTrabalhador.DtIniVincTrabalhador != null &&
                entidade.DataInicioActiv > relEntidadeTrabalhador.DtIniVincTrabalhador)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.InvalidDataInicioVinculo).ToString(),
                    ErrorMessage = ErrorsDataContract.InvalidDataInicioVinculo.ToString()
                });
                insertRelEntidadeTrabalhador = false;
            }

            if (insertRelEntidadeTrabalhador)
            {
                var dominio = _unitOfWork.DominioRepository.getTipoDeDominio(TiposDominio.NISSSEGURANCASOCIAL);
                if (dominio.descricao == entidade.Niss)
                {
                    var trabalhador = _unitOfWork.TrabalhadoresRepository.Get(relEntidadeTrabalhador.TrabalhadorFk);
                    var user = _unitOfWork.UtilizadoresRepository.GetByTrabalhadorFk(relEntidadeTrabalhador.TrabalhadorFk);
                    if (request.relEntidadeTrabalhador.DtIniVincTrabalhador <= DateTime.Now.Date && (request.relEntidadeTrabalhador.DtIniFimTrabalhador == null || request.relEntidadeTrabalhador.DtIniFimTrabalhador >= DateTime.Now.Date) && !trabalhador.Interno)
                    {
                        trabalhador.Interno = true;
                        if (user != null) user.Interno = true;
                    }
                    else if ((DateTime.Now.Date < request.relEntidadeTrabalhador.DtIniVincTrabalhador || (request.relEntidadeTrabalhador.DtIniFimTrabalhador != null && DateTime.Now.Date > request.relEntidadeTrabalhador.DtIniFimTrabalhador)) && trabalhador.Interno)
                    {
                        trabalhador.Interno = false;
                        if (user != null) user.Interno = false;
                    }
                }

            }

            try
            {
                if (insertRelEntidadeTrabalhador)
                {
                    _unitOfWork.RelEntidadeTrabalhadorRepository.Add(relEntidadeTrabalhador);
                    _unitOfWork.Commit();
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        public ResponseBaseDataContract DesvincularTrabalhador(DesvincularTrabalhadorRequest request)
        {
            DateTime end = request.DataFimdeVinculo;
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            RelentidadetrabalhadorDto relEntidadeTrabalhador = _unitOfWork.RelEntidadeTrabalhadorRepository.GetDto(request.IdRelEntidadeTrabalhador);
            relEntidadeTrabalhador.DtIniFimTrabalhador = end;
            relEntidadeTrabalhador = _utils.UpdateDetailsToEntity<RelentidadetrabalhadorDto>(relEntidadeTrabalhador);
            Relentidadetrabalhador relEntidadeTrabalhadorModel = Utils.MappClassFromDto<RelentidadetrabalhadorDto, Relentidadetrabalhador>(relEntidadeTrabalhador);

            if (!_utils.ValidateRelentidadetrabalhador(relEntidadeTrabalhadorModel, _unitOfWork, out List<Error> errors))
            {
                response.Errors = errors;
                return response;
            }

            if (end < DateTime.Now.Date)
            {
                var dominio = _unitOfWork.DominioRepository.getTipoDeDominio(TiposDominio.NISSSEGURANCASOCIAL);
                var entidade = _unitOfWork.EntidadeEmpregadoraRepository.Get(relEntidadeTrabalhador.EntidadeFk);
                if (dominio.descricao == entidade.Niss)
                {
                    var trabalhador = _unitOfWork.TrabalhadoresRepository.Get(relEntidadeTrabalhador.TrabalhadorFk);
                    var user = _unitOfWork.UtilizadoresRepository.GetByTrabalhadorFk(relEntidadeTrabalhador.TrabalhadorFk);
                    if (trabalhador.Interno == true)    
                    {
                        trabalhador.Interno = false;
                        if (user != null) user.Interno = false;
                    }
                }

            }

            try
            {
                _unitOfWork.RelEntidadeTrabalhadorRepository.Update(relEntidadeTrabalhadorModel);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        public ResponseBaseDataContract EditRelEntidadeTrabalhador(RelEntidadeTrabalhadorRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //Build Objects
            Relentidadetrabalhador relEntidadeTrabalhador = BuildRelEntidadeTrabalhadorObject(request.relEntidadeTrabalhador);

            if (!_utils.ValidateRelentidadetrabalhador(relEntidadeTrabalhador, _unitOfWork, out List<Error> errors))
            {
                response.Errors = errors;
                return response;
            }

            // A data de início de vinculo do trabalhador não pode ser inferior à data de ínicio de actividade da entidade Empregadora a que está vinculado
            bool updateRelEntidadeTrabalhador = true;
            Entidadeempregadora entidade = new Entidadeempregadora();
            entidade = _unitOfWork.EntidadeEmpregadoraRepository.Get(request.relEntidadeTrabalhador.EntidadeFk);
            if (entidade != null && entidade.DataInicioActiv != null && relEntidadeTrabalhador.DtIniVincTrabalhador != null &&
                entidade.DataInicioActiv > relEntidadeTrabalhador.DtIniVincTrabalhador)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.InvalidDataInicioVinculo).ToString(),
                    ErrorMessage = ErrorsDataContract.InvalidDataInicioVinculo.ToString()
                });
                updateRelEntidadeTrabalhador = false;
            }

            if (updateRelEntidadeTrabalhador)
            {
                var dominio = _unitOfWork.DominioRepository.getTipoDeDominio(TiposDominio.NISSSEGURANCASOCIAL);
                if (dominio.descricao == entidade.Niss)
                {
                    var trabalhador = _unitOfWork.TrabalhadoresRepository.Get(relEntidadeTrabalhador.TrabalhadorFk);
                    var user = _unitOfWork.UtilizadoresRepository.GetByTrabalhadorFk(relEntidadeTrabalhador.TrabalhadorFk);
                    if (request.relEntidadeTrabalhador.DtIniVincTrabalhador <= DateTime.Now.Date && (request.relEntidadeTrabalhador.DtIniFimTrabalhador == null || request.relEntidadeTrabalhador.DtIniFimTrabalhador >= DateTime.Now.Date) && !trabalhador.Interno)
                    {
                        trabalhador.Interno = true;
                        if (user != null) user.Interno = true;
                    }
                    else if ((DateTime.Now.Date < request.relEntidadeTrabalhador.DtIniVincTrabalhador || (request.relEntidadeTrabalhador.DtIniFimTrabalhador != null && DateTime.Now.Date > request.relEntidadeTrabalhador.DtIniFimTrabalhador)) && trabalhador.Interno)
                    {
                        trabalhador.Interno = false;
                        if (user != null) user.Interno = false;
                    }
                }

            }

            //Insert in DB
            try
            {
                if (updateRelEntidadeTrabalhador)
                {
                    _unitOfWork.RelEntidadeTrabalhadorRepository.Update(relEntidadeTrabalhador);
                    _unitOfWork.Commit();
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        public ResponseBaseDataContract EditRelEntidadeTrabalhadorRegime(RelEntidadeTrabalhadorRegimeRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //Build Objects
            Relentidadetrabalhador relEntidadeTrabalhadorModel = _unitOfWork.RelEntidadeTrabalhadorRepository.Get(request.relEntidadeTrabalhadorRegime.IdRel);

            if (relEntidadeTrabalhadorModel == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ContratoDoesNotExists).ToString(),
                    ErrorMessage = ErrorsDataContract.ContratoDoesNotExists.ToString()
                });
                return response;
            }

            RelentidadetrabalhadorDto relEntidadeTrabalhador = Utils.MappClassToDto<Relentidadetrabalhador, RelentidadetrabalhadorDto>(relEntidadeTrabalhadorModel);
            relEntidadeTrabalhador.EscalaoFk = request.relEntidadeTrabalhadorRegime.EscalaoFk;
            relEntidadeTrabalhador.RegimeFk = request.relEntidadeTrabalhadorRegime.RegimeFk;
            relEntidadeTrabalhador = _utils.UpdateDetailsToEntity(relEntidadeTrabalhador);

            Relentidadetrabalhador relEntidadeTrabalhadorFinal = Utils.MappClassFromDto<RelentidadetrabalhadorDto, Relentidadetrabalhador>(relEntidadeTrabalhador);

            if (!_utils.ValidateRelentidadetrabalhador(relEntidadeTrabalhadorFinal, _unitOfWork, out List<Error> errors))
            {
                response.Errors = errors;
                return response;
            }
            //Insert in DB
            try
            {
                _unitOfWork.RelEntidadeTrabalhadorRepository.Update(relEntidadeTrabalhadorFinal);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
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

        public TrabalhadorViewResponse GetTrabalhadorViewById(TrabalhadorListagemRequest request)
        {
            var response = new TrabalhadorViewResponse();
            var b64 = request.idStr.ToString().Replace('-', '+').Replace('_', '/');
            switch (b64.Length % 4) { case 2: b64 += "=="; break; case 3: b64 += "="; break; }
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(b64)).Trim();

            int decodedId = int.Parse(decoded);
            const int SECRET_A = 12;
            const int SECRET_B = 123456789;
            decodedId = (decodedId - SECRET_B) / SECRET_A;
            Relentidadetrabalhador rel = _unitOfWork.RelEntidadeTrabalhadorRepository.Get(decodedId);
            if (rel != null)
            {
                response.RelEntidadeTrabalhador = BuildObjectRelEntidadeTrabalhadorDataContract(rel);

                Trabalhador trabalhador = _unitOfWork.TrabalhadoresRepository.GetTrabalhadorViewById(rel.TrabalhadorFk);
                if (trabalhador != null)
                {
                    response.Trabalhador = BuildObjectTrabalhadorDataContract(trabalhador);
                    List<INSSEstrangeiroDataContract> inssEstrangeiros = new List<INSSEstrangeiroDataContract>();
                    foreach (Inssestrangeiro inssEstrangeiro in trabalhador.Inssestrangeiro)
                    {
                        inssEstrangeiros.Add(BuildObjectRelEntidadeTrabalhadorDataContract(inssEstrangeiro));
                    }
                    response.InssEstrangeiro = inssEstrangeiros;
                }
                else
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.TrabalhadorDoesNotExist).ToString(),
                        ErrorMessage = ErrorsDataContract.TrabalhadorDoesNotExist.ToString()
                    });
                }
            }
            else
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ContratoDoesNotExists).ToString(),
                    ErrorMessage = ErrorsDataContract.ContratoDoesNotExists.ToString()
                });
                return response;
            }

            return response;
        }
    }
}