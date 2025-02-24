using System;
using System.Collections.Generic;
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
    public class ResponsavelLegalDataManager : IResponsavelLegalDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ResponsavelLegalDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        #region Add Responsavel Legal

        public ResponseBaseDataContract AddResponsavelLegal(ResponsavelLegalRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            List<Documentoidentificacao> documentosList = new List<Documentoidentificacao>();
            Trabalhador trabalhador = null;

            if (request.DataInicioFuncao > request.DataFimFuncao)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DateFromSuperiorThanDateTo).ToString(),
                    ErrorMessage = ErrorsDataContract.DateFromSuperiorThanDateTo.ToString()
                });
            }

            //Get trabalhador to associate, if id is present, than it was validate by previous page, if Niss, then he is linking on new insert
            if (request.IdTrabalhador != 0)
                trabalhador = _unitOfWork.TrabalhadoresRepository.Get(request.IdTrabalhador);
            else if (request.Niss != null)
            {
                trabalhador = _unitOfWork.TrabalhadoresRepository.GetByNiss(request.Niss);
                //If Niss is not a match, send error back
                if (trabalhador == null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.TrabalhadorDoesNotExist).ToString(),
                        ErrorMessage = ErrorsDataContract.TrabalhadorDoesNotExist.ToString()
                    });
                }
                else //Check if there is a duplicate
                {
                    var duplicateRespLegal = _unitOfWork.ResponsavelLegalRepository.GetByTrabalhadorId(trabalhador.IdTrabalhador);
                    if (duplicateRespLegal != null)
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.TrabalhadorLinkedToRespLegal).ToString(),
                            ErrorMessage = ErrorsDataContract.TrabalhadorLinkedToRespLegal.ToString()
                        });
                    }
                }
            }

            //Validate Existing Tin
            var existingRespLegalTin = _unitOfWork.ResponsavelLegalRepository.GetByTin(request.ResponsavelLegal.Tin);
            if (existingRespLegalTin != null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.TinAlreadyExists).ToString(),
                    ErrorMessage = ErrorsDataContract.TinAlreadyExists.ToString()
                });
            }

            if (response.Errors.Count > 0)
                return response;

            //Validate if trabalhador exists
            if (trabalhador != null)
            {
                var trabalhadorDto = Utils.MappClassToDto<Trabalhador, TrabalhadorDto>(trabalhador);
                //Fetch all documents related to trabalhador
                documentosList = _unitOfWork.DocumentoIdentificacaoRepository.GetByTrabalhadorEntidade(request.IdTrabalhador);
                //Save all data from trabalhador to responsavel legal
                request = FillDetailsFromTrabalhador(request, trabalhadorDto);
            }
            else
            {
                foreach (var documento in request.DocumentoIdentificacao)
                {
                    documentosList.Add(BuildDocumentoidentificacaoObject(documento));
                }
            }

            //Build Objects
            Responsavellegal responsavellegal = BuildResponsavelLegalObject(request);
            Relentidaderesplegal relentidaderesplegal = BuildRelentidaderesplegalObject(request);

            // Relate Objects to each other
            responsavellegal.Relentidaderesplegal.Add(relentidaderesplegal);

            //Insert in DB
            try
            {
                _unitOfWork.ResponsavelLegalRepository.Add(responsavellegal);

                foreach (var documento in documentosList)
                {
                    documento.RespLegalDocumentoFkNavigation = responsavellegal;
                    if (trabalhador != null)
                        _unitOfWork.DocumentoIdentificacaoRepository.Update(documento);
                    else
                        _unitOfWork.DocumentoIdentificacaoRepository.Add(documento);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        #region Builders

        private ResponsavelLegalRequest FillDetailsFromTrabalhador(ResponsavelLegalRequest request, TrabalhadorDto trabalhador)
        {
            if (trabalhador != null)
            {
                request.ResponsavelLegal.Nome = trabalhador.Nome;
                request.ResponsavelLegal.Tin = trabalhador.Tin;
                request.ResponsavelLegal.DataNascimento = trabalhador.DataNasc;
                request.ResponsavelLegal.Sexo = trabalhador.SexoTrabalhador;
                request.ResponsavelLegal.Nacionalidade = trabalhador.NacionalidadeTrabalhador;
                request.ResponsavelLegal.Naturalidade = trabalhador.Naturalidade;
                request.ResponsavelLegal.RespLegalTabalhadorFk = trabalhador.IdTrabalhador;
            }

            return request;
        }

        private Responsavellegal BuildResponsavelLegalObject(ResponsavelLegalRequest request)
        {
            var responsavellegalDto = new ResponsavellegalDto
            {
                Nome = request.ResponsavelLegal.Nome,
                Tin = request.ResponsavelLegal.Tin,
                DataNasc = request.ResponsavelLegal.DataNascimento,
                Nacionalidade = request.ResponsavelLegal.Nacionalidade,
                Naturalidade = request.ResponsavelLegal.Naturalidade,
                Sexo = request.ResponsavelLegal.Sexo,
                Funcao = request.ResponsavelLegal.Funcao,
                FuncaoOutro = request.ResponsavelLegal.FuncaoOutro,
                IndFuncaoRem = request.ResponsavelLegal.IndFuncaoRem,
                RespLegalTabalhadorFk = request.ResponsavelLegal.RespLegalTabalhadorFk,
                IndActivo = true,
                FlagImportado = false
            };

            //Set common attributes for dtos
            responsavellegalDto = _utils.SetDetailsToEntity(responsavellegalDto);

            //Map Dto to Domain objects
            return Utils.MappClassFromDto<ResponsavellegalDto, Responsavellegal>(responsavellegalDto);
        }

        private Relentidaderesplegal BuildRelentidaderesplegalObject(ResponsavelLegalRequest request)
        {
            var relentidaderesplegalDto = new RelentidaderesplegalDto
            {
                RelEntidadeRespFk = request.IdEntidade,
                DataInicioFuncao = request.DataInicioFuncao,
                DataFimFuncao = request.DataFimFuncao,
                IndActivo = true
            };
            //Set common attributes for dtos
            relentidaderesplegalDto = _utils.SetDetailsToEntity(relentidaderesplegalDto);

            //Map Dto to Domain objects
            return Utils.MappClassFromDto<RelentidaderesplegalDto, Relentidaderesplegal>(relentidaderesplegalDto);
        }

        private Documentoidentificacao BuildDocumentoidentificacaoObject(DocumentoIdentificacaoDataContract documento)
        {
            // Transform file document to bit array
            byte[] doc = Convert.FromBase64String(documento.Documento);

            var documentoidentificacaoDto = new DocumentoidentificacaoDto
            {
                TpDocIdentificacao = documento.TpDocIdentificacao,
                Numero = documento.Numero,
                LocalEmissao = documento.LocalEmissao,
                DataEmissao = documento.DataEmissao,
                DataValidade = documento.DataValidade,
                Documento = doc,
                NomeDocumento = documento.NomeDocumento,
                IndActivo = true,
                FlagImportado = false
            };

            //Set common attributes for dtos
            documentoidentificacaoDto = _utils.SetDetailsToEntity(documentoidentificacaoDto);

            //Map Dto to Domain objects
            return Utils.MappClassFromDto<DocumentoidentificacaoDto, Documentoidentificacao>(documentoidentificacaoDto);
        }

        #endregion Builders

        #endregion Add Responsavel Legal

        #region List Responsavel Legal

        public SingleResponsavelLegalResponse GetById(ResponsavelLegalListagemRequest request)
        {
            var response = new SingleResponsavelLegalResponse();
            try
            {
                var resposavelLegal = _unitOfWork.ResponsavelLegalRepository.GetDto(request.id);
                var relEntidadeResponsavelLegal = _unitOfWork.RelEntidadeResponsavelLegalRepository.GetDtoByResponsavelLegal(request.id);
                var domainDocumentos = _unitOfWork.DocumentoIdentificacaoRepository.GetByResponsavelLegalEntidade(request.id);
                var relDocumentoResponsavelLegal = DocumentoContractBuilder(domainDocumentos);
                TrabalhadorDto trabalhador = null;

                if (resposavelLegal.RespLegalTabalhadorFk.HasValue)
                {
                    trabalhador = _unitOfWork.TrabalhadoresRepository.GetDto(resposavelLegal.RespLegalTabalhadorFk.Value);
                }

                if (resposavelLegal != null)
                {
                    response.DataInicioFuncao = relEntidadeResponsavelLegal.DataInicioFuncao;
                    response.DataFimFuncao = relEntidadeResponsavelLegal.DataFimFuncao;
                    if (trabalhador != null)
                    {
                        response.Niss = trabalhador.Niss ?? trabalhador.NumInscProvisoria.ToString();
                    }

                    response.responsavelLegal = new ResponsavelLegalDataContract
                    {
                        IdResponsavelLegal = resposavelLegal.IdResponsavelLegal,
                        Nome = resposavelLegal.Nome,
                        Tin = resposavelLegal.Tin,
                        DataNascimento = resposavelLegal.DataNasc,
                        Sexo = resposavelLegal.Sexo,
                        Nacionalidade = resposavelLegal.Nacionalidade,
                        Naturalidade = resposavelLegal.Naturalidade,
                        Funcao = resposavelLegal.Funcao,
                        FuncaoOutro = resposavelLegal.FuncaoOutro,
                        IndFuncaoRem = resposavelLegal.IndFuncaoRem,
                        RespLegalTabalhadorFk = resposavelLegal.RespLegalTabalhadorFk
                    };
                    response.DocumentoIdentificacao = relDocumentoResponsavelLegal;
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        #region Contract builder

        private List<DocumentoIdentificacaoDataContract> DocumentoContractBuilder(List<Documentoidentificacao> domainList)
        {
            List<DocumentoIdentificacaoDataContract> documentDtoList = new List<DocumentoIdentificacaoDataContract>();

            foreach (var document in domainList)
            {
                documentDtoList.Add(new DocumentoIdentificacaoDataContract
                {
                    IdDocumento = document.IdDocIdentificacao,
                    IdTrabalhador = document.TrabalhadorDocumetoFk,
                    IdResponsavelLegal = document.RespLegalDocumentoFk,
                    TpDocIdentificacao = document.TpDocIdentificacao,
                    Numero = document.Numero,
                    DataValidade = document.DataValidade,
                    DataEmissao = document.DataEmissao,
                    LocalEmissao = document.LocalEmissao,
                    Documento = Convert.ToBase64String(document.Documento),
                    NomeDocumento = document.NomeDocumento
                });
            }

            return documentDtoList;
        }

        public ResponsavelLegalListagemResponse GetByIdEntidadeEmpregadora(ResponsavelLegalListagemRequest request)
        {
            ResponsavelLegalListagemResponse response = new ResponsavelLegalListagemResponse();
            try
            {
                if (request.filter != null)
                {
                    bool valid = false;
                    var userId = request.UserId;
                    var user = _unitOfWork.UtilizadoresRepository.Get(userId);
                    var entidadeEmpregadoraId = user.UtilizadorEntidadeFk;
                    var funcaoDominioList = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.FUNCAO);

                    // Validar se o utilizador tem as permissões necessárias
                    if (entidadeEmpregadoraId.HasValue || user.Interno == true)
                        valid = true;

                    if (valid)
                    {
                        response = _unitOfWork.ResponsavelLegalRepository.GetByIdEntidadeEmpregadora(request, funcaoDominioList);
                    }
                    else
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(),
                            ErrorMessage = ErrorsDataContract.InvalidPermission.ToString()
                        });
                }
                else
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                        ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                    });
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        #endregion Contract builder

        #endregion List Responsavel Legal

        #region Update Responsavel Legal

        public ResponseBaseDataContract UpdateResponsavelLegal(ResponsavelLegalRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            Documentoidentificacao documento = new Documentoidentificacao();
            Trabalhador trabalhador = null;
            List<Documentoidentificacao> documentosList = new List<Documentoidentificacao>();
            var responsavellegal = _unitOfWork.ResponsavelLegalRepository.Get(request.ResponsavelLegal.IdResponsavelLegal);

            //validações
            if (request.DataInicioFuncao > request.DataFimFuncao)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DateFromSuperiorThanDateTo).ToString(),
                    ErrorMessage = ErrorsDataContract.DateFromSuperiorThanDateTo.ToString()
                });
            }

            if (request.ResponsavelLegal.Tin != responsavellegal.Tin)
            {
                var existingRespLegalTin = _unitOfWork.ResponsavelLegalRepository.GetByTin(request.ResponsavelLegal.Tin);
                if (existingRespLegalTin != null && existingRespLegalTin?.IdResponsavelLegal != responsavellegal.IdResponsavelLegal)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.TinAlreadyExists).ToString(),
                        ErrorMessage = ErrorsDataContract.TinAlreadyExists.ToString()
                    });
                }
            }

            if (request.Niss != null)
            {
                trabalhador = _unitOfWork.TrabalhadoresRepository.GetByNiss(request.Niss);
                //If Niss is not a match, send error back
                if (trabalhador == null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.TrabalhadorDoesNotExist).ToString(),
                        ErrorMessage = ErrorsDataContract.TrabalhadorDoesNotExist.ToString()
                    });
                }
                else if (request.ResponsavelLegal.RespLegalTabalhadorFk == null)//Check if there is a duplicate
                {
                    var duplicateRespLegal = _unitOfWork.ResponsavelLegalRepository.GetByTrabalhadorId(trabalhador.IdTrabalhador);
                    if (duplicateRespLegal != null)
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.TrabalhadorLinkedToRespLegal).ToString(),
                            ErrorMessage = ErrorsDataContract.TrabalhadorLinkedToRespLegal.ToString()
                        });
                    }
                }
            }

            if (response.Errors.Count > 0)
                return response;

            var relEntidadeRespLegal = _unitOfWork.RelEntidadeResponsavelLegalRepository.GetByResponsavelLegal(request.ResponsavelLegal.IdResponsavelLegal);

            if (responsavellegal.RespLegalTabalhadorFk != null)
            {
                //Build only: funcao na empresa, funcao rem, data de inicio e data de fim
                responsavellegal.Funcao = request.ResponsavelLegal.Funcao;
                responsavellegal.IndFuncaoRem = request.ResponsavelLegal.IndFuncaoRem;
                responsavellegal = _utils.UpdateDetailsToEntity(responsavellegal);
                relEntidadeRespLegal = MapRelEntidadeRespLegalAttributes(relEntidadeRespLegal, request);
            }
            else if (trabalhador != null)
            {
                var trabalhadorDto = Utils.MappClassToDto<Trabalhador, TrabalhadorDto>(trabalhador);
                //Fetch all documents related to trabalhador
                documentosList = _unitOfWork.DocumentoIdentificacaoRepository.GetByTrabalhadorEntidade(request.IdTrabalhador);
                //Rewrite all data according to trabalhador data
                request = FillDetailsFromTrabalhador(request, trabalhadorDto);
                responsavellegal = MapRespLegalAttributes(responsavellegal, request);
                relEntidadeRespLegal = MapRelEntidadeRespLegalAttributes(relEntidadeRespLegal, request);
            }
            else
            {
                documento = _unitOfWork.DocumentoIdentificacaoRepository.GetByResponsavelLegalEntidade(responsavellegal.IdResponsavelLegal)[0];
                documento = MapDocIdentificacaoAttributes(documento, request.DocumentoIdentificacao[0]);
                responsavellegal = MapRespLegalAttributes(responsavellegal, request);
                relEntidadeRespLegal = MapRelEntidadeRespLegalAttributes(relEntidadeRespLegal, request);
            }

            //Insert in DB
            try
            {
                _unitOfWork.RelEntidadeResponsavelLegalRepository.Update(relEntidadeRespLegal);
                _unitOfWork.ResponsavelLegalRepository.Update(responsavellegal);
                if (responsavellegal.RespLegalTabalhadorFk == null && trabalhador == null)
                {
                    _unitOfWork.DocumentoIdentificacaoRepository.Update(documento);
                }
                else
                {
                    foreach (var doc in documentosList)
                    {
                        doc.RespLegalDocumentoFkNavigation = responsavellegal;
                        _unitOfWork.DocumentoIdentificacaoRepository.Update(doc);
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        #region Mapper

        private Relentidaderesplegal MapRelEntidadeRespLegalAttributes(Relentidaderesplegal relentidaderesplegal, ResponsavelLegalRequest request)
        {
            relentidaderesplegal.DataInicioFuncao = request.DataInicioFuncao;
            relentidaderesplegal.DataFimFuncao = request.DataFimFuncao;
            relentidaderesplegal = _utils.UpdateDetailsToEntity(relentidaderesplegal);
            return relentidaderesplegal;
        }

        private Responsavellegal MapRespLegalAttributes(Responsavellegal resplegal, ResponsavelLegalRequest request)
        {
            resplegal.Nome = request.ResponsavelLegal.Nome;
            resplegal.Tin = request.ResponsavelLegal.Tin;
            resplegal.DataNasc = request.ResponsavelLegal.DataNascimento;
            resplegal.Nacionalidade = request.ResponsavelLegal.Nacionalidade;
            resplegal.Naturalidade = request.ResponsavelLegal.Naturalidade;
            resplegal.Sexo = request.ResponsavelLegal.Sexo;
            resplegal.Funcao = request.ResponsavelLegal.Funcao;
            resplegal.FuncaoOutro = request.ResponsavelLegal.FuncaoOutro;
            resplegal.IndFuncaoRem = request.ResponsavelLegal.IndFuncaoRem;
            resplegal.RespLegalTabalhadorFk = request.ResponsavelLegal.RespLegalTabalhadorFk;
            resplegal = _utils.UpdateDetailsToEntity(resplegal);
            return resplegal;
        }

        private Documentoidentificacao MapDocIdentificacaoAttributes(Documentoidentificacao doc, DocumentoIdentificacaoDataContract docRequest)
        {
            // Transform file document to bit array
            byte[] docFile = Convert.FromBase64String(docRequest.Documento);

            doc.TpDocIdentificacao = docRequest.TpDocIdentificacao;
            doc.Numero = docRequest.Numero;
            doc.LocalEmissao = docRequest.LocalEmissao;
            doc.DataEmissao = docRequest.DataEmissao;
            doc.DataValidade = docRequest.DataValidade;
            doc.Documento = docFile;
            doc.NomeDocumento = docRequest.NomeDocumento;
            doc = _utils.UpdateDetailsToEntity(doc);
            return doc;
        }

        #endregion Mapper

        #endregion Update Responsavel Legal

        #region Delete Responsavel Legal

        public ResponseBaseDataContract DeleteResponsavelLegal(ResponsavelLegalDeleteRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            List<Documentoidentificacao> documentosList = new List<Documentoidentificacao>();

            var responsavellegal = _unitOfWork.ResponsavelLegalRepository.Get(request.id);

            //Checks if responsavel legal exists
            if (responsavellegal == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString()
                });
                return response;
            }

            var relentidaderesplegal = _unitOfWork.RelEntidadeResponsavelLegalRepository.GetByResponsavelLegal(request.id);

            responsavellegal.IndActivo = false;

            if (responsavellegal.RespLegalTabalhadorFk == null)
            {
                documentosList = _unitOfWork.DocumentoIdentificacaoRepository.GetByResponsavelLegalEntidade(request.id);
            }

            //Build Hist Object

            var responsavellegalhist = BuildResponsavellegalhistObject(responsavellegal, relentidaderesplegal.RelEntidadeRespFk);

            //Insert in DB
            try
            {
                _unitOfWork.RelEntidadeResponsavelLegalRepository.Delete(relentidaderesplegal);
                _unitOfWork.ResponsavelLegalRepository.Update(responsavellegal);
                if (documentosList.Count > 0)
                {
                    foreach (var documento in documentosList)
                    {
                        documento.IndActivo = false;
                        _unitOfWork.DocumentoIdentificacaoRepository.Update(documento);
                    }
                }
                _unitOfWork.ResponsavelLegalHistRepository.Add(responsavellegalhist);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        #region Builders

        private Responsavellegalhist BuildResponsavellegalhistObject(Responsavellegal respLegal, int entidadeId)
        {
            var responsavellegalhist = new Responsavellegalhist
            {
                IdEntidadeEmpreg = entidadeId,
                Nome = respLegal.Nome,
                Tin = respLegal.Tin,
                DataNasc = respLegal.DataNasc,
                Nacionalidade = respLegal.Nacionalidade,
                Naturalidade = respLegal.Naturalidade,
                Sexo = respLegal.Sexo,
                Funcao = respLegal.Funcao,
                FuncaoOutro = respLegal.FuncaoOutro,
                IndFuncaoRem = respLegal.IndFuncaoRem,
                IndAdesFacultInss = respLegal.RespLegalTabalhadorFk == null ? true : false,
                IndActivo = true,
                FlagImportado = respLegal.FlagImportado
            };
            //Set common attributes for dtos
            responsavellegalhist = _utils.SetDetailsToEntity(responsavellegalhist);

            return responsavellegalhist;
        }

        #endregion Builders

        #endregion Delete Responsavel Legal
    }
}