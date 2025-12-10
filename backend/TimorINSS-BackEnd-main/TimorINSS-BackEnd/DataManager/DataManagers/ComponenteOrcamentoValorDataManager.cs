using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml.FormulaParsing.Utilities;
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
    public class ComponenteOrcamentoValorDataManager : IComponenteOrcamentoValorDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ComponenteOrcamentoValorDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public ResponseBaseDataContract AddOrcamentoValor(AddComponenteOrcamentoValorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            Componenteorcamentovalor componenteFilho = BuildComponenteOrcamentoValorObject(request.ComponenteOrcamentoValor);

            bool exists = _unitOfWork.ComponenteOrcamentoValorRepository.GetComponenteOrcamentoValorSameForeignKeys(componenteFilho) != null;

            if (exists)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(((int)ErrorsDataContract.ValorOrcamentoAlreadyExists).ToString());
                // ÿ é o valor de separador de datas para ser identificado no front para poder traduzir a mensagem de warning - ex: "As datas x e z (xÿz) sobrepoem a uma data de orçamento já existente"
                sb.Append("ÿ");
                sb.Append(componenteFilho.Valor.ToString("0.00"));
                response.Errors.Add(new Error
                {
                    ErrorCode = sb.ToString(),
                    ErrorMessage = ErrorsDataContract.ValorOrcamentoAlreadyExists.ToString()
                });
                return response;
            }

            List<Componenteorcamentovalor> componentesAdd = new List<Componenteorcamentovalor>();
            List<Componenteorcamentovalor> componentesUpdate = new List<Componenteorcamentovalor>();
            componentesAdd.Add(componenteFilho);
            //Agrupamentoconfig agrupamento = _unitOfWork.AgrupamentoConfigRepository.Get(componenteFilho.AgrupamentoFk);
            Componenteorcamentovalor componente;
            //ComponenteOrcamentoValorDto componenteDto;
            //while (agrupamento.ParentFk != null)
            //{
                //componente = _unitOfWork.ComponenteOrcamentoValorRepository.getOrcamentoValorByAgrupamentoAndOrcamentoRegistoInactivo(agrupamento.ParentFk.Value, componenteFilho.ComponenteOrcamentoRegistoFk);
                //if (componente == null)
                //{
                    componente = BuildComponenteOrcamentoValorFatherObject(componenteFilho);
                    //componentesAdd.Add(componente);
                //}
                //else
                //{
                //    componenteDto = Utils.MappClassToDto<Componenteorcamentovalor, ComponenteOrcamentoValorDto>(componente);
                //    if (componente.IndActivo)
                //        componenteDto.Valor += componenteFilho.Valor;
                //    else
                //    {
                //        componenteDto.Valor = componenteFilho.Valor;
                //        componenteDto.IndActivo = true;
                //    }
                //    componenteDto = _utils.UpdateDetailsToEntity(componenteDto);
                //    componente = Utils.MappClassFromDto<ComponenteOrcamentoValorDto, Componenteorcamentovalor>(componenteDto);
                //    componentesUpdate.Add(componente);
                //}
                //agrupamento = _unitOfWork.AgrupamentoConfigRepository.Get(agrupamento.ParentFk.Value);
            //}

            try
            {
                //foreach (Componenteorcamentovalor c in componentesUpdate)
                //{
                //    _unitOfWork.ComponenteOrcamentoValorRepository.Update(c);
                //}

                //foreach (Componenteorcamentovalor c in componentesAdd)
                //{
                    _unitOfWork.ComponenteOrcamentoValorRepository.Add(componenteFilho);
                //}

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }

            return response;
        }

        public SearchComponentesOrcamentoValorResponse SearchOrcamentoValor(SearchComponenteOrcamentoValorRequest request)
        {
            SearchComponentesOrcamentoValorResponse response = new SearchComponentesOrcamentoValorResponse();

            List<Componenteorcamentovalor> valores = _unitOfWork.ComponenteOrcamentoValorRepository.SearchComponenteOrcamentoValor(request.Filter);

            response.ValoresCorrentes = new List<ComponenteOrcamentoValorFullDataContract>();

            string fullCode = "";
            bool editavel = true;
            string actidadeDes = "";
            string economicDes = "";
            string funcionalDes = "";
            foreach (Componenteorcamentovalor valor in valores)
            {
                fullCode = valor.AgrupamentoFk.HasValue ? _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(valor.AgrupamentoFk.Value) : "";
                editavel = valor.AgrupamentoFkNavigation == null || valor.AgrupamentoFkNavigation.InverseParentFkNavigation.Count == 0;
                actidadeDes = valor.ActidadeFk.HasValue ? _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(valor.ActidadeFk.Value) : "";
                economicDes = valor.EconomicFk.HasValue ? _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(valor.EconomicFk.Value) : "";
                funcionalDes = valor.FuncionalFk.HasValue ? _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(valor.FuncionalFk.Value) : "";
                //editavel = true;
                response.ValoresCorrentes.Add(new ComponenteOrcamentoValorFullDataContract
                {
                    Id = valor.Id,
                    AgrupamentoFk = valor.AgrupamentoFk,
                    CentroCustoFk = valor.CentroCustoFk,
                    ComponenteOrcamentoRegistoFk = valor.ComponenteOrcamentoRegistoFk,
                    DepartamentoFk = valor.DepartamentoFk,
                    Valor = valor.Valor,
                    TipoDeConta = valor.TipoContaFk ?? 0,
                    TipoDeContaDescricao = valor.TipoContaFkNavigation?.Descricao,
                    Codigo = fullCode,
                    ActidadeDescricao = actidadeDes,
                    EconomicDescricao = economicDes,
                    FuncionalDescricao = funcionalDes,
                    Descricao = valor.AgrupamentoFkNavigation?.Designacao,
                    DepartamentoDescricao = valor.DepartamentoFkNavigation != null ? valor.DepartamentoFkNavigation.Nome : "",
                    CentroCustoDescricao = valor.CentroCustoFkNavigation != null ? valor.CentroCustoFkNavigation.Descricao : "",
                    Editavel = editavel
                });
            }
            response.ValoresCorrentes = response.ValoresCorrentes.OrderBy(v => v.TipoDeConta).ThenBy(v => v.Codigo).ToList();
            return response;
        }

        public ResponseBaseDataContract EditOrcamentoValor(AddComponenteOrcamentoValorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            Componenteorcamentovalor componenteFilho = BuildComponenteOrcamentoValorObject(request.ComponenteOrcamentoValor);
            Componenteorcamentovalor componenteFilhoOriginal = _unitOfWork.ComponenteOrcamentoValorRepository.GetComponenteOrcamentoValorSameForeignKeys(componenteFilho);

            decimal valorDiff = componenteFilho.Valor - componenteFilhoOriginal.Valor;

            ComponenteOrcamentoValorDto componenteFilhoDto = Utils.MappClassToDto<Componenteorcamentovalor, ComponenteOrcamentoValorDto>(componenteFilhoOriginal);
            componenteFilhoDto.Valor = componenteFilho.Valor;
            componenteFilhoDto = _utils.UpdateDetailsToEntity(componenteFilhoDto);
            componenteFilhoOriginal = Utils.MappClassFromDto<ComponenteOrcamentoValorDto, Componenteorcamentovalor>(componenteFilhoDto);

            List<Componenteorcamentovalor> componentesUpdate = new List<Componenteorcamentovalor>
            {
                componenteFilhoOriginal
            };

            ////Agrupamentoconfig agrupamento = _unitOfWork.AgrupamentoConfigRepository.Get(componenteFilhoOriginal.AgrupamentoFk);
            //Componenteorcamentovalor componente;
            //ComponenteOrcamentoValorDto componenteDto;
            //while (agrupamento.ParentFk != null)
            //{
            //    componente = _unitOfWork.ComponenteOrcamentoValorRepository.getOrcamentoValorByAgrupamentoAndOrcamentoRegisto(agrupamento.ParentFk.Value, componenteFilhoOriginal.ComponenteOrcamentoRegistoFk);
            //    if (componente != null)
            //    {
            //        componenteDto = Utils.MappClassToDto<Componenteorcamentovalor, ComponenteOrcamentoValorDto>(componente);
            //        componenteDto.Valor += valorDiff;
            //        componenteDto = _utils.UpdateDetailsToEntity(componenteDto);
            //        componente = Utils.MappClassFromDto<ComponenteOrcamentoValorDto, Componenteorcamentovalor>(componenteDto);
            //        componentesUpdate.Add(componente);
            //    }
            //    //agrupamento = _unitOfWork.AgrupamentoConfigRepository.Get(agrupamento.ParentFk.Value);
            //}

            try
            {
                foreach (Componenteorcamentovalor c in componentesUpdate)
                {
                    _unitOfWork.ComponenteOrcamentoValorRepository.Update(c);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }

            return response;
        }

        private Componenteorcamentovalor BuildComponenteOrcamentoValorObject(ComponenteOrcamentoValorDataContract input)
        {
            ComponenteOrcamentoValorDto entity = new ComponenteOrcamentoValorDto();

            if (input.Id > 0)
                entity = _unitOfWork.ComponenteOrcamentoValorRepository.GetDto(input.Id);

            entity.IndActivo = true;
            entity.Valor = input.Valor;
            entity.DepartamentoFk = input.DepartamentoFk;
            entity.InstitutionId = input.InstitutionId;
            entity.ComponenteOrcamentoRegistoFk = input.ComponenteOrcamentoRegistoFk;
            entity.CentroCustoFk = input.CentroCustoFk;
            entity.TipoContaFk = input.TipoContaFk;
            entity.AgrupamentoFk = input.AgrupamentoFk;
            entity.ActidadeFk = input.ActidadeFk;
            entity.EconomicFk = input.EconomicFk;
            entity.FuncionalFk = input.FuncionalFk;
            entity.IndActivo = true;

            if (entity.Id > 0)
                entity = _utils.UpdateDetailsToEntity(entity);
            else
                entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<ComponenteOrcamentoValorDto, Componenteorcamentovalor>(entity);
        }

        private Componenteorcamentovalor BuildComponenteOrcamentoValorFatherObject(Componenteorcamentovalor componente)
        {
            ComponenteOrcamentoValorDto entity = new ComponenteOrcamentoValorDto
            {
                IndActivo = true,
                Valor = componente.Valor,
                ComponenteOrcamentoRegistoFk = componente.ComponenteOrcamentoRegistoFk,
                //AgrupamentoFk = agrupamento
            };
            entity.IndActivo = true;

            entity = _utils.SetDetailsToEntity(entity);

            return Utils.MappClassFromDto<ComponenteOrcamentoValorDto, Componenteorcamentovalor>(entity);
        }

        public ResponseBaseDataContract EliminarOrcamentoValor(EliminarComponenteOrcamentoValorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            Componenteorcamentovalor componenteFilho = _unitOfWork.ComponenteOrcamentoValorRepository.Get(request.Id);

            ComponenteOrcamentoValorDto componenteFilhoDto = Utils.MappClassToDto<Componenteorcamentovalor, ComponenteOrcamentoValorDto>(componenteFilho);
            componenteFilhoDto.IndActivo = false;
            componenteFilhoDto = _utils.UpdateDetailsToEntity(componenteFilhoDto);
            componenteFilho = Utils.MappClassFromDto<ComponenteOrcamentoValorDto, Componenteorcamentovalor>(componenteFilhoDto);

            List<Componenteorcamentovalor> componentesUpdate = new List<Componenteorcamentovalor>();
            componentesUpdate.Add(componenteFilho);

            //Agrupamentoconfig agrupamento = _unitOfWork.AgrupamentoConfigRepository.Get(componenteFilho.AgrupamentoFk);
            Componenteorcamentovalor componente;
            ComponenteOrcamentoValorDto componenteDto;
            //while (agrupamento.ParentFk != null)
            //{
            //    componente = _unitOfWork.ComponenteOrcamentoValorRepository.getOrcamentoValorByAgrupamentoAndOrcamentoRegisto(agrupamento.ParentFk.Value, componenteFilho.ComponenteOrcamentoRegistoFk);
            //    if (componente != null)
            //    {
            //        componenteDto = Utils.MappClassToDto<Componenteorcamentovalor, ComponenteOrcamentoValorDto>(componente);
            //        if (componenteDto.Valor == componenteFilho.Valor)
            //        {
            //            componenteDto.Valor = 0;
            //            componenteDto.IndActivo = false;
            //        }
            //        else
            //        {
            //            componenteDto.Valor -= componenteFilho.Valor;
            //        }
            //        componenteDto = _utils.UpdateDetailsToEntity(componenteDto);
            //        componente = Utils.MappClassFromDto<ComponenteOrcamentoValorDto, Componenteorcamentovalor>(componenteDto);
            //        componentesUpdate.Add(componente);
            //    }
            //    agrupamento = _unitOfWork.AgrupamentoConfigRepository.Get(agrupamento.ParentFk.Value);
            //}

            try
            {
                foreach (Componenteorcamentovalor c in componentesUpdate)
                {
                    _unitOfWork.ComponenteOrcamentoValorRepository.Update(c);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }

            return response;
        }
    }
}