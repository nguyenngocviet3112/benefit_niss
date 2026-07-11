using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataManager.DataManagers;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using TimorINSSBackEnd.Repository.Repositories;
using static TimorINSSBackEnd.ExcelDocumentService.Enums;
using static TimorINSSBackEnd.ExcelDocumentService.Models;

namespace TimorINSSBackEnd.Extensions
{
    public static class ServiceExtensions
    {
        public static readonly Dictionary<string, ExcelDocumentTextStyle> ExcelDocumentTextStyles = new Dictionary<string, ExcelDocumentTextStyle>()
        {
            {
                "Header",
                new ExcelDocumentTextStyle()
                {
                    Color = "#fff",
                    BackgroundColor = "#2A81CC",
                    Bold = true
                }
            },
            {
                "HeaderWrap",
                new ExcelDocumentTextStyle()
                {
                    Color = "#fff",
                    BackgroundColor = "#2A81CC",
                    Bold = true,
                    WrapText = true
                }
            },
            {
                "HeaderAlignLeftMoney",
                new ExcelDocumentTextStyle()
                {
                    NumberFormat = "$ #,##0.00",
                    Color = "#fff",
                    BackgroundColor = "#2A81CC",
                    Bold = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Left
                }
            },
            {
                "HeaderAlignRight",
                new ExcelDocumentTextStyle()
                {
                    Color = "#fff",
                    BackgroundColor = "#2A81CC",
                    Bold = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Right
                }
            },
            {
                "TableColumn",
                new ExcelDocumentTextStyle()
                {
                    Color = "#fff",
                    BackgroundColor = "#2A81CC",
                    Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
                    HorizontalAlign = ExcelHorizontalAlignment.Left,
                    Bold = true
                }
            },
            {
                "TableCell",
                new ExcelDocumentTextStyle()
                {
                    Color = "#000",
                    Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
                    HorizontalAlign = ExcelHorizontalAlignment.Left
                }
            },
            {
                "TableCellWrap",
                new ExcelDocumentTextStyle()
                {
                    Color = "#000",
                    Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
                    HorizontalAlign = ExcelHorizontalAlignment.Left,
                    WrapText = true
                }
            },
            {
                "TableCellWrapGreenCenter",
                new ExcelDocumentTextStyle()
                {
                    Color = "#00ff00",
                    Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    WrapText = true
                }
            },
            {
                "TableCellWrapRedCenter",
                new ExcelDocumentTextStyle()
                {
                    Color = "#ff0000",
                    Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    WrapText = true
                }
            },
            {
                "TableCellMoney",
                new ExcelDocumentTextStyle()
                {
                    NumberFormat = "$ #,##0.00",
                    Color = "#000",
                    Border = ExcelDocumentBorderType.Top | ExcelDocumentBorderType.Right | ExcelDocumentBorderType.Bottom | ExcelDocumentBorderType.Left,
                    HorizontalAlign = ExcelHorizontalAlignment.Left
                }
            }
        };

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }

        public static void ConfigureSqlServerContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<TimorINSSModuloContribuicoesContext>(o => o.UseSqlServer(config.GetConnectionString("sqlserverconnection")));
        }

        public static void ConfigureScopes(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Repositories
            services.AddScoped<IActividadeEconomicaRepository, ActividadeEconomicaRepository>();
            services.AddScoped<IAldeiaRepository, AldeiaRepository>();
            services.AddScoped<ICamposEditaveisRepository, CamposEditaveisRepository>();
            services.AddScoped<IClassificacaoRepository, ClassificacaoRepository>();
            services.AddScoped<IComponenteAccoesTarefaRepository, ComponenteAccoesTarefaRepository>();
            services.AddScoped<IComponenteCarregarDocumentoRepository, ComponenteCarregarDocumentosRepository>();
            services.AddScoped<IComponenteClassificacaoSubClassificRepository, ComponenteClassificacaoSubClassificRepository>();
            services.AddScoped<IComponenteControloAcessoRepository, ComponenteControloAcessoRepository>();
            services.AddScoped<IComponenteOrcamentoRegistoRepository, ComponenteOrcamentoRegistoRepository>();
            services.AddScoped<IComponenteOrcamentoValorRepository, ComponenteOrcamentoValorRepository>();
            services.AddScoped<IComponenteRepository, ComponenteRepository>();
            services.AddScoped<IComponenteTextoRepository, ComponenteTextoRepository>();
            services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
            services.AddScoped<IContactoRepository, ContactoRepository>();
            services.AddScoped<IDeclaracaoremuneracaoRepository, DeclaracaoremuneracaoRepository>();
            services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
            services.AddScoped<IDocumentoIdentificacaoRepository, DocumentoIdentificacaoRepository>();
            services.AddScoped<IDominioRepository, DominioRepository>();
            services.AddScoped<IEntidadeEmpregadoraRepository, EntidadeEmpregadoraRepository>();
            services.AddScoped<IEscalaoRepository, EscalaoRepository>();
            services.AddScoped<IFuncionalidadeRepository, FuncionalidadeRepository>();
            services.AddScoped<IGuiaPagamentoRepository, GuiaPagamentoRepository>();
            services.AddScoped<IINSSEstrangeiroRepository, INSSEstrangeiroRepository>();
            services.AddScoped<IMoradaRepository, MoradaRepository>();
            services.AddScoped<IMunicipioRepository, MunicipioRepository>();
            services.AddScoped<INaturezaJuridicaRepository, NaturezaJuridicaRepository>();
            services.AddScoped<IPaisRepository, PaisRepository>();
            services.AddScoped<IPerfilRepository, PerfilRepository>();
            services.AddScoped<IPostoAdministrativoRepository, PostoAdministrativoRepository>();
            services.AddScoped<IRegimeRepository, RegimeRepository>();
            services.AddScoped<IRelEntidadeResponsavelLegalRepository, RelEntidadeResponsavelLegalRepository>();
            services.AddScoped<IRelEntidadeTrabalhadorRepository, RelEntidadeTrabalhadorRepository>();
            services.AddScoped<IRelPerfilFuncionalidadeRepository, RelPerfilFuncionalidadeRepository>();
            services.AddScoped<IRelTarefaComponenteRepository, RelTarefaComponenteRepository>();
            services.AddScoped<IRelUtilizadorDepartamentoRepository, RelUtilizadorDepartamentoRepository>();
            services.AddScoped<IRelUtilizadorPerfilRepository, RelUtilizadorPerfilRepository>();
            services.AddScoped<IReservaCreditoRepository, ReservaCreditoRepository>();
            services.AddScoped<IResponsavelLegalRepository, ResponsavelLegalRepository>();
            services.AddScoped<ISectorActividadeRepository, SectorActividadeRepository>();
            services.AddScoped<ISubClassificacaoRepository, SubClassificacaoRepository>();
            services.AddScoped<ISucoRepository, SucoRepository>();
            services.AddScoped<ISuspensaoRepository, SuspensaoRepository>();
            services.AddScoped<ITarefaRepository, TarefaRepository>();
            services.AddScoped<ITaxaJuroMensalRepository, TaxaJuroMensalRepository>();
            services.AddScoped<ITrabalhadoresRepository, TrabalhadoresRepository>();
            services.AddScoped<IUtilizadoresRepository, UtilizadoresRepository>();
            services.AddScoped<IRelCodigoContaAgrupamentoConfigRepository, RelCodigoContaAgrupamentoConfigRepository>();
            services.AddScoped<IComponenteDespesaRegistoRepository, ComponenteDespesaRegistoRepository>();
            services.AddScoped<IDestinatarioRepository, DestinatarioRepository>();
            services.AddScoped<IPagamentosExecutadosRepository, PagamentosExecutadosRepository>();
            services.AddScoped<IMovimentosPorConciliarRepository, MovimentosPorConciliarRepository>();
            services.AddScoped<IComponenteReceitaRepository, ComponenteReceitaRepository>();
            services.AddScoped<IComponenteReceitaRegistoRepository, ComponenteReceitaRegistoRepository>();
            services.AddScoped<IUtilizadorTokenRepository, UtilizadorTokenRepository>();
            services.AddScoped<IPagamentosExecutadosRepository, PagamentosExecutadosRepository>();


            //DataManagers
            services.AddScoped<IAldeiaDataManager, AldeiaDataManager>();
            services.AddScoped<IDeclaracaoremuneracaoDataManager, DeclaracaoremuneracaoDataManager>();
            services.AddScoped<IDocumentoIdentificacaoDataManager, DocumentoIdentificacaoDataManager>();
            services.AddScoped<IDominioDataManager, DominioDataManager>();
            services.AddScoped<IEntidadeEmpregadoraDataManager, EntidadeEmpregadoraDataManager>();
            services.AddScoped<IMoradaDataManager, MoradaDataManager>();
            services.AddScoped<IMunicipioDataManager, MunicipioDataManager>();
            services.AddScoped<IDominioDataManager, DominioDataManager>();
            services.AddScoped<IPaisDataManager, PaisDataManager>();
            services.AddScoped<IPostoAdministrativoDataManager, PostoAdministrativoDataManager>();
            services.AddScoped<IRegimeDataManager, RegimeDataManager>();
            services.AddScoped<IRelEntidadeResponsavelLegalDataManager, RelEntidadeResponsavelLegalDataManager>();
            services.AddScoped<IRelEntidadeTrabalhadorDataManager, RelEntidadeTrabalhadorDataManager>();
            services.AddScoped<IResponsavelLegalDataManager, ResponsavelLegalDataManager>();
            services.AddScoped<ISucoDataManager, SucoDataManager>();
            services.AddScoped<ITrabalhadorDataManager, TrabalhadorDataManager>();
            services.AddScoped<IUtilizadorDataManager, UtilizadorDataManager>();
            services.AddScoped<IUtilsDataManager, UtilsDataManager>();
            services.AddScoped<INaturezaJuridicaDataManager, NaturezaJuridicaDataManager>();
            services.AddScoped<IActividadeEconomicaDataManager, ActividadeEconomicaDataManager>();
            services.AddScoped<ISectorActividadeDataManager, SectorActividadeDataManager>();
            services.AddScoped<IContactoDataManager, ContactoDataManager>();
            services.AddScoped<ISuspensaoDataManager, SuspensaoDataManager>();
            services.AddScoped<IINSSEstrangeiroDataManager, INSSEstrangeiroDataManager>();
            services.AddScoped<IContaCorrenteDataManager, ContaCorrenteDataManager>();
            services.AddScoped<IGuiaPagamentoDataManager, GuiaPagamentoDataManager>();
            services.AddScoped<IReservaCreditoDataManager, ReservaCreditoDataManager>();
            services.AddScoped<ITaskDataManager, TaskDataManager>();
            services.AddScoped<IEscalaoDataManager, EscalaoDataManager>();
            services.AddScoped<IPerfilDataManager, PerfilDataManager>();
            services.AddScoped<ICamposEditaveisDataManager, CamposEditaveisDataManager>();
            services.AddScoped<IFuncionalidadeDataManager, FuncionalidadeDataManager>();
            services.AddScoped<IRelPerfilFuncionalidadeDataManager, RelPerfilFuncionalidadeDataManager>();
            services.AddScoped<IDepartamentoDataManager, DepartamentoDataManager>();
            services.AddScoped<IRelUtilizadorPerfilDataManager, RelUtilizadorPerfilDataManager>();
            services.AddScoped<IRelUtilizadorDepartamentoDataManager, RelUtilizadorDepartamentoDataManager>();
            services.AddScoped<IComponenteDataManager, ComponenteDataManager>();
            services.AddScoped<ITarefaDataManager, TarefaDataManager>();
            services.AddScoped<IProcessosDataManager, ProcessosDataManager>();
            services.AddScoped<IClassificacaoDataManager, ClassificacaoDataManager>();
            services.AddScoped<ISubClassificacaoDataManager, SubClassificacaoDataManager>();
            services.AddScoped<IRelTarefaComponenteDataManager, RelTarefaComponenteDataManager>();
            services.AddScoped<IComponenteOrcamentoRegistoDataManager, ComponenteOrcamentoRegistoDataManager>();
            services.AddScoped<IComponenteOrcamentoValorDataManager, ComponenteOrcamentoValorDataManager>();
            services.AddScoped<IAgrupamentoConfigDataManager, AgrupamentoConfigDataManager>();
            services.AddScoped<IComponenteDespesaRegistoDataManager, ComponenteDespesaRegistoDataManager>();
            services.AddScoped<IComponenteDespesaConfigDataManager, ComponenteDespesaConfigDataManager>();
            services.AddScoped<IDestinatarioDataManager, DestinatarioDataManager>();
            services.AddScoped<IPagamentoExecutadoDataManager, PagamentoExecutadoDataManager>();
            services.AddScoped<ICeInssGlobalDataManager, CeInssGlobalDataManager>();
            services.AddScoped<IMovimentosPorConciliarDataManager, MovimentosPorConciliarDataManager>();
            services.AddScoped<IComponenteReceitaConfigDataManager, ComponenteReceitaConfigDataManager>();
            services.AddScoped<IMovimentosBancariosDataManager, MovimentosBancariosDataManager>();
            services.AddScoped<IComponenteReceitaRegistoDataManager, ComponenteReceitaRegistoDataManager>();
            services.AddScoped<IComponenteOrcamentoConfigDataManager, ComponenteOrcamentoConfigDataManager>();
            services.AddScoped<IKhachHangDataManager, KhachHangDataManager>();
            services.AddScoped<IBenefitDataManager, BenefitDataManager>();
            services.AddScoped<IInstitutionDataManager, InstitutionDataManager>();
            services.AddScoped<IProgramActivityDataManager, ProgramActivityDataManager>();
            services.AddScoped<IFunctionalClassificationDataManager, FunctionalClassificationDataManager>();
            services.AddScoped<IUserModeAccessDataManager, UserModeAccessDataManager>();
            services.AddScoped<ICodigoContaTreeDataManager, CodigoContaTreeDataManager>();
            services.AddScoped<IEconomicClassificationDataManager, EconomicClassificationDataManager>();
            services.AddScoped<IOrcamentoDataManager, OrcamentoDataManager>();
            services.AddScoped<IExpenditureAuthorizationDataManager, ExpenditureAuthorizationDataManager>();
            services.AddScoped<ICabimentoDataManager, CabimentoDataManager>();
            services.AddScoped<ICompromissoDespesaDataManager, CompromissoDespesaDataManager>();
            services.AddScoped<IObligationDataManager, ObligationDataManager>();
            services.AddScoped<IUserPermissionDataManager, UserPermissionDataManager>();
            services.AddScoped<ILanguageConfigDataManager, LanguageConfigDataManager>();
            services.AddScoped<IOrcamentoConfigDataManager, OrcamentoConfigDataManager>();
            services.AddScoped<IPaymentDataManager, PaymentDataManager>();

            //Misc
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<ICacheProvider, CacheProvider>();
        }
    }
}