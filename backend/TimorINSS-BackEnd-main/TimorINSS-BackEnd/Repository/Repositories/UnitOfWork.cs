using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using TimorINSSBackEnd.Resources;
using Microsoft.EntityFrameworkCore;


namespace TimorINSSBackEnd.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResource> _localizer;

        //Repository interfaces
        private IAldeiaRepository _aldeiaRepository;
        private IImporterRepository _importerRepository;

        private IContactoRepository _contactoRepository;
        private IComponenteReceitaRegistoMovimentosRepository _componenteReceitaRegistoMovimentosRepository;
        private IDeclaracaoremuneracaoRepository _declaracaoRemuneracaoRepository;
        private IDocumentoIdentificacaoRepository _documentoIdentificacaoRepository;
        private IDominioRepository _dominioRepository;
        private IEntidadeEmpregadoraRepository _entidadeEmpregadoraRepository;
        private IINSSEstrangeiroRepository _inssEstrangeiroRepository;
        private IMoradaRepository _moradaRepository;
        private IMunicipioRepository _municipioRepository;
        private IPaisRepository _paisRepository;
        private IPostoAdministrativoRepository _postoAdministrativoRepository;
        private IRegimeRepository _regimeRepository;
        private IRelEntidadeResponsavelLegalRepository _relEntidadeResponsavelLegalRepository;
        private IRelEntidadeTrabalhadorRepository _relEntidadeTrabalhadorRepository;
        private IResponsavelLegalHistRepository _responsavelLegalHistRepository;
        private IResponsavelLegalRepository _responsavelLegalRepository;
        private ISucoRepository _sucoRepository;
        private ITrabalhadoresRepository _trabalhadoresRepository;
        private IUtilizadoresRepository _utilizadoresRepository;
        private INaturezaJuridicaRepository _naturezaJuridicaRepository;
        private ISectorActividadeRepository _sectorActividadeRepository;
        private IActividadeEconomicaRepository _actividadeEconomicaRepository;
        private ISuspensaoRepository _suspensaoRepository;
        private IContaCorrenteRepository _contaCorrenteRepository;
        private IGuiaPagamentoRepository _guiaPagamentoRepository;
        private IReservaCreditoRepository _reservaCreditoRepository;
        private ITaxaJuroMensalRepository _taxaJuroMensalRepository;
        private IDispensaContributivaRepository _dispensaContributivaRepository;
        private IEscalaoRepository _escalaoRepository;
        private IPerfilRepository _perfilRepository;
        private ICamposEditaveisRepository _camposEditaveisRepository;
        private IFuncionalidadeRepository _funcionalidadeRepository;
        private IDepartamentoRepository _departamentoRepository;
        private IRelPerfilFuncionalidadeRepository _relPerfilFuncionalidadeRepository;
        private IRelUtilizadorPerfilRepository _relUtilizadorPerfilRepository;
        private IRelUtilizadorDepartamentoRepository _relUtilizadorDepartamentoRepository;
        private IOrcamentoConfigRepository _orcamentoConfigRepository;
        private IComponenteRepository _componenteRepository;
        private ITarefaRepository _tarefaRepository;
        private IClassificacaoRepository _classificacaoRepository;
        private ISubClassificacaoRepository _subClassificacaoRepository;
        private IComponenteTextoRepository _componenteTextoRepository;
        private IComponenteAccoesTarefaRepository _componenteAccoesTarefaRepository;
        private IComponenteClassificacaoSubClassificRepository _componenteClassificacaoSubClassificRepository;
        private IComponenteCarregarDocumentoRepository _componenteCarregarDocumentoRepository;
        private IRelTarefaComponenteRepository _relTarefaComponenteRepository;
        private IComponenteControloAcessoRepository _componenteControloAcessoRepository;
        private ICentroCustoRepository _centroCustoRepository;
        private IRelTipoDeContaOrcamentoConfigRepository _relTipoDeContaOrcamentoConfigRepository;
        private IAgrupamentoConfigRepository _agrupamentoConfigRepository;
        private ICodigoContaRepository _codigoContaRepository;
        private IProcessoConfigRepository _processoConfigRepository;
        private IRelCodigoContaAgrupamentoConfigRepository _relCodigoContaAgrupamentoConfigRepository;
        private IRelProcessoConfigPerfilRepository _relProcessoConfigPerfilRepository;
        private IRelProcessoConfigTarefaRepository _relProcessoConfigTarefaRepository;
        private IContaBancariaRepository _contaBancariaRepository;
        private IMovimentoBancarioRepository _movimentoBancarioRepository;
        private IComponenteOrcamentoRepository _componenteOrcamentoRepository;
        private IProcessoAtivoRepository _processoAtivoRepository;
        private ITarefaAtivoRepository _tarefaAtivoRepository;
        private IComponenteDespesaRepository _componenteDespesaRepository;
        private IComponenteOrcamentoRegistoRepository _componenteOrcamentoRegistoRepository;
        private IComponenteOrcamentoValorRepository _componenteOrcamentoValorRepository;
        private IComponenteOrcamentoAjusteRepository _componenteOrcamentoAjusteRepository;
        private IComponenteDespesaRegistoRepository _componenteDespesaRegistoRepository;
        private IComponenteTextoRegistoRepository _componenteTextoRegistoRepository;
        private IPagamentosExecutadosRepository _pagamentosExecutadosRepository;
        private IComponenteDocumentosRegistoRepository _componenteDocumentosRegistoRepository;
        private IComponenteClassificacaoSubClassificRegistoRepository _componenteClassificacaoSubClassificRegistoRepository;
        private IDestinatarioRepository _destinatarioRepository;
        private IComponenteConciliacaoMovimentosRepository _componenteConciliacaoMovimentosRepository;
        private IMovimentosPorConciliarRepository _movimentosPorConciliarRepository;
        private IMovimentosbancariosRepository _movimentosbancariosRepository;
        private IComponenteReceitaRepository _componenteReceitaRepository;
        private IRelMovimentosporconciliarMovimentosRepository _relMovimentosporconciliarMovimentosRepository;
        private IComponenteReceitaRegistoRepository _componenteReceitaRegistoRepository;
        private IUtilizadorTokenRepository _utilizadorTokenRepository;
        private ICompromissoRepository _compromissoRepository;
        private IKhachHangRepository _khachHangRepository;
        private IInstitutionRepository _institutionRepository;




        public UnitOfWork(TimorINSSModuloContribuicoesContext moduloContribuicoesContext, IHttpContextAccessor httpContextAccessor, IStringLocalizer<SharedResource> localizer)
        {
            _moduloContribuicoesContext = moduloContribuicoesContext;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        public IInstitutionRepository InstitutionRepository
        { get { return _institutionRepository ??= new InstitutionRepository(_moduloContribuicoesContext); } }

        //Repository declarations in unit of work
        public IImporterRepository ImporterRepository
        { get { return _importerRepository ??= new ImporterRepository(_moduloContribuicoesContext); } }

        public IAldeiaRepository AldeiaRepository
        { get { return _aldeiaRepository ??= new AldeiaRepository(_moduloContribuicoesContext); } }
        
        public IComponenteReceitaRegistoMovimentosRepository ComponenteReceitaRegistoMovimentosRepository
        { get { return _componenteReceitaRegistoMovimentosRepository ??= new ComponenteReceitaRegistoMovimentosRepository(_moduloContribuicoesContext); } }
        public IContactoRepository ContactoRepository
        { get { return _contactoRepository ??= new ContactoRepository(_moduloContribuicoesContext); } }
        public IDeclaracaoremuneracaoRepository DeclaracaoRemuneracaoRepository
        { get { return _declaracaoRemuneracaoRepository ??= new DeclaracaoremuneracaoRepository(_moduloContribuicoesContext); } }
        public IDocumentoIdentificacaoRepository DocumentoIdentificacaoRepository
        { get { return _documentoIdentificacaoRepository ??= new DocumentoIdentificacaoRepository(_moduloContribuicoesContext); } }
        public IDominioRepository DominioRepository
        { get { return _dominioRepository ??= new DominioRepository(_moduloContribuicoesContext); } }
        public IEntidadeEmpregadoraRepository EntidadeEmpregadoraRepository
        { get { return _entidadeEmpregadoraRepository ??= new EntidadeEmpregadoraRepository(_moduloContribuicoesContext); } }
        public IINSSEstrangeiroRepository INSSEstrangeiroRepository
        { get { return _inssEstrangeiroRepository ??= new INSSEstrangeiroRepository(_moduloContribuicoesContext); } }
        public IMoradaRepository MoradaRepository
        { get { return _moradaRepository ??= new MoradaRepository(_moduloContribuicoesContext, _httpContextAccessor); } }
        public IMunicipioRepository MunicipioRepository
        { get { return _municipioRepository ??= new MunicipioRepository(_moduloContribuicoesContext); } }
        public IPaisRepository PaisRepository
        { get { return _paisRepository ??= new PaisRepository(_moduloContribuicoesContext); } }
        public IPostoAdministrativoRepository PostoAdministrativoRepository
        { get { return _postoAdministrativoRepository ??= new PostoAdministrativoRepository(_moduloContribuicoesContext); } }
        public IRegimeRepository RegimeRepository
        { get { return _regimeRepository ??= new RegimeRepository(_moduloContribuicoesContext); } }
        public IRelEntidadeResponsavelLegalRepository RelEntidadeResponsavelLegalRepository
        { get { return _relEntidadeResponsavelLegalRepository ??= new RelEntidadeResponsavelLegalRepository(_moduloContribuicoesContext); } }
        public IRelEntidadeTrabalhadorRepository RelEntidadeTrabalhadorRepository
        { get { return _relEntidadeTrabalhadorRepository ??= new RelEntidadeTrabalhadorRepository(_moduloContribuicoesContext); } }
        public IResponsavelLegalHistRepository ResponsavelLegalHistRepository
        { get { return _responsavelLegalHistRepository ??= new ResponsavelLegalHistRepository(_moduloContribuicoesContext); } }
        public IResponsavelLegalRepository ResponsavelLegalRepository
        { get { return _responsavelLegalRepository ??= new ResponsavelLegalRepository(_moduloContribuicoesContext); } }
        public ISucoRepository SucoRepository
        { get { return _sucoRepository ??= new SucoRepository(_moduloContribuicoesContext); } }
        public ITrabalhadoresRepository TrabalhadoresRepository
        { get { return _trabalhadoresRepository ??= new TrabalhadoresRepository(_moduloContribuicoesContext, _httpContextAccessor); } }
        public IUtilizadoresRepository UtilizadoresRepository
        { get { return _utilizadoresRepository ??= new UtilizadoresRepository(_moduloContribuicoesContext); } }
        public INaturezaJuridicaRepository NaturezaJuridicaRepository
        { get { return _naturezaJuridicaRepository ??= new NaturezaJuridicaRepository(_moduloContribuicoesContext); } }
        public ISectorActividadeRepository SectorActividadeRepository
        { get { return _sectorActividadeRepository ??= new SectorActividadeRepository(_moduloContribuicoesContext); } }
        public IActividadeEconomicaRepository ActividadeEconomicaRepository
        { get { return _actividadeEconomicaRepository ??= new ActividadeEconomicaRepository(_moduloContribuicoesContext); } }
        public ISuspensaoRepository SuspensaoRepository
        { get { return _suspensaoRepository ??= new SuspensaoRepository(_moduloContribuicoesContext, _httpContextAccessor); } }
        public IContaCorrenteRepository ContaCorrenteRepository
        { get { return _contaCorrenteRepository ??= new ContaCorrenteRepository(_moduloContribuicoesContext, _httpContextAccessor); } }
        public IGuiaPagamentoRepository GuiaPagamentoRepository
        { get { return _guiaPagamentoRepository ??= new GuiaPagamentoRepository(_moduloContribuicoesContext); } }
        public IReservaCreditoRepository ReservaCreditoRepository
        { get { return _reservaCreditoRepository ??= new ReservaCreditoRepository(_moduloContribuicoesContext, _httpContextAccessor); } }
        public ITaxaJuroMensalRepository TaxaJuroMensalRepository
        { get { return _taxaJuroMensalRepository ??= new TaxaJuroMensalRepository(_moduloContribuicoesContext, _httpContextAccessor); } }
        public IDispensaContributivaRepository DispensaContributivaRepository
        { get { return _dispensaContributivaRepository ??= new DispensaContributivaRepository(_moduloContribuicoesContext); } }
        public IEscalaoRepository EscalaoRepository
        { get { return _escalaoRepository ??= new EscalaoRepository(_moduloContribuicoesContext); } }
        public IPerfilRepository PerfilRepository
        { get { return _perfilRepository ??= new PerfilRepository(_moduloContribuicoesContext); } }
        public ICamposEditaveisRepository CamposEditaveisRepository
        { get { return _camposEditaveisRepository ??= new CamposEditaveisRepository(_moduloContribuicoesContext); } }
        public IFuncionalidadeRepository FuncionalidadeRepository
        { get { return _funcionalidadeRepository ??= new FuncionalidadeRepository(_moduloContribuicoesContext); } }
        public IRelUtilizadorPerfilRepository RelUtilizadorPerfilRepository
        { get { return _relUtilizadorPerfilRepository ??= new RelUtilizadorPerfilRepository(_moduloContribuicoesContext); } }
        public IRelPerfilFuncionalidadeRepository RelPerfilFuncionalidadeRepository
        { get { return _relPerfilFuncionalidadeRepository ??= new RelPerfilFuncionalidadeRepository(_moduloContribuicoesContext); } }

        public IDepartamentoRepository DepartamentoRepository
        { get { return _departamentoRepository ??= new DepartamentoRepository(_moduloContribuicoesContext); } }
        public IRelUtilizadorDepartamentoRepository RelUtilizadorDepartamentoRepository
        { get { return _relUtilizadorDepartamentoRepository ??= new RelUtilizadorDepartamentoRepository(_moduloContribuicoesContext); } }
        public IOrcamentoConfigRepository OrcamentoConfigRepository
        { get { return _orcamentoConfigRepository ??= new OrcamentoConfigRepository(_moduloContribuicoesContext); } }
        public IComponenteRepository ComponenteRepository
        { get { return _componenteRepository ??= new ComponenteRepository(_moduloContribuicoesContext); } }
        public ITarefaRepository TarefaRepository
        { get { return _tarefaRepository ??= new TarefaRepository(_moduloContribuicoesContext); } }
        public IClassificacaoRepository ClassificacaoRepository
        { get { return _classificacaoRepository ??= new ClassificacaoRepository(_moduloContribuicoesContext); } }
        public ISubClassificacaoRepository SubClassificacaoRepository
        { get { return _subClassificacaoRepository ??= new SubClassificacaoRepository(_moduloContribuicoesContext); } }

        public IComponenteTextoRepository ComponenteTextoRepository
        { get { return _componenteTextoRepository ??= new ComponenteTextoRepository(_moduloContribuicoesContext); } }
        public IComponenteAccoesTarefaRepository ComponenteAccoesTarefaRepository
        { get { return _componenteAccoesTarefaRepository ??= new ComponenteAccoesTarefaRepository(_moduloContribuicoesContext); } }
        public IComponenteClassificacaoSubClassificRepository ComponenteClassificacaoSubClassificRepository
        { get { return _componenteClassificacaoSubClassificRepository ??= new ComponenteClassificacaoSubClassificRepository(_moduloContribuicoesContext); } }
        public IComponenteCarregarDocumentoRepository ComponenteCarregarDocumentoRepository
        { get { return _componenteCarregarDocumentoRepository ??= new ComponenteCarregarDocumentosRepository(_moduloContribuicoesContext); } }
        public IRelTarefaComponenteRepository RelTarefaComponenteRepository
        { get { return _relTarefaComponenteRepository ??= new RelTarefaComponenteRepository(_moduloContribuicoesContext); } }
        public IComponenteControloAcessoRepository ComponenteControloAcessoRepository
        { get { return _componenteControloAcessoRepository ??= new ComponenteControloAcessoRepository(_moduloContribuicoesContext); } }
        public ICentroCustoRepository CentroCustoRepository
        { get { return _centroCustoRepository ??= new CentroCustoRepository(_moduloContribuicoesContext); } }

        public IRelTipoDeContaOrcamentoConfigRepository RelTipoDeContaOrcamentoConfigRepository
        { get { return _relTipoDeContaOrcamentoConfigRepository ??= new RelTipoDeContaOrcamentoConfigRepository(_moduloContribuicoesContext); } }

        public IAgrupamentoConfigRepository AgrupamentoConfigRepository
        { get { return _agrupamentoConfigRepository ??= new AgrupamentoConfigRepository(_moduloContribuicoesContext, _localizer); } }
        public ICodigoContaRepository CodigoContaRepository
        { get { return _codigoContaRepository ??= new CodigoContaRepository(_moduloContribuicoesContext); } }
        public IProcessoConfigRepository ProcessoConfigRepository
        { get { return _processoConfigRepository ??= new ProcessoConfigRepository(_moduloContribuicoesContext); } }

        public IContaBancariaRepository ContaBancariaRepository
        { get { return _contaBancariaRepository ??= new ContaBancariaRepository(_moduloContribuicoesContext); } }
        public IMovimentoBancarioRepository MovimentoBancarioRepository
        { get { return _movimentoBancarioRepository ??= new MovimentoBancarioRepository(_moduloContribuicoesContext); } }

        public IRelCodigoContaAgrupamentoConfigRepository RelCodigoContaAgrupamentoConfigRepository
        { get { return _relCodigoContaAgrupamentoConfigRepository ??= new RelCodigoContaAgrupamentoConfigRepository(_moduloContribuicoesContext); } }
        public IRelProcessoConfigPerfilRepository RelProcessoConfigPerfilRepository
        { get { return _relProcessoConfigPerfilRepository ??= new RelProcessoConfigPerfilRepository(_moduloContribuicoesContext); } }
        public IRelProcessoConfigTarefaRepository RelProcessoConfigTarefaRepository
        { get { return _relProcessoConfigTarefaRepository ??= new RelProcessoConfigTarefaRepository(_moduloContribuicoesContext); } }
        public IComponenteOrcamentoRepository ComponenteOrcamentoRepository
        { get { return _componenteOrcamentoRepository ??= new ComponenteOrcamentoRepository(_moduloContribuicoesContext); } }
        public IProcessoAtivoRepository ProcessoAtivoRepository
        { get { return _processoAtivoRepository ??= new ProcessoAtivoRepository(_moduloContribuicoesContext); } }
        public ITarefaAtivoRepository TarefaAtivoRepository
        { get { return _tarefaAtivoRepository ??= new TarefaAtivoRepository(_moduloContribuicoesContext); } }
        public IComponenteDespesaRepository ComponenteDespesaRepository
        { get { return _componenteDespesaRepository ??= new ComponenteDespesaRepository(_moduloContribuicoesContext); } }
        public IComponenteOrcamentoRegistoRepository ComponenteOrcamentoRegistoRepository
        { get { return _componenteOrcamentoRegistoRepository ??= new ComponenteOrcamentoRegistoRepository(_moduloContribuicoesContext); } }
        public IComponenteOrcamentoValorRepository ComponenteOrcamentoValorRepository
        { get { return _componenteOrcamentoValorRepository ??= new ComponenteOrcamentoValorRepository(_moduloContribuicoesContext); } }

        public IComponenteOrcamentoAjusteRepository ComponenteOrcamentoAjusteRepository
        { get { return _componenteOrcamentoAjusteRepository ??= new ComponenteOrcamentoAjusteRepository(_moduloContribuicoesContext); } }
        public IComponenteDespesaRegistoRepository ComponenteDespesaRegistoRepository
        { get { return _componenteDespesaRegistoRepository ??= new ComponenteDespesaRegistoRepository(_moduloContribuicoesContext, _localizer); } }
        public IComponenteTextoRegistoRepository ComponenteTextoRegistoRepository
        { get { return _componenteTextoRegistoRepository ??= new ComponenteTextoRegistoRepository(_moduloContribuicoesContext); } }
        public IPagamentosExecutadosRepository PagamentosExecutadosRepository
        { get { return _pagamentosExecutadosRepository ??= new PagamentosExecutadosRepository(_moduloContribuicoesContext, _localizer); } }
        public IComponenteDocumentosRegistoRepository ComponenteDocumentosRegistoRepository
        { get { return _componenteDocumentosRegistoRepository ??= new ComponenteDocumentosRegistoRepository(_moduloContribuicoesContext); } }
        public IComponenteClassificacaoSubClassificRegistoRepository ComponenteClassificacaoSubClassificRegistoRepository
        { get { return _componenteClassificacaoSubClassificRegistoRepository ??= new ComponenteClassificacaoSubClassificRegistoRepository(_moduloContribuicoesContext); } }
        public IDestinatarioRepository DestinatarioRepository
        { get { return _destinatarioRepository ??= new DestinatarioRepository(_moduloContribuicoesContext); } }
        public IComponenteConciliacaoMovimentosRepository ComponenteConciliacaoMovimentosRepository
        { get { return _componenteConciliacaoMovimentosRepository ??= new ComponenteConciliacaoMovimentosRepository(_moduloContribuicoesContext); } }
        public IMovimentosPorConciliarRepository MovimentosPorConciliarRepository
        { get { return _movimentosPorConciliarRepository ??= new MovimentosPorConciliarRepository(_moduloContribuicoesContext, _localizer); } }
        public IMovimentosbancariosRepository MovimentosbancariosRepository
        { get { return _movimentosbancariosRepository ??= new MovimentosbancariosRepository(_moduloContribuicoesContext, _localizer); } }
        public IComponenteReceitaRepository ComponenteReceitaRepository
        { get { return _componenteReceitaRepository ??= new ComponenteReceitaRepository(_moduloContribuicoesContext); } }
        public IRelMovimentosporconciliarMovimentosRepository RelMovimentosPorConciliarMovimentosRepository
        { get { return _relMovimentosporconciliarMovimentosRepository ??= new RelMovimentosporconciliarMovimentosRepository(_moduloContribuicoesContext); } }
        public IComponenteReceitaRegistoRepository ComponenteReceitaRegistoRepository
        { get { return _componenteReceitaRegistoRepository ??= new ComponenteReceitaRegistoRepository(_moduloContribuicoesContext, _localizer); } }
        public IUtilizadorTokenRepository UtilizadorTokenRepository
        { get { return _utilizadorTokenRepository ??= new UtilizadorTokenRepository(_moduloContribuicoesContext); } }
        public ICompromissoRepository CompromissoRepository
        { get { return _compromissoRepository ??= new CompromissoRepository(_moduloContribuicoesContext); } }

        public IKhachHangRepository KhachHangRepository
        { get { return _khachHangRepository ??= new KhachHangRepository(_moduloContribuicoesContext); } }
        //General functions to save changes or rollback
        public void Commit()
        { _moduloContribuicoesContext.SaveChanges(); }

        //public void Commit()
        //{
        //    try
        //    {
        //        _moduloContribuicoesContext.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error SaveChanges: " + ex.Message);

        //        // Lấy tất cả entity đang được theo dõi
        //        var entries = _moduloContribuicoesContext.ChangeTracker.Entries()
        //            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        //        foreach (var entry in entries)
        //        {
        //            Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");

        //            foreach (var prop in entry.Properties)
        //            {
        //                if (prop.CurrentValue == null)
        //                {
        //                    var clrType = prop.Metadata.ClrType;
        //                    bool isNullable = Nullable.GetUnderlyingType(clrType) != null || !clrType.IsValueType;

        //                    if (!isNullable)
        //                    {
        //                        Console.WriteLine($"❌ Field NULL : {prop.Metadata.Name}");
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"(nullable) Field {prop.Metadata.Name} = NULL");
        //                    }
        //                }
        //            }
        //        }

        //        throw; // vẫn ném lỗi ra ngoài để không che bug
        //    }
        //}


        public void Rollback()
        { _moduloContribuicoesContext.Dispose(); }
    }
}