using System;
using System.Collections.Generic;
using System.Text;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ContactoDataManager : IContactoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ContactoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public ContactoDto GetDto(int id)
        {
            return _unitOfWork.ContactoRepository.GetDto(id);
        }

        public ContatoListagemResponse GetContatosByIdEntidadeEmpregadora(ContatoListagemRequest request)
        {
            ContatoListagemResponse response = new ContatoListagemResponse();
            try
            {
                if (request.filter != null)
                {
                    bool valid = false;
                    var userId = request.UserId;
                    var user = _unitOfWork.UtilizadoresRepository.Get(userId);
                    var entidadeEmpregadoraId = user.UtilizadorEntidadeFk;

                    // Validar se o utilizador tem as permissões necessárias
                    if (entidadeEmpregadoraId.HasValue || user.Interno == true)
                        valid = true;

                    if (valid)
                    {
                        request.filter.filterField = "ENTIDADEEMPREGADORA";
                        response = _unitOfWork.ContactoRepository.GetContactosByFilter(request);
                    }
                    else
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(),
                            ErrorMessage = ErrorsDataContract.InvalidPermission.ToString()
                        });
                    }
                }
                else
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                        ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                    });
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ContatoListagemResponse GetContatosByIdTrabalhador(ContatoListagemRequest request)
        {
            ContatoListagemResponse response = new ContatoListagemResponse();
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

                    int trabalhadorId = int.Parse(decoded);
                    const int SECRET_A = 12;
                    const int SECRET_B = 123456789;
                    trabalhadorId = (trabalhadorId - SECRET_B) / SECRET_A;


                    var entidadeEmpregadoraId = user.UtilizadorEntidadeFk;

                    // Validar se o utilizador tem as permissões necessárias
                    if (user.Interno == true)
                        valid = true;
                    else if (entidadeEmpregadoraId.HasValue)
                        valid = _unitOfWork.RelEntidadeTrabalhadorRepository.IsTrabalhadorAssociadoEntidadeEmpregadora(trabalhadorId, entidadeEmpregadoraId.Value);

                    if (valid)
                    {
                        request.filter.filterField = "TRABALHADOR";
                        response = _unitOfWork.ContactoRepository.GetContactosByFilter(request);
                    }
                    else
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(),
                            ErrorMessage = ErrorsDataContract.InvalidPermission.ToString()
                        });
                    }
                }
                else
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                        ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                    });
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ContatoListagemResponse SaveContato(ContatoRequest request)
        {
            var response = new ContatoListagemResponse { RequestId = request.RequestId };

            //validações

            //Build Objects
            Contacto contato = BuildContatoObject(request);

            //evitar registo de contactos duplicados
            bool insertContacto = true;
            var listaContacto = _unitOfWork.ContactoRepository.GetByTrabalhadorFkEntidadeFk(contato.ContactoTrabalhadorFk, contato.ContactoEntidadeFk);

            if (listaContacto != null && listaContacto.Count > 0)
            {
                foreach (var contacto in listaContacto)
                {
                    if (contacto.Email != null && request.Contato.Email != null &&
                        contacto.Email == request.Contato.Email)
                    {
                        if (contacto.ContactoEntidadeFk != null && contacto.ContactoEntidadeFk > 0)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.ExistEmailEntidade).ToString(),
                                ErrorMessage = ErrorsDataContract.ExistEmailEntidade.ToString()
                            });
                            insertContacto = false;
                            break;
                        }
                        else if (contacto.ContactoTrabalhadorFk != null && contacto.ContactoTrabalhadorFk > 0)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.ExistEmailTrabalhador).ToString(),
                                ErrorMessage = ErrorsDataContract.ExistEmailTrabalhador.ToString()
                            });
                            insertContacto = false;
                            break;
                        }
                    }

                    if (contacto.Telemovel != null && request.Contato.Telemovel != null &&
                        contacto.Telemovel == request.Contato.Telemovel)
                    {
                        if (contacto.ContactoEntidadeFk != null && contacto.ContactoEntidadeFk > 0)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.ExistTelemovelEntidade).ToString(),
                                ErrorMessage = ErrorsDataContract.ExistTelemovelEntidade.ToString()
                            });
                            insertContacto = false;
                            break;
                        }

                        if (contacto.ContactoTrabalhadorFk != null && contacto.ContactoTrabalhadorFk > 0)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.ExistTelemovelTrabalhador).ToString(),
                                ErrorMessage = ErrorsDataContract.ExistTelemovelTrabalhador.ToString()
                            });
                            insertContacto = false;
                            break;
                        }
                    }
                }
            }

            //Insert in DB
            try
            {
                if (insertContacto)
                {
                    _unitOfWork.ContactoRepository.Add(contato);
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

        public ContatoListagemResponse UpdateContato(ContatoRequest request)
        {
            var response = new ContatoListagemResponse { RequestId = request.RequestId };

            //Build Objects
            Contacto contato = BuildContatoObject(request);

            //evitar registo de contactos duplicados
            bool updateContacto = true;
            var listaContacto = _unitOfWork.ContactoRepository.GetByTrabalhadorFkEntidadeFk(contato.ContactoTrabalhadorFk, contato.ContactoEntidadeFk);
            if (listaContacto != null && listaContacto.Count > 0)
            {
                foreach (var contacto in listaContacto)
                {
                    if (contacto.IdContacto != request.Contato.IdContacto && contacto.Email != null && request.Contato.Email != null &&
                        contacto.Email == request.Contato.Email)
                    {
                        if (contacto.ContactoEntidadeFk != null && contacto.ContactoEntidadeFk > 0)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.ExistEmailEntidade).ToString(),
                                ErrorMessage = ErrorsDataContract.ExistEmailEntidade.ToString()
                            });
                            updateContacto = false;
                            break;
                        }
                        else if (contacto.ContactoTrabalhadorFk != null && contacto.ContactoTrabalhadorFk > 0)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.ExistEmailTrabalhador).ToString(),
                                ErrorMessage = ErrorsDataContract.ExistEmailTrabalhador.ToString()
                            });
                            updateContacto = false;
                            break;
                        }
                    }

                    if (contacto.IdContacto != request.Contato.IdContacto && contacto.Telemovel != null && request.Contato.Telemovel != null &&
                        contacto.Telemovel == request.Contato.Telemovel)
                    {
                        if (contacto.ContactoEntidadeFk != null && contacto.ContactoEntidadeFk > 0)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.ExistTelemovelEntidade).ToString(),
                                ErrorMessage = ErrorsDataContract.ExistTelemovelEntidade.ToString()
                            });
                            updateContacto = false;
                            break;
                        }

                        if (contacto.ContactoTrabalhadorFk != null && contacto.ContactoTrabalhadorFk > 0)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.ExistTelemovelTrabalhador).ToString(),
                                ErrorMessage = ErrorsDataContract.ExistTelemovelTrabalhador.ToString()
                            });
                            updateContacto = false;
                            break;
                        }
                    }
                }
            }

            //Insert in DB
            try
            {
                if (updateContacto)
                {
                    _unitOfWork.ContactoRepository.Update(contato);
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

        public ResponseBaseDataContract DeleteContato(ContatoDeleteRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            Contacto domainContato = _unitOfWork.ContactoRepository.Get(request.Id);

            if (domainContato == null)
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });

            if (response.Errors.Count > 0)
            {
                return response;
            }

            domainContato.IndActivo = false;

            //Update in DB
            try
            {
                _unitOfWork.ContactoRepository.Update(domainContato);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        private Contacto BuildContatoObject(ContatoRequest request)
        {
            ContactoDto contato = new ContactoDto
            {
                IdContacto = request.Contato.IdContacto,
                ContactoEntidadeFk = request.Contato.IdEntidade,
                ContactoTrabalhadorFk = request.Contato.IdTrabalhador,
                Telemovel = request.Contato.Telemovel,
                Email = request.Contato.Email,
                IndActivo = true,
                FlagImportado = false,
                DataCriacao = DateTime.Now
            };
            contato = _utils.SetDetailsToEntity(contato);
            if (contato.IdContacto != 0)
            {
                contato.DataAlteracao = DateTime.Now;
                contato.UtilizadorAlteracao = Convert.ToInt32(request.UserId);
            }
            return Utils.MappClassFromDto<ContactoDto, Contacto>(contato);
        }
    }
}