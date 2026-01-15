using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        IImporterRepository ImporterRepository { get; }
        IAldeiaRepository AldeiaRepository { get; }

        IComponenteReceitaRegistoMovimentosRepository ComponenteReceitaRegistoMovimentosRepository { get; }
        IContactoRepository ContactoRepository { get; }
        IDeclaracaoremuneracaoRepository DeclaracaoRemuneracaoRepository { get; }
        IDocumentoIdentificacaoRepository DocumentoIdentificacaoRepository { get; }
        IDominioRepository DominioRepository { get; }
        IEntidadeEmpregadoraRepository EntidadeEmpregadoraRepository { get; }
        IINSSEstrangeiroRepository INSSEstrangeiroRepository { get; }
        IMoradaRepository MoradaRepository { get; }
        IMunicipioRepository MunicipioRepository { get; }
        IPaisRepository PaisRepository { get; }
        IPostoAdministrativoRepository PostoAdministrativoRepository { get; }
        IRegimeRepository RegimeRepository { get; }
        IRelEntidadeResponsavelLegalRepository RelEntidadeResponsavelLegalRepository { get; }
        IRelEntidadeTrabalhadorRepository RelEntidadeTrabalhadorRepository { get; }
        IResponsavelLegalHistRepository ResponsavelLegalHistRepository { get; }
        IResponsavelLegalRepository ResponsavelLegalRepository { get; }
        ISucoRepository SucoRepository { get; }
        ITrabalhadoresRepository TrabalhadoresRepository { get; }
        IUtilizadoresRepository UtilizadoresRepository { get; }
        INaturezaJuridicaRepository NaturezaJuridicaRepository { get; }
        ISectorActividadeRepository SectorActividadeRepository { get; }
        IActividadeEconomicaRepository ActividadeEconomicaRepository { get; }
        ISuspensaoRepository SuspensaoRepository { get; }
        IContaCorrenteRepository ContaCorrenteRepository { get; }
        IGuiaPagamentoRepository GuiaPagamentoRepository { get; }
        IReservaCreditoRepository ReservaCreditoRepository { get; }
        ITaxaJuroMensalRepository TaxaJuroMensalRepository { get; }
        IDispensaContributivaRepository DispensaContributivaRepository { get; }
        IEscalaoRepository EscalaoRepository { get; }
        IPerfilRepository PerfilRepository { get; }
        IRelUtilizadorPerfilRepository RelUtilizadorPerfilRepository { get; }
        ICamposEditaveisRepository CamposEditaveisRepository { get; }
        IFuncionalidadeRepository FuncionalidadeRepository { get; }
        IDepartamentoRepository DepartamentoRepository { get; }
        IInstitutionRepository InstitutionRepository { get; }
        IRelPerfilFuncionalidadeRepository RelPerfilFuncionalidadeRepository { get; }
        IRelUtilizadorDepartamentoRepository RelUtilizadorDepartamentoRepository { get; }
        IOrcamentoConfigRepository OrcamentoConfigRepository { get; }
        IComponenteRepository ComponenteRepository { get; }
        ITarefaRepository TarefaRepository { get; }
        IClassificacaoRepository ClassificacaoRepository { get; }
        ISubClassificacaoRepository SubClassificacaoRepository { get; }
        IComponenteTextoRepository ComponenteTextoRepository { get; }
        IComponenteAccoesTarefaRepository ComponenteAccoesTarefaRepository { get; }
        IComponenteClassificacaoSubClassificRepository ComponenteClassificacaoSubClassificRepository { get; }
        IComponenteCarregarDocumentoRepository ComponenteCarregarDocumentoRepository { get; }
        IRelTarefaComponenteRepository RelTarefaComponenteRepository { get; }
        IComponenteControloAcessoRepository ComponenteControloAcessoRepository { get; }
        ICentroCustoRepository CentroCustoRepository { get; }
        IRelTipoDeContaOrcamentoConfigRepository RelTipoDeContaOrcamentoConfigRepository { get; }
        IAgrupamentoConfigRepository AgrupamentoConfigRepository { get; }
        ICodigoContaRepository CodigoContaRepository { get; }
        IProcessoConfigRepository ProcessoConfigRepository { get; }
        IRelCodigoContaAgrupamentoConfigRepository RelCodigoContaAgrupamentoConfigRepository { get; }
        IRelProcessoConfigPerfilRepository RelProcessoConfigPerfilRepository { get; }
        IRelProcessoConfigTarefaRepository RelProcessoConfigTarefaRepository { get; }
        IContaBancariaRepository ContaBancariaRepository { get; }
        IMovimentoBancarioRepository MovimentoBancarioRepository { get; }
        IComponenteOrcamentoRepository ComponenteOrcamentoRepository { get; }
        IProcessoAtivoRepository ProcessoAtivoRepository { get; }
        ITarefaAtivoRepository TarefaAtivoRepository { get; }
        IComponenteDespesaRepository ComponenteDespesaRepository { get; }
        IComponenteOrcamentoRegistoRepository ComponenteOrcamentoRegistoRepository { get; }
        IComponenteOrcamentoValorRepository ComponenteOrcamentoValorRepository { get; }
        IComponenteDespesaRegistoRepository ComponenteDespesaRegistoRepository { get; }
        IComponenteTextoRegistoRepository ComponenteTextoRegistoRepository { get; }
        IPagamentosExecutadosRepository PagamentosExecutadosRepository { get; }
        IComponenteDocumentosRegistoRepository ComponenteDocumentosRegistoRepository { get; }
        IComponenteClassificacaoSubClassificRegistoRepository ComponenteClassificacaoSubClassificRegistoRepository { get; }
        IDestinatarioRepository DestinatarioRepository { get; }
        IComponenteConciliacaoMovimentosRepository ComponenteConciliacaoMovimentosRepository { get; }
        IMovimentosPorConciliarRepository MovimentosPorConciliarRepository { get; }
        IMovimentosbancariosRepository MovimentosbancariosRepository { get; }
        IComponenteReceitaRepository ComponenteReceitaRepository { get; }
        IRelMovimentosporconciliarMovimentosRepository RelMovimentosPorConciliarMovimentosRepository { get; }
        IComponenteReceitaRegistoRepository ComponenteReceitaRegistoRepository { get; }
        IUtilizadorTokenRepository UtilizadorTokenRepository { get; }
        ICompromissoRepository CompromissoRepository { get; }


        IKhachHangRepository KhachHangRepository { get; }

        void Commit();

        void Rollback();
    }
}