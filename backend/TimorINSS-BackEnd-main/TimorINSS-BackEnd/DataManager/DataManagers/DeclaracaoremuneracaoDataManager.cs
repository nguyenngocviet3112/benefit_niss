using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.ExcelDocumentService;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using TimorINSSBackEnd.Resources;
using static TimorINSSBackEnd.ExcelDocumentService.Models;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class DeclaracaoremuneracaoDataManager : IDeclaracaoremuneracaoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;
        private IDominioDataManager _dominioManager;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DeclaracaoremuneracaoDataManager(IUnitOfWork unitOfWork,
                                                IUtilsDataManager utils,
                                                IDominioDataManager dominioManager,
                                                IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _dominioManager = dominioManager;
            _localizer = localizer;
        }

        private DeclaracaoListagem BuildDeclaracaoDataContract(DeclaracaoremuneracaoDto declaracao, DateTime mesAno)
        {
            DeclaracaoRemuneracaoDataContract declaracaoModel = new DeclaracaoRemuneracaoDataContract
            {
                idDeclaracao = declaracao.IdDeclaracao,
                declaracaoRelEntidadeTrabalhadorFk = declaracao.DeclaracaoRelEntidadeTrabalhadorFk,
                diasContrato = declaracao.DiasContrato,
                diasEfecTrabalhados = declaracao.DiasEfecTrabalhados,
                faltasInjustific = declaracao.FaltasInjustific,
                diasParentalidade = declaracao.DiasParentalidade,
                diasTrabcontabSegSocial = declaracao.DiasTrabcontabSegSocial,
                remunDeclarada = declaracao.RemunDeclarada,
                decimoTerceiro = declaracao.DecimoTerceiro,
                mesAno = mesAno,
                contaCorrenteFk = declaracao.ContaCorrenteFk,
                decimoTerceiroMes = false,
                oficioso = declaracao.Oficioso,
                flagImportado = declaracao.FlagImportado,
                nacionalidadeFk = declaracao.NacionalidadeFk,
                regimeFk = declaracao.RegimeFk,
                sexoFk = declaracao.SexoFk
            };

            return _unitOfWork.DeclaracaoRemuneracaoRepository
                .GetDeclaracaoTrabalhadorInfo(declaracaoModel);
        }

        private DeclaracaoListagem BuildEmptyDeclaracaoDataContract(int idRel, DateTime mesAno)
        {
            DeclaracaoRemuneracaoDataContract declaracaoModel = new DeclaracaoRemuneracaoDataContract
            {
                idDeclaracao = 0,
                declaracaoRelEntidadeTrabalhadorFk = idRel,
                diasContrato = 0,
                diasEfecTrabalhados = 0,
                faltasInjustific = 0,
                diasParentalidade = 0,
                diasTrabcontabSegSocial = 0,
                remunDeclarada = 0,
                decimoTerceiro = 0,
                mesAno = mesAno,
                contaCorrenteFk = 0,
                decimoTerceiroMes = false,
                oficioso = false,
                flagImportado = false
            };

            return _unitOfWork.DeclaracaoRemuneracaoRepository
                .GetDeclaracaoTrabalhadorInfo(declaracaoModel);
        }

        public GetDeclaracaoByEntidadeAndFilterResponse GetDeclaracaoByEntidadeAndFilter(GetDeclaracaoByEntidadeAndFilterRequest request)
        {
            GetDeclaracaoByEntidadeAndFilterResponse response = new GetDeclaracaoByEntidadeAndFilterResponse();

            if (request.filter == null)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                });
            else if (!request.filter.dateFilterBegin.HasValue)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DateDoesNotExists).ToString(),
                    ErrorMessage = ErrorsDataContract.DateDoesNotExists.ToString()
                });
            else
            {
                DateTime date = request.filter.dateFilterBegin.Value;
                //get declaracoes for moth requested
                date = new DateTime(date.Year, date.Month, 1);
                const int SECRET_A = 987654321;
                const int SECRET_B = 123456789;
                int decodedId = (request.IdEntidade - SECRET_B) / SECRET_A;

                List<Declaracaoremuneracao> declaracoesModel = _unitOfWork.DeclaracaoRemuneracaoRepository.GetByEntidadeAndDate(decodedId, date);
                List<DeclaracaoremuneracaoDto> declaracoes = new List<DeclaracaoremuneracaoDto>();
                bool declaracoesExists = false;

                //get declaracoes for previous month of requested
                if (declaracoesModel.Count == 0)
                {
                    declaracoesModel = _unitOfWork.DeclaracaoRemuneracaoRepository.GetLastDeclaracaoOficiosa(decodedId, date);
                    // Último dia do mês recebido no request
                    int lastDay = date.AddMonths(1).AddDays(-1).Day;
                    foreach (Declaracaoremuneracao declaracaoModel in declaracoesModel)
                    {
                        DeclaracaoremuneracaoDto generatedDeclaracao = Utils.MappClassToDto<Declaracaoremuneracao, DeclaracaoremuneracaoDto>(declaracaoModel);
                        generatedDeclaracao.IdDeclaracao = 0;
                        generatedDeclaracao.RegimeFk = 0;
                        generatedDeclaracao.SexoFk = 0;
                        generatedDeclaracao.NacionalidadeFk = 0;
                        generatedDeclaracao.ContaCorrenteFk = 0;
                        declaracoes.Add(generatedDeclaracao);
                    }
                }
                else
                {
                    declaracoesExists = true;
                    foreach (Declaracaoremuneracao declaracaoModel in declaracoesModel)
                    {
                        declaracoes.Add(Utils.MappClassToDto<Declaracaoremuneracao, DeclaracaoremuneracaoDto>(declaracaoModel));
                    }
                }

                //filter only working Workers
                List<int> relIds = _unitOfWork.RelEntidadeTrabalhadorRepository.GetAllRelTrabalhadorByEntidadeAndMonth(decodedId, date);
                List<int> relSuspensosIds = _unitOfWork.SuspensaoRepository.GetAllRelEntidadeTrabalhadorSuspensosByDate(decodedId, date, date.AddMonths(1).AddDays(-1));

                DeclaracaoremuneracaoDto declaracao;
                DeclaracaoListagem declaracaoListagem;
                response.declaracoes = new List<DeclaracaoListagem>();
                //Para todas as relações de trabalhadores com entidades valida se existe pelo menos um trabalhador relacionado e se existe alguma declaração, senão cria uma nova vazia
                bool exists;
                foreach (int idRel in relIds)
                {
                    exists = false;
                    //Caso haja alguma relação entre este trabalhador e a entidadem que não esteja suspenso
                    if (!relSuspensosIds.Contains(idRel))
                    {
                        //Se encontrar declaração ele constroi a declaração com dados já existentes, senão ele cria uma nova vazia
                        declaracao = declaracoes.Find(d => d.DeclaracaoRelEntidadeTrabalhadorFk == idRel);
                        if (declaracao == null)
                            declaracaoListagem = BuildEmptyDeclaracaoDataContract(idRel, date);
                        else
                        {
                            declaracaoListagem = BuildDeclaracaoDataContract(declaracao, date);
                            exists = true;
                        }

                        //Se não houver informação do trabalhador quer dizer que existem erros com a declaração
                        if (declaracaoListagem.trabalhadorInfo == null)
                        {
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.DeclaracaoComErro).ToString(),
                                ErrorMessage = ErrorsDataContract.DeclaracaoComErro.ToString()
                            });
                            return response;
                        }
                        else
                        {
                            //caso não haja nenhuma declaração, ele vai criar uma nova vazia consoante as regras de negócio estabelecidas
                            if (!declaracoesExists || !exists)
                            {
                                RelentidadetrabalhadorDto rel = _unitOfWork.RelEntidadeTrabalhadorRepository.GetDto(idRel);

                                //Como foi dito nas regras de negócio, e segundo os calculos fornecidos, o dia inicial e o ultimo dia do mês são calculados conforme o mês, pois nem todos os meses têm o dia final como 30
                                int diaInicial = 0;
                                int diaFinal = 30;

                                if (rel.DtIniFimTrabalhador.HasValue && rel.DtIniFimTrabalhador.Value.Month == date.Month)
                                    diaFinal = rel.DtIniFimTrabalhador.Value.Day;

                                if (rel.DtIniVincTrabalhador.Month == date.Month)
                                    diaInicial = rel.DtIniVincTrabalhador.Day - 1;

                                declaracaoListagem.declaracao.diasContrato = diaFinal - diaInicial;

                                int diaFinalMes = date.AddMonths(1).AddDays(-1).Day;

                                if (declaracaoListagem.declaracao.diasContrato > 30 || (diaInicial == 0 && diaFinal >= diaFinalMes))
                                    declaracaoListagem.declaracao.diasContrato = 30;

                                // Este parte conta todos os dias que são admissiveis para a segurança social (dias trabalhados, dias de parentalidade e com faltas injustificadas), para depois fazer a diferença com os dias que estão no contrato
                                decimal diasTrabcontabSegSocialPart1 = declaracaoListagem.declaracao.diasEfecTrabalhados + declaracaoListagem.declaracao.diasParentalidade + declaracaoListagem.declaracao.faltasInjustific;
                                decimal diasTrabcontabSegSocialPart2 = diasTrabcontabSegSocialPart1 - declaracaoListagem.declaracao.diasContrato;

                                declaracaoListagem.declaracao.diasEfecTrabalhados = declaracaoListagem.declaracao.diasEfecTrabalhados - diasTrabcontabSegSocialPart2;
                                declaracaoListagem.declaracao.diasTrabcontabSegSocial = declaracaoListagem.declaracao.diasContrato;
                            }

                            response.declaracoes.Add(declaracaoListagem);
                        }
                    }
                }
            }

            return response;
        }

        public ResumoDeclaracaoResponse GetResumoDeclaracao(GetResumoDeclaracaoRequest request)
        {
            ResumoDeclaracaoResponse response = new ResumoDeclaracaoResponse();

            List<DeclaracaoRemuneracaoDataContract> declaracoes = request.declaracoes;
            Dictionary<int, NacionalidadeResumoDeclaracao> sumNacionalidades = new Dictionary<int, NacionalidadeResumoDeclaracao>();
            Dictionary<int, RegimesResumoDeclaracao> sumRegimes = new Dictionary<int, RegimesResumoDeclaracao>();
            decimal sumDeclaracoes = 0;
            decimal sumDeclaracoesTotal = 0;
            NacionalidadeResumoDeclaracao totalNacionalidades;
            TotalResumoDeclaracao total;
            int totalTrabalhadores = declaracoes.Count;

            int relID;
            string nacionalidade;
            Relentidadetrabalhador rel;
            Trabalhador trab;
            Regime regime;
            decimal nacionais = 0;
            int entidadeId = 0;

            foreach (DeclaracaoRemuneracaoDataContract declaracao in declaracoes)
            {
                relID = declaracao.declaracaoRelEntidadeTrabalhadorFk;
                rel = _unitOfWork.RelEntidadeTrabalhadorRepository.Get(relID);
                trab = _unitOfWork.TrabalhadoresRepository.Get(rel.TrabalhadorFk);
                nacionalidade = _unitOfWork.DominioRepository.Get(trab.NacionalidadeTrabalhador).Descricao;
                sumDeclaracoesTotal += declaracao.remunDeclarada + declaracao.decimoTerceiro;
                if (entidadeId == 0)
                    entidadeId = rel.EntidadeFk;

                if (sumNacionalidades.ContainsKey(trab.NacionalidadeTrabalhador))
                {
                    sumNacionalidades[trab.NacionalidadeTrabalhador].total += declaracao.remunDeclarada + declaracao.decimoTerceiro;
                    sumNacionalidades[trab.NacionalidadeTrabalhador].trabalhadores++;
                }
                else
                    sumNacionalidades[trab.NacionalidadeTrabalhador] = new NacionalidadeResumoDeclaracao
                    {
                        nacionalidade = nacionalidade,
                        total = declaracao.remunDeclarada + declaracao.decimoTerceiro,
                        trabalhadores = 1
                    };

                if (nacionalidade != "Estrangeiro (não desconta em TL)")
                {
                    if (nacionalidade == "Timor-Leste")
                        nacionais++;

                    regime = _unitOfWork.RegimeRepository.GetRegimeByParentAndDate(rel.RegimeFk, declaracao.mesAno);
                    if (regime != null)
                    {
                        sumDeclaracoes += declaracao.remunDeclarada + declaracao.decimoTerceiro;

                        if (sumRegimes.ContainsKey(rel.RegimeFk))
                            sumRegimes[rel.RegimeFk].remuneracoes += declaracao.remunDeclarada + declaracao.decimoTerceiro;
                        else
                            sumRegimes[rel.RegimeFk] = new RegimesResumoDeclaracao
                            {
                                remuneracoes = declaracao.remunDeclarada + declaracao.decimoTerceiro,
                                regime = regime.NomeRegime,
                                taxaEntidade = regime.PercentEntidadeEmpreg,
                                taxaTrabalhador = regime.PercentTrabalhador,
                                contribuicoes = 0,
                                quotizacoes = 0,
                                total = 0
                            };
                    }
                }
            }

            decimal totalQuotizacoes = 0;
            decimal totalContribuicoes = 0;
            decimal totalRemuneracoes = 0;
            decimal dispensaContributiva = 0;

            //Caso o número de trabalhadores da empresa seja menor ou igual a 10, pelo menos 60% dos trabalhadores da empresa forem nacionais e a empresa não tenha guias não pagas com a data de vencimento superior à data atual
            //Ele vai validar se não tem dividas, para poder ir buscar o ano de declaração para poder exibir a dispensa contributiva no resumo
            if (totalTrabalhadores > 0 && totalTrabalhadores <= 10 &&
                totalTrabalhadores * 0.6M <= nacionais && entidadeId > 0)
            {
                bool semDividas = _unitOfWork.ContaCorrenteRepository.IsEntidadeRegularizada(entidadeId);

                if (semDividas)
                {
                    int ano = declaracoes[0].mesAno.Year;
                    dispensaContributiva = _unitOfWork.DispensaContributivaRepository.getDispensaContributivaByYear(ano);
                }
            }

            foreach (RegimesResumoDeclaracao resumoRegime in sumRegimes.Values)
            {
                //Consoante o regime que o que declarou a contar com o subsidio (decimo terceiro mes) ele adiciona ao total de contribuições e quotizações para calcular o resumo para ser exposto
                resumoRegime.contribuicoes = resumoRegime.remuneracoes * resumoRegime.taxaEntidade * 0.01M * (100 - dispensaContributiva) * 0.01M;
                resumoRegime.quotizacoes = resumoRegime.remuneracoes * resumoRegime.taxaTrabalhador * 0.01M;
                resumoRegime.total = resumoRegime.contribuicoes + resumoRegime.quotizacoes;
                totalQuotizacoes += resumoRegime.quotizacoes;
                totalContribuicoes += resumoRegime.contribuicoes;
                totalRemuneracoes += resumoRegime.remuneracoes;
            }

            totalNacionalidades = new NacionalidadeResumoDeclaracao
            {
                nacionalidade = "TOTAL",
                total = sumDeclaracoesTotal,
                trabalhadores = totalTrabalhadores
            };

            total = new TotalResumoDeclaracao
            {
                remuneracoes = sumDeclaracoes,
                contribuicoes = totalContribuicoes,
                quotizacoes = totalQuotizacoes,
                total = totalContribuicoes + totalQuotizacoes,
            };

            response.nacionalidades = sumNacionalidades.Values.ToList();
            response.totalNacionalidades = totalNacionalidades;
            response.regimes = sumRegimes.Values.ToList();
            response.total = total;

            return response;
        }

        public Declaracaoremuneracao BuildDeclaracaoRemuneracaoObject(DeclaracaoRemuneracaoDataContract declaracao)
        {
            DeclaracaoremuneracaoDto declaracaoModel = new DeclaracaoremuneracaoDto
            {
                IdDeclaracao = declaracao.idDeclaracao,
                DeclaracaoRelEntidadeTrabalhadorFk = declaracao.declaracaoRelEntidadeTrabalhadorFk,
                DiasContrato = declaracao.diasContrato,
                DiasEfecTrabalhados = declaracao.diasEfecTrabalhados,
                FaltasInjustific = declaracao.faltasInjustific,
                DiasParentalidade = declaracao.diasParentalidade,
                DiasTrabcontabSegSocial = declaracao.diasTrabcontabSegSocial,
                RemunDeclarada = declaracao.remunDeclarada,
                DecimoTerceiro = declaracao.decimoTerceiro,
                MesAno = declaracao.mesAno,
                ContaCorrenteFk = declaracao.contaCorrenteFk,
                Oficioso = declaracao.oficioso,
                RegimeFk = declaracao.regimeFk,
                SexoFk = declaracao.sexoFk,
                NacionalidadeFk = declaracao.nacionalidadeFk,
                FlagImportado = declaracao.flagImportado,
                IndActivo = true,
                UtilizadorCriacao = 0,
                DataCriacao = DateTime.Now,
                Ipv6 = ""
            };

            if (declaracaoModel.IdDeclaracao > 0)
            {
                Declaracaoremuneracao original = _unitOfWork.DeclaracaoRemuneracaoRepository.Get(declaracaoModel.IdDeclaracao);
                declaracaoModel.UtilizadorCriacao = original.UtilizadorCriacao;
                declaracaoModel.DataCriacao = original.DataCriacao;
                declaracaoModel = _utils.UpdateDetailsToEntity(declaracaoModel);
            }
            else
                declaracaoModel = _utils.SetDetailsToEntity(declaracaoModel);
            return Utils.MappClassFromDto<DeclaracaoremuneracaoDto, Declaracaoremuneracao>(declaracaoModel);
        }

        public ResponseBaseDataContract SaveDeclaracao(SaveDeclaracoesRequest request, bool oficioso)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            List<Declaracaoremuneracao> declaracoes = new List<Declaracaoremuneracao>();

            foreach (var declaracao in request.declaracoes)
            {
                declaracoes.Add(BuildDeclaracaoRemuneracaoObject(declaracao));
            }

            int relID;
            string nacionalidade;
            Relentidadetrabalhador rel;
            Trabalhador trab;
            Regime regime;
            decimal totalContribuicoes = 0;
            decimal totalQuotizacoes = 0;
            int totalTrabalhadores = request.declaracoes.Count();
            decimal nacionais = 0;
            int diaVencimento = int.MaxValue;
            List<Tuple<int, decimal>> descontantes = new List<Tuple<int, decimal>>();

            if (totalTrabalhadores > 0)
            {
                foreach (var declaracao in request.declaracoes)
                {
                    relID = declaracao.declaracaoRelEntidadeTrabalhadorFk;
                    rel = _unitOfWork.RelEntidadeTrabalhadorRepository.Get(relID);
                    trab = _unitOfWork.TrabalhadoresRepository.Get(rel.TrabalhadorFk);
                    nacionalidade = _unitOfWork.DominioRepository.Get(trab.NacionalidadeTrabalhador).Descricao;

                    if (nacionalidade != "Estrangeiro (não desconta em TL)")
                    {
                        if (nacionalidade == "Timor - Leste")
                            nacionais++;

                        descontantes.Add(new Tuple<int, decimal>(rel.RegimeFk, declaracao.remunDeclarada + declaracao.decimoTerceiro));
                    }
                }

                decimal dispensaContributiva = 0;
                //Regra de negócio de dispensa contributiva:
                //Caso o número de trabalhadores da empresa seja menor ou igual a 10, pelo menos 60% dos trabalhadores da empresa forem nacionais e a empresa não tenha guias não pagas com a data de vencimento superior à data atual
                if (totalTrabalhadores <= 10 && totalTrabalhadores * 0.6M <= nacionais)
                {
                    bool semDividas = _unitOfWork.ContaCorrenteRepository.IsEntidadeRegularizada(request.entidadeId);

                    if (semDividas)
                    {
                        int ano = request.data.Year;
                        dispensaContributiva = _unitOfWork.DispensaContributivaRepository.getDispensaContributivaByYear(ano);
                    }
                }

                foreach (Tuple<int, decimal> tuple in descontantes)
                {
                    regime = _unitOfWork.RegimeRepository.GetRegimeByParentAndDate(tuple.Item1, request.data);
                    if (regime != null)
                    {
                        //Regra de negocio: consoante o regime que o trabalhador se encontra e o que declarou a contar com o subsidio (decimo terceiro mes) ele adiciona ao total de contribuições e quotizações
                        decimal contribuicoes = tuple.Item2 * regime.PercentEntidadeEmpreg * 0.01M * (100 - dispensaContributiva) * 0.01M;
                        decimal quotizacoes = tuple.Item2 * regime.PercentTrabalhador * 0.01M;
                        totalQuotizacoes += quotizacoes;
                        totalContribuicoes += contribuicoes;
                        if (regime.DataVencimento < diaVencimento)
                            diaVencimento = regime.DataVencimento;
                    }
                }

                //Por default, se algum dia o valor exceder o limite aceite por sistema, o valor passa para dia 10, como foi definido como regra de negócio
                //Só um pequeno arranjo, pois no futuro haverá dias de vencimento que vão exceder o limite maximo
                if (diaVencimento == int.MaxValue)
                    diaVencimento = 10;
            }

            var total = totalContribuicoes + totalQuotizacoes;

            Dominio contribuicoesDominio = _unitOfWork.DominioRepository.getDominioByDescricao(TiposDominio.TIPODIVIDA, "Contribuições");
            Dominio guiaNaoGeradaDominio = _unitOfWork.DominioRepository.getDominioByDescricao(TiposDominio.SITUACAOPAGAMENTO, "Sem Guia Gerada");

            ContacorrenteDto contaExistente = new ContacorrenteDto();
            ContacorrenteDto contaDto;

            if (declaracoes.Count > 0)
            {
                var relEtnidadeTrabalhador = _unitOfWork.RelEntidadeTrabalhadorRepository.Get(declaracoes[0].DeclaracaoRelEntidadeTrabalhadorFk);
                Contacorrente contaExistenteBD = _unitOfWork.ContaCorrenteRepository.GetContaCorrenteByMesAnoAndEntidadeId(declaracoes[0].MesAno, relEtnidadeTrabalhador.EntidadeFk);
                contaExistente = Utils.MappClassToDto<Contacorrente, ContacorrenteDto>(contaExistenteBD);
            }

            if (contaExistente != null)
            {
                if (contaExistente.SituacaoPagamento == guiaNaoGeradaDominio.IdDominio)
                {
                    contaDto = contaExistente;
                    contaDto.DataVencimento = new DateTime(request.data.Year, request.data.Month, diaVencimento).AddMonths(1);
                    contaDto.ValorEntidade = totalContribuicoes;
                    contaDto.ValorTrabalhador = totalQuotizacoes;
                    contaDto.ValorTotal = total;
                }
                else
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.ContaWrongPaymentState).ToString(),
                        ErrorMessage = ErrorsDataContract.ContaWrongPaymentState.ToString()
                    });
                    return response;
                }
            }
            else
            {
                contaDto = new ContacorrenteDto
                {
                    ContaCorrenteEntidadeFk = request.entidadeId,
                    MesAno = request.data,
                    DataVencimento = new DateTime(request.data.Year, request.data.Month, diaVencimento).AddMonths(1),
                    ValorEntidade = totalContribuicoes,
                    ValorTrabalhador = totalQuotizacoes,
                    ValorTotal = total,
                    SituacaoPagamento = guiaNaoGeradaDominio.IdDominio,
                    IndActivo = true,
                    TipoDivida = contribuicoesDominio.IdDominio
                };
            }

            try
            {
                contaDto = CalculateInterestForContaCorrent(contaDto, request.data, diaVencimento);
            }
            catch
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroAoCalcularJuros).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroAoCalcularJuros.ToString()
                });
                return response;
            }

            bool contaCriada = contaDto.IdContaCorrente > 0;
            if (contaCriada)
                contaDto = _utils.UpdateDetailsToEntity(contaDto);
            else
                contaDto = _utils.SetDetailsToEntity(contaDto);

            Contacorrente conta = Utils.MappClassFromDto<ContacorrenteDto, Contacorrente>(contaDto);
            
            if (contaCriada)
                _unitOfWork.ContaCorrenteRepository.Update(conta);
            else
                _unitOfWork.ContaCorrenteRepository.Add(conta);

            foreach (var declaracao in declaracoes)
            {
                declaracao.Oficioso = oficioso;

                if (contaCriada)
                    declaracao.ContaCorrenteFk = conta.IdContaCorrente;
                else
                    declaracao.ContaCorrenteFkNavigation = conta;

                if (declaracao.IdDeclaracao > 0)
                    _unitOfWork.DeclaracaoRemuneracaoRepository.Update(declaracao);
                else
                    _unitOfWork.DeclaracaoRemuneracaoRepository.Add(declaracao);
            }

            return response;
        }


        private ContacorrenteDto CalculateInterestForContaCorrent(ContacorrenteDto contaDto, DateTime data, int diaVencimento)
        {
            DateTime now = DateTime.Now;

            if (now.Day < diaVencimento)
                now = now.AddMonths(-1);

            now = new DateTime(now.Year, now.Month, 1);
            DateTime iterator = data.AddMonths(1);
            if (iterator <= now)
            {
                Dominio contribuicoesJuroDominio = _unitOfWork.DominioRepository.getDominioByDescricao(TiposDominio.TIPODIVIDA, "Contribuições e Juros");
                contaDto.TipoDivida = contribuicoesJuroDominio.IdDominio;

                //decimal valorJuros = 0;
                //Taxajuromensal taxaJuro;
                //Dominio contribuicoesJuroDominio = _unitOfWork.DominioRepository.getDominioByDescricao(TiposDominio.TIPODIVIDA, "Contribuições e Juros");
                //while (iterator <= now)
                //{
                //    taxaJuro = _unitOfWork.TaxaJuroMensalRepository.GetTaxaJuroMensalByData(iterator);
                //    valorJuros += (contaDto.ValorEntidade + contaDto.ValorTrabalhador) * taxaJuro.Percentagem * 0.01M;
                //    iterator = iterator.AddMonths(1);
                //}

                //contaDto.TipoDivida = contribuicoesJuroDominio.IdDominio;
                //contaDto.ValorJuros = valorJuros;
                //contaDto.ValorTotal = contaDto.ValorEntidade + contaDto.ValorTrabalhador + juroapurado;
            }
            return contaDto;
        }

        public ResponseBaseDataContract SubmitDeclaracao(SaveDeclaracoesRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                response = SaveDeclaracao(request, true);

                if (response.Errors?.Count > 0)
                    return response;
                else
                    _unitOfWork.Commit();
                // update CONTACORRENTE again
                Contacorrente contaExistenteBD = _unitOfWork.ContaCorrenteRepository.GetContaCorrenteByMesAnoAndEntidadeId(request.data, request.entidadeId);
                if (contaExistenteBD != null)
                {
                    _unitOfWork.ContaCorrenteRepository.UpdateCorrente(contaExistenteBD.IdContaCorrente);
                    //_unitOfWork.Commit();
                }

                // response = AutoGenerateNextDeclarations(request.entidadeId, request.data);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }
            return response;
        }


        public ResponseBaseDataContract AutoGenerateNextDeclarations(int entidadeId, DateTime beginDate)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            DateTime now = DateTime.Now;
            DateTime currentMonth = new DateTime(now.Year, now.Month, 1);

            //check day
            SingleDominioDescricaoStringResponse diaDeclaracao = _dominioManager.GetDeclarationDay("TE");
            if (now.Day < diaDeclaracao.dominio.value)
                currentMonth = currentMonth.AddMonths(-1);

            GetDeclaracaoByEntidadeAndFilterRequest getRequest = new GetDeclaracaoByEntidadeAndFilterRequest
            {
                filter = new SearchFilter()
            };
            getRequest.IdEntidade = entidadeId;

            SaveDeclaracoesRequest saveRequest = new SaveDeclaracoesRequest
            {
                entidadeId = entidadeId
            };
            GetDeclaracaoByEntidadeAndFilterResponse declaracoes;
            DateTime iterationDate = beginDate.AddMonths(1);

            try
            {
                while (iterationDate <= currentMonth)
                {
                    getRequest.filter.dateFilterBegin = iterationDate;
                    declaracoes = GetDeclaracaoByEntidadeAndFilter(getRequest);
                    if (declaracoes.declaracoes.Count > 0)
                    {
                        if (declaracoes.declaracoes.First().declaracao.idDeclaracao == 0)
                        {
                            saveRequest.declaracoes = declaracoes.declaracoes.Select(d => d.declaracao).ToList();
                            saveRequest.data = iterationDate;

                            response = SaveDeclaracao(saveRequest, false);

                            if (response.Errors?.Count > 0)
                                throw new Exception();
                            else
                                _unitOfWork.Commit();
                        }
                        else
                            break;
                    }

                    iterationDate = iterationDate.AddMonths(1);
                }
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                StringBuilder sb = new StringBuilder();
                sb.Append(((int)ErrorsDataContract.DeclaracaoPosteriorComErro).ToString());
                // ÿ é o valor de separador de datas para ser identificado no front para poder traduzir a mensagem de warning - ex: "As datas x e z (xÿz) sobrepoem a uma data de orçamento já existente"
                sb.Append("ÿ");
                sb.Append(iterationDate.ToString("yyyy-MM"));
                Error newError = new Error
                {
                    ErrorCode = sb.ToString(),
                    ErrorMessage = ErrorsDataContract.DeclaracaoPosteriorComErro.ToString()
                };

                if (response.Errors.Count > 0)
                    response.Errors.Add(newError);
                else
                    response.Errors = new List<Error> { newError };
            }
            return response;
        }

        public RelatorioDeclaracaoRenumeracaoListagemResponse GetDeclaracoesRelatorios(RelatorioDeclaracaoRenumeracaoListagemRequest request)
        {
            RelatorioDeclaracaoRenumeracaoListagemResponse response = new RelatorioDeclaracaoRenumeracaoListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.DeclaracaoRemuneracaoRepository.GetDeclaracoesRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public StringFileReponse ExtractToExcelRelatorios(RelatorioDeclaracaoRenumeracaoListagemRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            request.filter.index = 0;
            request.filter.rows = 999999;

            var declaracoes = _unitOfWork.DeclaracaoRemuneracaoRepository.GetDeclaracoesRelatorios(request);

            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["situacaoContributiva"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            // Se no website, o utilizador escolheu Trabalhador, aparece o nome do trabalhador, caso contrário o nome do Empregador
            var nome = declaracoes.declaracoes.Any() ? (request.isTrabalhador ? declaracoes.declaracoes.First().nomeTrabalhador : declaracoes.declaracoes.First().nomeEmpregador) : "";

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["consultaSituacaoContributiva"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(6, 1));
            excelDocument.Pages[0].AddText(nome, new ExcelDocumentTextPosition(1, 2), "Header", new ExcelDocumentTextPosition(6, 2));

            bool hasBeginDate = request.filter.dateFilterBegin.HasValue;
            bool hasEndDate = request.filter.dateFilterEnd.HasValue;
            bool hasDate = (hasBeginDate && hasEndDate) || hasBeginDate;

            if (hasBeginDate && hasEndDate)
            {
                var date = $"{request.filter.dateFilterBegin.Value.ToString("yyyy/MM")} {_localizer["a"].Value} {request.filter.dateFilterEnd.Value.ToString("yyyy/MM")}";
                excelDocument.Pages[0].AddText(date, new ExcelDocumentTextPosition(1, 3), "Header", new ExcelDocumentTextPosition(4, 3));
            }
            else if (hasBeginDate)
            {
                var date = $"{request.filter.dateFilterBegin.Value.ToString("yyyy/MM")}";
                excelDocument.Pages[0].AddText(date, new ExcelDocumentTextPosition(1, 3), "Header", new ExcelDocumentTextPosition(4, 3));
            }

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, hasDate ? 5 : 4), declaracoes.declaracoes, new List<ColumnOption<RelatorioDeclaracaoRenumeracaoDataContract>>()
            {
                new ColumnOption<RelatorioDeclaracaoRenumeracaoDataContract>()
                {
                    Name = _localizer["mesReferencia"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.mesAno.ToString("yyyy/MM")
                },
                new ColumnOption<RelatorioDeclaracaoRenumeracaoDataContract>()
                {
                    Name = _localizer["empregador"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.nomeEmpregador
                },
                new ColumnOption<RelatorioDeclaracaoRenumeracaoDataContract>()
                {
                    Name = _localizer["valorRenumeracoesEmpregadorTrabalhador"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.valorRenumeracoes
                },
                new ColumnOption<RelatorioDeclaracaoRenumeracaoDataContract>()
                {
                    Name = _localizer["valorContribuicoes"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.valorContribuicoes
                },
                new ColumnOption<RelatorioDeclaracaoRenumeracaoDataContract>()
                {
                    Name = _localizer["valorPago"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.valorPago
                },
                new ColumnOption<RelatorioDeclaracaoRenumeracaoDataContract>()
                {
                    Name = _localizer["valorDivida"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.valorDivida
                }
            });

            // Conversão do documento excel em base64
            response.File = excelDocument.GetFileString();
            return response;
        }
    }
}