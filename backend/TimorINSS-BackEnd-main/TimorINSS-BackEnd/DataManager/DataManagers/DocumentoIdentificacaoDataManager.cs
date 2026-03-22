using System;
using System.Collections.Generic;
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
    public class DocumentoIdentificacaoDataManager : IDocumentoIdentificacaoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public DocumentoIdentificacaoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private Documentoidentificacao BuildDocumentoIdentificacaoObject(DocumentoIdentificacaoDataContract documento)
        {
            byte[] doc = Convert.FromBase64String(documento.Documento);

            DocumentoidentificacaoDto newEntity = new DocumentoidentificacaoDto
            {
                IdDocIdentificacao = documento.IdDocumento,
                TrabalhadorDocumetoFk = documento.IdTrabalhador,
                RespLegalDocumentoFk = documento.IdResponsavelLegal,
                TpDocIdentificacao = documento.TpDocIdentificacao,
                Numero = documento.Numero,
                LocalEmissao = documento.LocalEmissao,
                DataEmissao = documento.DataEmissao,
                DataValidade = documento.DataValidade,
                Documento = doc,
                NomeDocumento = documento.NomeDocumento,
                IndActivo = true,
                FlagImportado = false,
                UtilizadorCriacao = 0,
                DataCriacao = DateTime.Now,
                Ipv6 = ""
            };
            if (newEntity.IdDocIdentificacao > 0)
            {
                Documentoidentificacao original = _unitOfWork.DocumentoIdentificacaoRepository.Get(newEntity.IdDocIdentificacao);
                newEntity.UtilizadorCriacao = original.UtilizadorCriacao;
                newEntity.DataCriacao = original.DataCriacao;
                newEntity = _utils.UpdateDetailsToEntity(newEntity);
            }
            else
                newEntity = _utils.SetDetailsToEntity(newEntity);
            return Utils.MappClassFromDto<DocumentoidentificacaoDto, Documentoidentificacao>(newEntity);
        }

        public DocumentosListagemResponse GetDocumentosIdentificacaoByIdTrabalhador(DocumentosListagemRequest request)
        {
            DocumentosListagemResponse response = new DocumentosListagemResponse();
            try
            {
                if (request.filter != null)
                {
                    bool valid = false;
                    var userId = request.UserId;
                    var user = _unitOfWork.UtilizadoresRepository.Get(userId);
                    //var trabalhadorId = request.Id;

                    var b64 = request.IdStr.ToString().Replace('-', '+').Replace('_', '/');
                    switch (b64.Length % 4) { case 2: b64 += "=="; break; case 3: b64 += "="; break; }
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(b64)).Trim();

                    int trabalhadorId = IdDecoder.DecodeId(request.IdStr);

                    var entidadeEmpregadoraId = user.UtilizadorEntidadeFk;

                    // Validar se o utilizador tem as permissões necessárias
                    if (user.Interno == true)
                        valid = true;
                    else if (entidadeEmpregadoraId.HasValue)
                        valid = _unitOfWork.RelEntidadeTrabalhadorRepository.IsTrabalhadorAssociadoEntidadeEmpregadora(trabalhadorId, entidadeEmpregadoraId.Value);

                    if (valid)
                    {
                        request.filter.filterField = "TRABALHADOR";
                        response = _unitOfWork.DocumentoIdentificacaoRepository.GetListagemByFilter(request);
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

        public DocumentoResponse GetDocumentosIdentificacaoById(DocumentoIdRequest request)
        {
            var response = new DocumentoResponse { RequestId = request.RequestId };

            var b64 = request.idStr.ToString().Replace('-', '+').Replace('_', '/');
            switch (b64.Length % 4) { case 2: b64 += "=="; break; case 3: b64 += "="; break; }
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(b64)).Trim();

            int decodedId = int.Parse(decoded);

            Documentoidentificacao documento = _unitOfWork.DocumentoIdentificacaoRepository.Get(decodedId);
            string doc = Convert.ToBase64String(documento.Documento);
            response.documento = new DocumentoIdentificacaoDataContract
            {
                IdDocumento = documento.IdDocIdentificacao,
                IdTrabalhador = documento.TrabalhadorDocumetoFk,
                IdResponsavelLegal = documento.RespLegalDocumentoFk,
                TpDocIdentificacao = documento.TpDocIdentificacao,
                Numero = documento.Numero,
                LocalEmissao = documento.LocalEmissao,
                DataEmissao = documento.DataEmissao,
                DataValidade = documento.DataValidade,
                Documento = doc,
                NomeDocumento = documento.NomeDocumento,
            };
            return response;
        }

        public ResponseBaseDataContract SaveDocumento(DocumentoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //validações
            if (request.documento?.Documento.Length <= 0)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DocumentoDoesNotExists).ToString(),
                    ErrorMessage = ErrorsDataContract.DocumentoDoesNotExists.ToString()
                });
            }

            //Build Objects
            Documentoidentificacao documento = BuildDocumentoIdentificacaoObject(request.documento);

            var listaDocumentoIdentificacao = _unitOfWork.DocumentoIdentificacaoRepository.GetDocumentoidentificacaoByTipoENumero(documento.TpDocIdentificacao, documento.Numero);
            bool insertDocumento = true;

            //data de validade dos documentos deve ser igual ou superior  à data atual
            if (documento.DataValidade != null && documento.DataValidade < DateTime.Now.Date)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroDataValidade).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroDataValidade.ToString()
                });
                insertDocumento = false;
            }

            List<Documentoidentificacao> listaDocumentosTrabalhador = new List<Documentoidentificacao>();
            if (documento.TrabalhadorDocumetoFk != null)
            {
                listaDocumentosTrabalhador = _unitOfWork.DocumentoIdentificacaoRepository.GetDocumentoidentificacaoByTrabalhadorFkETipo((int)documento.TrabalhadorDocumetoFk, documento.TpDocIdentificacao);
            }
            // Não podem existir documentos duplicados no sistema
            if (listaDocumentoIdentificacao != null && listaDocumentoIdentificacao.Count > 0)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroDocumentoIdentificacao).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroDocumentoIdentificacao.ToString()
                });
                insertDocumento = false;
            }
            else if (listaDocumentosTrabalhador != null && listaDocumentosTrabalhador.Count > 0)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroDocumentoIdentificacao).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroDocumentoIdentificacao.ToString()
                });
                insertDocumento = false;
            }

            try
            {
                //Insert in DB
                if (insertDocumento)
                {
                    _unitOfWork.DocumentoIdentificacaoRepository.Add(documento);
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

        public ResponseBaseDataContract UpdateDocumento(DocumentoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //Build Objects
            Documentoidentificacao documento = BuildDocumentoIdentificacaoObject(request.documento);

            List<Documentoidentificacao> listaDocumentoIdentificacao = new List<Documentoidentificacao>();
            listaDocumentoIdentificacao = _unitOfWork.DocumentoIdentificacaoRepository.
            GetDocumentoidentificacaoByTipoENumero(documento.TpDocIdentificacao, documento.Numero);
            bool updateDocumento = true;

            //data de validade dos documentos deve ser igual ou superior  à data atual
            if (documento.DataValidade != null && documento.DataValidade < DateTime.Now.Date)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroDataValidade).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroDataValidade.ToString()
                });
                updateDocumento = false;
            }

            // Não podem existir documentos duplicados no sistema
            List<Documentoidentificacao> listaDocumentosTrabalhador = new List<Documentoidentificacao>();
            if (documento.TrabalhadorDocumetoFk != null)
            {
                listaDocumentosTrabalhador = _unitOfWork.DocumentoIdentificacaoRepository.GetDocumentoidentificacaoByTrabalhadorFkETipo((int)documento.TrabalhadorDocumetoFk, documento.TpDocIdentificacao);
            }
            // Não podem existir documentos duplicados no sistema
            if (listaDocumentoIdentificacao != null && listaDocumentoIdentificacao.Count > 0)
            {
                foreach (var doc in listaDocumentoIdentificacao)
                {
                    if (doc.IdDocIdentificacao != documento.IdDocIdentificacao)
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.ErroDocumentoIdentificacao).ToString(),
                            ErrorMessage = ErrorsDataContract.ErroDocumentoIdentificacao.ToString()
                        });
                        updateDocumento = false;
                    }
                }
            }
            else if (listaDocumentosTrabalhador != null && listaDocumentosTrabalhador.Count > 0)
            {
                foreach (var doc in listaDocumentosTrabalhador)
                {
                    if (doc.IdDocIdentificacao != documento.IdDocIdentificacao)
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.ErroDocumentoIdentificacao).ToString(),
                            ErrorMessage = ErrorsDataContract.ErroDocumentoIdentificacao.ToString()
                        });
                        updateDocumento = false;
                    }
                }
            }
            try
            {
                //Insert in DB
                if (updateDocumento)
                {
                    _unitOfWork.DocumentoIdentificacaoRepository.Update(documento);
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

        public ResponseBaseDataContract DeleteDocumento(DocumentoIdRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //validações
            Documentoidentificacao documentoModel = _unitOfWork.DocumentoIdentificacaoRepository.Get(request.id);
            DocumentoidentificacaoDto documento = Utils.MappClassToDto<Documentoidentificacao, DocumentoidentificacaoDto>(documentoModel);

            if (documento == null)
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });

            if (response.Errors.Count > 0)
            {
                return response;
            }

            documento.IndActivo = false;
            documento = _utils.UpdateDetailsToEntity(documento);
            Documentoidentificacao documentoModelFinal = Utils.MappClassFromDto<DocumentoidentificacaoDto, Documentoidentificacao>(documento);

            //Update in DB
            try
            {
                _unitOfWork.DocumentoIdentificacaoRepository.Update(documentoModelFinal);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        public DocumentosTarefaListagemResponse getDocumentosByIdTarefaAtivo(DocumentosListagemRequest request)
        {
            DocumentosTarefaListagemResponse response = new DocumentosTarefaListagemResponse();
            try
            {
                if (request.filter != null)
                {
                    int decodedId = IdDecoder.DecodeId(request.IdStr);

                    request.Id = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(decodedId).ProcessoAtivoFk;
                    response = _unitOfWork.ComponenteDocumentosRegistoRepository.GetListagemByIdProcesso(request);
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

        public DocumentosTarefaListagemResponse getDocumentosByIdProcessoAtivo(DocumentosListagemRequest request)
        {
            DocumentosTarefaListagemResponse response = new DocumentosTarefaListagemResponse();
            try
            {
                if (request.filter != null)
                {
                    request.Id = IdDecoder.DecodeId(request.IdStr);
                    response = _unitOfWork.ComponenteDocumentosRegistoRepository.GetListagemByIdProcesso(request);
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

        public ResponseBaseDataContract SaveTarefaDocumento(TarefaDocumentoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //validações
            if (request.documento.Length <= 0)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DocumentoDoesNotExists).ToString(),
                    ErrorMessage = ErrorsDataContract.DocumentoDoesNotExists.ToString()
                });
            }

            var tarefaAtivo = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.tarefaAtivoId);

            if (tarefaAtivo == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString()
                });
            }

            if (response.Errors.Count > 0)
                return response;

            var previousDocTypePerTarefaCount = _unitOfWork.ComponenteDocumentosRegistoRepository.GetByTarefaIdAndType(request.tarefaAtivoId, request.tipoDocumento).Count;

            byte[] doc = Convert.FromBase64String(request.documento);

            //Build Object
            ComponentedocumentoRegisto documento = new ComponentedocumentoRegisto
            {
                DocumentoFk = request.tipoDocumento,
                TarefaAtivoFk = request.tarefaAtivoId,
                Documento = doc,
                Numero = previousDocTypePerTarefaCount > 0 ? previousDocTypePerTarefaCount.ToString() : "",
                IndActivo = true
            };

            documento = _utils.SetDetailsToEntity(documento);
            try
            {
                //Insert in DB
                _unitOfWork.ComponenteDocumentosRegistoRepository.Add(documento);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        public ResponseBaseDataContract DeleteDocumentoComponente(DocumentoIdRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //validações
            ComponentedocumentoRegisto documentoModel = _unitOfWork.ComponenteDocumentosRegistoRepository.Get(request.id);
            ComponentedocumentoRegistoDto documento = Utils.MappClassToDto<ComponentedocumentoRegisto, ComponentedocumentoRegistoDto>(documentoModel);

            if (documento == null)
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });

            if (response.Errors.Count > 0)
            {
                return response;
            }

            documento.IndActivo = false;
            documento = _utils.UpdateDetailsToEntity(documento);
            ComponentedocumentoRegisto documentoModelFinal = Utils.MappClassFromDto<ComponentedocumentoRegistoDto, ComponentedocumentoRegisto>(documento);

            //Update in DB
            try
            {
                _unitOfWork.ComponenteDocumentosRegistoRepository.Update(documentoModelFinal);
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