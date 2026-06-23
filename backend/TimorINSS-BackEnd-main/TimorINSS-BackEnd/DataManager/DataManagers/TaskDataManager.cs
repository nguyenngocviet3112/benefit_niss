using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class TaskDataManager : ITaskDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;
        private IDeclaracaoremuneracaoDataManager _declaracaoManager;
        private IDominioDataManager _dominioManager;

        public TaskDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils,
                                IDeclaracaoremuneracaoDataManager declaracaoManager,
                                IDominioDataManager dominioManager)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _declaracaoManager = declaracaoManager;
            _dominioManager = dominioManager;
        }

        #region Conta Corrente

        //public List<Contacorrente> GetContaCorrenteByData(DateTime dataVencimento)
        //{
        //    List<int> situacaoGuia = new List<int>();
        //    List<Contacorrente> listaContaCorrenteToUpdate = new List<Contacorrente>();
        //    Guiapagamento guiaPagamentoMaisRecente = new Guiapagamento();
        //    Guiapagamento guiaPagamentoPai = new Guiapagamento();

        //    List<Contacorrente> listaContasExpiradas = _unitOfWork.ContaCorrenteRepository.GetContaCorrenteByData(dataVencimento);

        //    if (listaContasExpiradas != null && listaContasExpiradas.Count > 0)
        //    {
        //        foreach (var contaCorrente in listaContasExpiradas)
        //        {
        //            if (contaCorrente.SituacaoPagamento == _unitOfWork.DominioRepository.getIdDominio("SITUACAOPAGAMENTO", 4))
        //            {
        //                listaContaCorrenteToUpdate.Add(contaCorrente);
        //            }

        //            if (contaCorrente.SituacaoPagamento == _unitOfWork.DominioRepository.getIdDominio("SITUACAOPAGAMENTO", 2) ||
        //                contaCorrente.SituacaoPagamento == _unitOfWork.DominioRepository.getIdDominio("SITUACAOPAGAMENTO", 5) ||
        //                contaCorrente.SituacaoPagamento == _unitOfWork.DominioRepository.getIdDominio("SITUACAOPAGAMENTO", 6))
        //            {
        //                // obter a guia filha mais recente, caso esta estiver expirada, a conta também deverá ser inativada
        //                guiaPagamentoMaisRecente = _unitOfWork.GuiaPagamentoRepository.GetUltimoGuiaFilho(contaCorrente.GuiaPagamentoFk);

        //                if (guiaPagamentoMaisRecente != null && !guiaPagamentoMaisRecente.IndActivo)
        //                {
        //                    listaContaCorrenteToUpdate.Add(contaCorrente);
        //                }
        //                else if (guiaPagamentoMaisRecente == null && contaCorrente.GuiaPagamentoFk != null)
        //                {
        //                    //valida se o guia pai está inativo
        //                    guiaPagamentoPai = _unitOfWork.GuiaPagamentoRepository.Get((long)contaCorrente.GuiaPagamentoFk);
        //                    if (guiaPagamentoPai != null && !guiaPagamentoPai.IndActivo)
        //                    {
        //                        listaContaCorrenteToUpdate.Add(contaCorrente);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return listaContaCorrenteToUpdate;
        //}

        //public ResponseBaseDataContract UpdateContaCorrente(DateTime dataVencimento)
        //{
        //    var response = new ResponseBaseDataContract();
        //    //obter todas as contas correntes com data vencimento inferior à data do parâmetro e sem guia gerada
        //    List<Contacorrente> listaContaCorrente = GetContaCorrenteByData(dataVencimento);

        //    // caso existem contas com data de vencimento inferior à data, inativar essas contas e criar nova conta
        //    try
        //    {
        //        if (listaContaCorrente != null && listaContaCorrente.Count() > 0)
        //        {
        //            foreach (var contaCorrente in listaContaCorrente)
        //            {
        //                // inativação da conta
        //                contaCorrente.IndActivo = false;
        //                contaCorrente.UtilizadorAlteracao = _unitOfWork.UtilizadoresRepository.GetAdminUser().IdUtilizador;
        //                contaCorrente.DataAlteracao = DateTime.Now;
        //                _unitOfWork.ContaCorrenteRepository.Update(contaCorrente);

        //                //adicionar nova linha tipoDivida= "Contribuições e Juros"

        //                _unitOfWork.ContaCorrenteRepository.Add(BuildContaCorrenteToUpdate(contaCorrente));
        //            }
        //        }
        //        _unitOfWork.Commit();
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        _unitOfWork.Rollback();
        //        response.Errors.Add(new Error { ErrorMessage = e.Message });
        //        return response;
        //    }
        //}

        //private Contacorrente BuildContaCorrenteToUpdate(Contacorrente contaCorrente)
        //{
        //    decimal taxaJuroMensal = 0;
        //    int taxaJuroFk = 0;

        //    Taxajuromensal taxaJuro = _unitOfWork.TaxaJuroMensalRepository.GetTaxaJuroMensalByData(contaCorrente.MesAno);

        //    if (taxaJuro != null)
        //    {
        //        taxaJuroFk = taxaJuro.IdTaxa;
        //        taxaJuroMensal = taxaJuro.Percentagem;

        //        decimal valorTotalSemJuros = contaCorrente.ValorEntidade + contaCorrente.ValorTrabalhador;
        //        decimal juros = valorTotalSemJuros * taxaJuroMensal / 100;
        //        decimal juroApurado = 0;
        //        if (contaCorrente.Juroapurado != null)
        //        {
        //            juroApurado = (decimal)contaCorrente.Juroapurado;
        //        }

        //        ContacorrenteDto contaCorrenteDto = new ContacorrenteDto
        //        {
        //            ContaCorrenteEntidadeFk = contaCorrente.ContaCorrenteEntidadeFk,
        //            ContaCorrenteTrabalhadorFk = contaCorrente.ContaCorrenteTrabalhadorFk,
        //            TipoDivida = _unitOfWork.DominioRepository.getIdDominio("TIPODIVIDA", 2),
        //            MesAno = contaCorrente.MesAno,
        //            DataVencimento = contaCorrente.DataVencimento.AddMonths(1),
        //            ValorEntidade = contaCorrente.ValorEntidade,
        //            ValorTrabalhador = contaCorrente.ValorTrabalhador,
        //            ValorTotal = valorTotalSemJuros + juros + juroApurado,
        //            DataCriacao = DateTime.Now,
        //            PagoEm = null,
        //            SituacaoPagamento = _unitOfWork.DominioRepository.getIdDominio("SITUACAOPAGAMENTO", 4),
        //            ContaCorrenteTaxaJuroFk = taxaJuroFk,
        //            Juroapurado = juros + juroApurado,
        //            GuiaPagamentoFk = contaCorrente.GuiaPagamentoFk,
        //            IndActivo = true,
        //            UtilizadorCriacao = _unitOfWork.UtilizadoresRepository.GetAdminUser().IdUtilizador,
        //            Ipv6 = contaCorrente.Ipv6
        //        };
        //        return Utils.MappClassFromDto<ContacorrenteDto, Contacorrente>(contaCorrenteDto);
        //    }
        //    else
        //    {
        //        return contaCorrente;
        //    }
        //}

        #endregion Conta Corrente

        #region Guia de Pagamento

        //public ResponseBaseDataContract UpdateGuiaPagamento()
        //{
        //    var response = new ResponseBaseDataContract();

        //    // get all guias de pagamento que passaram a data atual
        //    var geradaStateId = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.INDPAGO).Find(x => x.value == 2).id;
        //    var guiasAtrasadas = _unitOfWork.GuiaPagamentoRepository.GetAllGuiasAtrasadas(geradaStateId);
        //    var idAdmin = _unitOfWork.UtilizadoresRepository.GetAdminUser().IdUtilizador;

        //    if (guiasAtrasadas.Count > 0)
        //    {
        //        foreach (var guia in guiasAtrasadas)
        //        {
        //            guia.IndActivo = false;
        //            guia.UtilizadorAlteracao = idAdmin;
        //            guia.DataAlteracao = DateTime.Now;

        //            try
        //            {
        //                _unitOfWork.GuiaPagamentoRepository.Update(guia);
        //                _unitOfWork.Commit();
        //            }
        //            catch (Exception e)
        //            {
        //                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
        //                _unitOfWork.Rollback();
        //            }
        //        }
        //    }

        //    return response;
        //}

        #endregion Guia de Pagamento

        #region Declaracao Remuneracao

        public ResponseBaseDataContract UpdateDeclaracao()
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract
            {
                Errors = new List<Error>()
            };
            //check day
            SingleDominioDescricaoStringResponse diaDeclaracao = _dominioManager.GetDeclarationDay("TE");
            DateTime now = DateTime.Now;
            DateTime checkDate = new DateTime(now.Year, now.Month, 1);
            if (now.Day < diaDeclaracao.dominio.value)
                checkDate = checkDate.AddMonths(-1);

            //get late entityies
            List<Tuple<int, DateTime>> empresas = _unitOfWork.DeclaracaoRemuneracaoRepository.GetEntidadesAndFirstDateComDeclaracaoNaoRegistadasPastDay(checkDate);

            ResponseBaseDataContract saveResponse;

            foreach (Tuple<int, DateTime> empresa in empresas)
            {
                saveResponse = _declaracaoManager.AutoGenerateNextDeclarations(empresa.Item1, empresa.Item2);
                if (saveResponse.Errors.Count > 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = string.Format("The declarations automatic job has encountered erros for the Entity: {0}", empresa.Item1) });
                    response.Errors.AddRange(saveResponse.Errors);
                }
            }

            return response;
        }

        #endregion Declaracao Remuneracao

        #region Trabalhador

        public ResponseBaseDataContract UpdateTrabalhadorInterno()
        {
            var response = new ResponseBaseDataContract();

            List<Relentidadetrabalhador> rel = new List<Relentidadetrabalhador>();
            DominioDescricaoString dominio = new DominioDescricaoString();
            List<Trabalhador> listaTrabalhador = new List<Trabalhador>();

            
            // vai buscar o niss da segurança social
            dominio = _unitOfWork.DominioRepository.getTipoDeDominio(TiposDominio.NISSSEGURANCASOCIAL);
            if (dominio != null && dominio.descricao != null)
            {
                //obter todos os trabalhadores com niss = niss da INSS
                rel = _unitOfWork.RelEntidadeTrabalhadorRepository.GetRelEntidadeTrabalhadorINSS(dominio.descricao);

                if (rel != null && rel.Count > 0)
                {
                    foreach (var relEntidadeTrab in rel)
                    {
                        var user = _unitOfWork.UtilizadoresRepository.GetByTrabalhadorFk(relEntidadeTrab.TrabalhadorFk);
                        if (relEntidadeTrab.DtIniVincTrabalhador <= DateTime.Now.Date && (relEntidadeTrab.DtIniFimTrabalhador == null
                            || relEntidadeTrab.DtIniFimTrabalhador > DateTime.Now.Date))
                        {
                            //atualizar trabalhador para interno
                            relEntidadeTrab.TrabalhadorFkNavigation.Interno = true;
                            if (user != null) user.Interno = true;
                            relEntidadeTrab.TrabalhadorFkNavigation = _utils.UpdateDetailsToEntity(relEntidadeTrab.TrabalhadorFkNavigation);
                            listaTrabalhador.Add(relEntidadeTrab.TrabalhadorFkNavigation);
                        }
                        if (relEntidadeTrab.DtIniFimTrabalhador != null && relEntidadeTrab.DtIniFimTrabalhador < DateTime.Now.Date
                            && relEntidadeTrab.TrabalhadorFkNavigation.Interno)
                        {
                            relEntidadeTrab.TrabalhadorFkNavigation.Interno = false;
                            if (user != null) user.Interno = false;
                            relEntidadeTrab.TrabalhadorFkNavigation = _utils.UpdateDetailsToEntity(relEntidadeTrab.TrabalhadorFkNavigation);
                            listaTrabalhador.Add(relEntidadeTrab.TrabalhadorFkNavigation);
                        }
                    }
                }
            }

            try
            {
                if (listaTrabalhador != null && listaTrabalhador.Count > 0)
                {
                    foreach (var trabalhador in listaTrabalhador)
                    {
                        _unitOfWork.TrabalhadoresRepository.Update(trabalhador);
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        #endregion Trabalhador

        #region Importer

        public void DeleteOldImports()
        {
            _unitOfWork.ImporterRepository.DeleteOld();
            _unitOfWork.Commit();
        }

        #endregion Importer
    }
}