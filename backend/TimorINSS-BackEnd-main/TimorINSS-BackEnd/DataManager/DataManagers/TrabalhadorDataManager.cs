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
    public class TrabalhadorDataManager : ITrabalhadorDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public TrabalhadorDataManager(IUnitOfWork unitOfWork,
                                      IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private Trabalhador BuildTrabalhadorObject(TrabalhadorDataContract trabalhador)
        {
            TrabalhadorDto newEntity = new TrabalhadorDto
            {
                IdTrabalhador = trabalhador.IdTrabalhador,
                Nome = trabalhador.Nome,
                Niss = trabalhador.Niss,
                Tin = trabalhador.Tin,
                NumInscProvisoria = trabalhador.NumInscProvisoria,
                DataNasc = trabalhador.DataNasc,
                NomeMae = trabalhador.NomeMae,
                IndDescNomeMae = trabalhador.IndDescNomeMae,
                NomePai = trabalhador.NomePai,
                IndDescNomePai = trabalhador.IndDescNomePai,
                SexoTrabalhador = trabalhador.Sexo,
                EstadoCivil = trabalhador.EstadoCivil,
                NacionalidadeTrabalhador = trabalhador.Nacionalidade,
                Naturalidade = trabalhador.Naturalidade,
                FlagImportado = false,
                UtilizadorCriacao = 0,
                DataCriacao = DateTime.Now,
                Ipv6 = "",
                Interno = false
            };

            if (newEntity.IdTrabalhador > 0)
            {
                Trabalhador original = _unitOfWork.TrabalhadoresRepository.Get(newEntity.IdTrabalhador);
                newEntity.UtilizadorCriacao = original.UtilizadorCriacao;
                newEntity.DataCriacao = original.DataCriacao;
                newEntity.Interno = original.Interno;
                newEntity = _utils.UpdateDetailsToEntity(newEntity);
            }
            else
                newEntity = _utils.SetDetailsToEntity(newEntity);

            return Utils.MappClassFromDto<TrabalhadorDto, Trabalhador>(newEntity);
        }

        private Contacto BuildContactoObject(ContactoDataContract contacto)
        {
            ContactoDto newEntity = new ContactoDto
            {
                IdContacto = contacto.IdContacto,
                ContactoTrabalhadorFk = contacto.IdTrabalhador,
                ContactoEntidadeFk = contacto.IdEntidade,
                Telemovel = contacto.Telemovel,
                Email = contacto.Email,
                IndActivo = true,
                FlagImportado = false,
                UtilizadorCriacao = 0,
                DataCriacao = DateTime.Now,
                Ipv6 = ""
            };
            newEntity = _utils.SetDetailsToEntity<ContactoDto>(newEntity);
            return Utils.MappClassFromDto<ContactoDto, Contacto>(newEntity);
        }

        private Morada BuildMoradaObject(MoradaDataContract morada)
        {
            MoradaDto newEntity = new MoradaDto
            {
                IdMorada = morada.IdMorada,
                MoradaAldeiaFk = morada.MoradaAldeiaFk,
                Rua = morada.Rua,
                NumPorta = morada.NumPorta,
                MoradaPaisFk = morada.MoradaPaisFk,
                MoradaPrincipal = morada.MoradaPrincipal,
                EntidadeMoradaFk = morada.IdEntidadeEmpreg,
                FlagImportado = false,
                UtilizadorCriacao = 0,
                DataCriacao = DateTime.Now,
                Ipv6 = ""
            };
            newEntity = _utils.SetDetailsToEntity<MoradaDto>(newEntity);
            return Utils.MappClassFromDto<MoradaDto, Morada>(newEntity);
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
            newEntity = _utils.SetDetailsToEntity<DocumentoidentificacaoDto>(newEntity);
            return Utils.MappClassFromDto<DocumentoidentificacaoDto, Documentoidentificacao>(newEntity);
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

            if (inssEstrangeiro.Documento.Length > 0)
            {
                byte[] doc = Convert.FromBase64String(inssEstrangeiro.Documento);
                newEntity.NomeDocumento = inssEstrangeiro.NomeDocumento;
                newEntity.Documento = doc;
            }
            newEntity = _utils.SetDetailsToEntity<InssestrangeiroDto>(newEntity);
            return Utils.MappClassFromDto<InssestrangeiroDto, Inssestrangeiro>(newEntity);
        }

        private Relentidadetrabalhador BuildRelEntidadeTrabalhadorObject(RelEntidadeTrabalhadorDataContract rel)
        {
            RelentidadetrabalhadorDto newEntity = new RelentidadetrabalhadorDto
            {
                IdRel = rel.IdRelEntidadeTrabalhador,
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
                FlagImportado = false,
                UtilizadorCriacao = 0,
                DataCriacao = DateTime.Now,
                Ipv6 = "",
                RegimeFk = rel.RegimeFk,
                EscalaoFk = rel.EscalaoFk,
                ProfissaoOutro = rel.ProfissaoOutro
            };
            newEntity = _utils.SetDetailsToEntity<RelentidadetrabalhadorDto>(newEntity);
            return Utils.MappClassFromDto<RelentidadetrabalhadorDto, Relentidadetrabalhador>(newEntity);
        }

        public TrabalhadorListagemResponse GetTrabalhadoresByIdEntidadeEmpregadora(TrabalhadorListagemRequest request)
        {
            TrabalhadorListagemResponse response = new TrabalhadorListagemResponse();
            try
            {
                response = _unitOfWork.TrabalhadoresRepository.GetTrabalhadoresByIdEntidadeEmpregadora(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public VincularTrabalhadorListagemResponse getTrabalhadoresByFilter(SearchFilterRequest request)
        {
            return _unitOfWork.TrabalhadoresRepository.getTrabalhadoresByFilter(request);
        }

        public ResponseBaseDataContract SaveTrabalhador(TrabalhadorRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //build and validations
            Trabalhador trabalhador = BuildTrabalhadorObject(request.Trabalhador);

            if (string.IsNullOrWhiteSpace(trabalhador.Niss))
            {
                trabalhador.NumInscProvisoria = _unitOfWork.TrabalhadoresRepository.GetNextNumInscProvisoria();
                trabalhador.Niss = _unitOfWork.TrabalhadoresRepository.GetNextNISS();
            }
            else
            {
                if (_unitOfWork.TrabalhadoresRepository.NissExists(trabalhador.Niss))
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.NissAlreadyExists).ToString(),
                        ErrorMessage = ErrorsDataContract.NissAlreadyExists.ToString()
                    });
                }
            }

            if (_unitOfWork.TrabalhadoresRepository.TinExists(trabalhador.Tin))
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.TinAlreadyExists).ToString(),
                    ErrorMessage = ErrorsDataContract.TinAlreadyExists.ToString()
                });
            }

            Relentidadetrabalhador relacao = new Relentidadetrabalhador();
            if (request.RelEntidadeTrabalhador != null)
            {
                relacao = BuildRelEntidadeTrabalhadorObject(request.RelEntidadeTrabalhador);

                if (!_utils.ValidateRelentidadetrabalhador(relacao, _unitOfWork, out List<Error> errors))
                {
                    response.Errors = errors;
                    return response;
                }

                // A data de início de vinculo do trabalhador não pode ser inferior à data de ínicio de actividade da entidade Empregadora a que está vinculado
                var entidade = _unitOfWork.EntidadeEmpregadoraRepository.Get(relacao.EntidadeFk);
                if (entidade != null && entidade.DataInicioActiv != null && relacao.DtIniVincTrabalhador != null &&
                    entidade.DataInicioActiv > relacao.DtIniVincTrabalhador)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.InvalidDataInicioVinculo).ToString(),
                        ErrorMessage = ErrorsDataContract.InvalidDataInicioVinculo.ToString()
                    });
                    return response;
                }

                var dominio = _unitOfWork.DominioRepository.getTipoDeDominio(TiposDominio.NISSSEGURANCASOCIAL);

                if (entidade != null && entidade.Niss == dominio.descricao && relacao.DtIniVincTrabalhador <= DateTime.Now.Date && (relacao.DtIniFimTrabalhador == null || relacao.DtIniFimTrabalhador >= DateTime.Now.Date))
                {
                    trabalhador.Interno = true;
                }

                trabalhador.Relentidadetrabalhador.Add(relacao);
            }
            else
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ContratoDoesNotExists).ToString(),
                    ErrorMessage = ErrorsDataContract.ContratoDoesNotExists.ToString()
                });
            }

            Contacto contacto = new Contacto();
            if (request.Contacto != null)
            {
                contacto = BuildContactoObject(request.Contacto);
                trabalhador.Contacto.Add(contacto);
            }
            else
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ContactoDoesNotExists).ToString(),
                    ErrorMessage = ErrorsDataContract.ContactoDoesNotExists.ToString()
                });
            }

            Morada morada = new Morada();
            if (request.Morada != null)
            {
                morada = BuildMoradaObject(request.Morada);
                trabalhador.Morada.Add(morada);
            }
            else
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.MoradaDoesNotExists).ToString(),
                    ErrorMessage = ErrorsDataContract.MoradaDoesNotExists.ToString()
                });
            }

            Documentoidentificacao documento = new Documentoidentificacao();
            if (request.DocumentoIdentificacao?.Documento.Length > 0)
            {
                documento = BuildDocumentoIdentificacaoObject(request.DocumentoIdentificacao);

                //data de validade dos documentos deve ser igual ou superior  à data atual
                if (documento.DataValidade != null && documento.DataValidade < DateTime.Now.Date)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.ErroDataValidade).ToString(),
                        ErrorMessage = ErrorsDataContract.ErroDataValidade.ToString()
                    });
                    return response;
                }

                List<Documentoidentificacao> listaDocumentoIdentificacao = new List<Documentoidentificacao>();
                listaDocumentoIdentificacao = _unitOfWork.DocumentoIdentificacaoRepository.
                GetDocumentoidentificacaoByTipoENumero(documento.TpDocIdentificacao, documento.Numero);
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
                    return response;
                }
                else if (listaDocumentosTrabalhador != null && listaDocumentosTrabalhador.Count > 0)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.ErroDocumentoIdentificacao).ToString(),
                        ErrorMessage = ErrorsDataContract.ErroDocumentoIdentificacao.ToString()
                    });
                    return response;
                }

                trabalhador.Documentoidentificacao.Add(documento);
            }
            else
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DocumentoDoesNotExists).ToString(),
                    ErrorMessage = ErrorsDataContract.DocumentoDoesNotExists.ToString()
                });
            }

            Inssestrangeiro estrangeiro = new Inssestrangeiro();
            if (request.InssEstrangeiro != null)
            {
                estrangeiro = BuildINSSEstrangeiroObject(request.InssEstrangeiro);
                trabalhador.Inssestrangeiro.Add(estrangeiro);
            }

            if (response.Errors.Count > 0)
                return response;

            try
            {
                //transaction
                _unitOfWork.TrabalhadoresRepository.Add(trabalhador);

                if (request.Contacto != null)
                    _unitOfWork.ContactoRepository.Add(contacto);

                if (request.Morada != null)
                    _unitOfWork.MoradaRepository.Add(morada);

                if (request.DocumentoIdentificacao != null)
                    _unitOfWork.DocumentoIdentificacaoRepository.Add(documento);

                if (request.RelEntidadeTrabalhador != null)
                    _unitOfWork.RelEntidadeTrabalhadorRepository.Add(relacao);

                if (request.InssEstrangeiro != null)
                    _unitOfWork.INSSEstrangeiroRepository.Add(estrangeiro);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }
            return response;
        }

        public SingleTrabalhadorResponse GetById(TrabalhadorListagemRequest request)
        {
            var response = new SingleTrabalhadorResponse();

            var trabalhador = _unitOfWork.TrabalhadoresRepository.Get(request.id);

            if (trabalhador != null)
            {
                response.trabalhador = new TrabalhadorDataContract
                {
                    Nome = trabalhador.Nome,
                    Tin = trabalhador.Tin,
                    DataNasc = trabalhador.DataNasc,
                    Sexo = trabalhador.SexoTrabalhador,
                    Nacionalidade = trabalhador.NacionalidadeTrabalhador,
                    Naturalidade = trabalhador.Naturalidade,
                    Niss = trabalhador.Niss,
                    NumInscProvisoria = trabalhador.NumInscProvisoria
                };
            }

            return response;
        }

        public TrabalhadorListagemResponse GetTrabalhadoresByNiss(TrabalhadorListagemRequest request)
        {
            return _unitOfWork.TrabalhadoresRepository.GetTrabalhadoresByNiss(request);
        }

        public TrabalhadorListagemResponse GetSingleByNiss(TrabalhadorListagemNissRequest request)
        {
            var response = new TrabalhadorListagemResponse();

            var domainTrabalhador = _unitOfWork.TrabalhadoresRepository.GetByNiss(request.niss);

            if (domainTrabalhador == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.TrabalhadorDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.TrabalhadorDoesNotExist.ToString()
                });
            }
            else
            {
                var responsavelLegal = _unitOfWork.ResponsavelLegalRepository.GetByTrabalhadorId(domainTrabalhador.IdTrabalhador);

                if (responsavelLegal != null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.TrabalhadorLinkedToRespLegal).ToString(),
                        ErrorMessage = ErrorsDataContract.TrabalhadorLinkedToRespLegal.ToString()
                    });
                }
            }

            if (response.Errors.Count > 0)
                return response;

            var trabalhadorDto = Utils.MappClassToDto<Trabalhador, TrabalhadorDto>(domainTrabalhador);

            response.trabalhadores = new List<TrabalhadorListagem>
            {
                new TrabalhadorListagem
                {
                    id = trabalhadorDto.IdTrabalhador
                }
            };

            return response;
        }

        public ResponseBaseDataContract EditTrabalhador(EditTrabalhadorRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            Trabalhador trabalhador = BuildTrabalhadorObject(request.Trabalhador);

            //validações
            if (_unitOfWork.TrabalhadoresRepository.NissExists(trabalhador.Niss, trabalhador.IdTrabalhador))
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.NissAlreadyExists).ToString(),
                    ErrorMessage = ErrorsDataContract.NissAlreadyExists.ToString()
                });
                return response;
            }

            //Insert in DB
            try
            {
                _unitOfWork.TrabalhadoresRepository.Update(trabalhador);
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