using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.ExcelDocumentService;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using TimorINSSBackEnd.Resources;
using static TimorINSSBackEnd.ExcelDocumentService.Models;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class AgrupamentoConfigRepository : IAgrupamentoConfigRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AgrupamentoConfigRepository(TimorINSSModuloContribuicoesContext storeContext, IStringLocalizer<SharedResource> localizer)
        {
            _moduloContribuicoesContext = storeContext;
            _localizer = localizer;
        }

        public IEnumerable<Agrupamentoconfig> GetAll()
        {
            return _moduloContribuicoesContext.Agrupamentoconfig.ToList();
        }

        public Agrupamentoconfig Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var agrupamentoconfig = _moduloContribuicoesContext.Agrupamentoconfig
                .SingleOrDefault(u => u.Id == id);

            return agrupamentoconfig;
        }

        public AgrupamentoConfigDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var agrupamentoconfig = _moduloContribuicoesContext.Agrupamentoconfig
                .SingleOrDefault(u => u.Id == id);

            AgrupamentoConfigDto agrupamentoconfigDto = Utils.MappClassToDto<Agrupamentoconfig, AgrupamentoConfigDto>(agrupamentoconfig);
            return agrupamentoconfigDto;
        }

        public void Add(Agrupamentoconfig entity)
        {
            _moduloContribuicoesContext.Agrupamentoconfig.Add(entity);
        }

        public void Update(Agrupamentoconfig entity)
        {
            Agrupamentoconfig entityToUpdate = _moduloContribuicoesContext.Agrupamentoconfig
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Agrupamentoconfig entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ValueCampoEditavelListagemResponse GetAllActiveAgrupamentoConfig(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Agrupamentoconfig> queryAgrupamentoConfigAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            if (int.TryParse(filter.filterField, out int parent))
                queryAgrupamentoConfigAux = _moduloContribuicoesContext.Agrupamentoconfig
                    .Where(a => a.IndActivo && a.Designacao.Contains(filter.filterBy) && a.ReltipoDeContaOrcamentoConfigFk == parent && a.ParentFk == null);
            else
                throw new Exception("No id was found for the entity parent");

            var queryAgrupamentoConfig = queryAgrupamentoConfigAux
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.Id,
                   Nome = u.Designacao,
                   ParentId = u.ReltipoDeContaOrcamentoConfigFk,
                   Parametros = new List<ParametrosAdicionais>()
                   {
                        new ParametrosAdicionais
                        {
                            Nome = "Codigo",
                            Size = "3",
                            Type = "number",
                            Valor = u.Codigo
                        }
                   }
               });

            var agrupamentos = queryAgrupamentoConfig
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryAgrupamentoConfig.Count();

            response.ValuesCampo = agrupamentos;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public ValueCampoEditavelListagemResponse GetAllActiveSubAgrupamentoConfig(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Agrupamentoconfig> queryAgrupamentoConfigAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            if (int.TryParse(filter.filterField, out int parent))
                queryAgrupamentoConfigAux = _moduloContribuicoesContext.Agrupamentoconfig
                    .Include(a => a.ParentFkNavigation)
                    .Where(a => a.IndActivo && a.Designacao.Contains(filter.filterBy) && a.ParentFk == parent);
            else
                throw new Exception("No id was found for the entity parent");

            var agrupamentos = queryAgrupamentoConfigAux
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            List<ValorCamposEditaveis> result = new List<ValorCamposEditaveis>();
            foreach (Agrupamentoconfig c in agrupamentos)
            {
                StringBuilder nome = new StringBuilder();
                result.Add(new ValorCamposEditaveis
                {
                    Id = c.Id,
                    Nome = c.Designacao,
                    ParentId = c.ParentFk,
                    Parametros = new List<ParametrosAdicionais>()
                    {
                        new ParametrosAdicionais
                        {
                            Nome = "CodigoTotal",
                            Size = "16",
                            Type = "number",
                            Valor = GetFullCodigo(c.Id),
                            NaoVisivel = true
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "Codigo",
                            Size = "3",
                            Type = "number",
                            Valor = c.Codigo
                        }
                    }
                });
            }

            response.ValuesCampo = result;
            response.CountValuesCampo = queryAgrupamentoConfigAux.Count();

            return response;
        }

        public List<SelectDescription> GetAllAgrupamentoConfigByParent(int parent)
        {
            int? parentOfParent = _moduloContribuicoesContext.Agrupamentoconfig
                .Where(u => u.Id == parent && u.IndActivo).Select(u => u.ReltipoDeContaOrcamentoConfigFk).FirstOrDefault();

            if (!parentOfParent.HasValue)
                throw new Exception("No id was found for the entity parent of parent");

            return _moduloContribuicoesContext.Agrupamentoconfig
                .Where(u => u.IndActivo && u.ParentFk == null && u.ReltipoDeContaOrcamentoConfigFk == parentOfParent)
                .Select(u => new SelectDescription
                {
                    id = u.Id,
                    nome = u.Codigo + " " + u.Designacao,
                    parentId = u.ReltipoDeContaOrcamentoConfigFk,
                    indActivo = u.IndActivo
                })
                .ToList();
        }

        public List<SelectDescription> GetAllSubAgrupamentoConfigByParent(int parent)
        {
            int? parentOfParent = _moduloContribuicoesContext.Agrupamentoconfig
                .Include(c => c.ParentFkNavigation)
                .Where(u => u.Id == parent && u.IndActivo).Select(u => u.ParentFk).FirstOrDefault();

            if (!parentOfParent.HasValue)
                throw new Exception("No id was found for the entity parent of parent");

            var objectList = _moduloContribuicoesContext.Agrupamentoconfig
                .Include(a => a.ParentFkNavigation)
                .Where(u => u.IndActivo && u.ParentFk == parentOfParent.Value)
                .ToList();

            List<SelectDescription> result = new List<SelectDescription>();
            foreach (Agrupamentoconfig a in objectList)
            {
                StringBuilder nome = new StringBuilder();
                nome.Append(GetFullCodigo(a.Id));
                nome.Append(" ");
                nome.Append(a.Designacao);
                result.Add(
                    new SelectDescription
                    {
                        id = a.Id,
                        nome = nome.ToString(),
                        parentId = a.ParentFk.Value,
                        indActivo = a.IndActivo
                    });
            }

            return result;
        }

        public List<MultipleSelectAdditionalParameter> GetAllAgrupamentosMultipleSelect(int orcamentoConfigId)
        {
            Dictionary<int, Agrupamentoconfig> objectDictionary = _moduloContribuicoesContext.Agrupamentoconfig
            .Where(a => a.IndActivo)
            .Include(a => a.ParentFkNavigation)
            .Include(a => a.ReltipoDeContaOrcamentoConfigFkNavigation)
                .ThenInclude(r => r.TipoContaFkNavigation)
            .Where(a => a.ReltipoDeContaOrcamentoConfigFkNavigation.OrcamentoConfigFk == orcamentoConfigId)
            .ToDictionary(a => a.Id);

            List<MultipleSelectAdditionalParameter> result = new List<MultipleSelectAdditionalParameter>();
            Agrupamentoconfig a;
            foreach (int key in objectDictionary.Keys)
            {
                a = objectDictionary[key];
                StringBuilder nome = new StringBuilder();
                nome.Append(GetFullCodigoTransactionless(a.Id, objectDictionary));
                nome.Append(" - ");
                nome.Append(a.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFkNavigation.Descricao);
                nome.Append(" - ");
                nome.Append(a.Designacao);
                result.Add(
                    new MultipleSelectAdditionalParameter
                    {
                        Id = a.Id,
                        Nome = nome.ToString()
                    });
            }

            return result;
        }

        public bool IsCodeValid(Agrupamentoconfig agrupamento)

        {
            int countSameCode = 0;
            if (agrupamento.ParentFk != null) {
                 countSameCode = _moduloContribuicoesContext.Agrupamentoconfig
                     .Where(a => a.ParentFk == agrupamento.ParentFk && a.Codigo == agrupamento.Codigo && a.Id != agrupamento.Id && a.IndActivo)
                     .Count();
            }
        
            

            return countSameCode == 0;
        }

        public List<AgrupamentoConfigDataContract> GetAlllActivAgrupamentoConfigByOrcamentoConfig(int orcamentoId)
        {
            Dictionary<int, Agrupamentoconfig> objectDictionary = _moduloContribuicoesContext.Agrupamentoconfig
            .Where(a => a.IndActivo)
            .Include(a => a.ParentFkNavigation)
            .Include(a => a.ReltipoDeContaOrcamentoConfigFkNavigation)
            .Include(a => a.InverseParentFkNavigation)
            .Where(a => a.ReltipoDeContaOrcamentoConfigFkNavigation.OrcamentoConfigFk == orcamentoId)
            .ToDictionary(a => a.Id);

            List<AgrupamentoConfigDataContract> result = new List<AgrupamentoConfigDataContract>();
            Agrupamentoconfig a;
            foreach (int key in objectDictionary.Keys)
            {
                a = objectDictionary[key];
                StringBuilder nome = new StringBuilder();
                nome.Append(GetFullCodigoTransactionless(a.Id, objectDictionary));
                nome.Append(" - ");
                nome.Append(a.Designacao);
                result.Add(
                    new AgrupamentoConfigDataContract
                    {
                        Id = a.Id,
                        Designacao = nome.ToString(),
                        ParentFk = a.ParentFk,
                        TipoDeConta = a.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk,
                        Final = a.InverseParentFkNavigation.Count == 0
                    });
            }

            return result;
        }

        public List<AgrupamentoConfigDataContract> GetActidadesAgrupamentoConfigByOrcamentoConfig(int orcamentoId, int tipoContaId)
        {
            Dictionary<int, Agrupamentoconfig> objectDictionary = _moduloContribuicoesContext.Agrupamentoconfig
            .Where(a => a.IndActivo)
            .Include(a => a.ParentFkNavigation)
            .Include(a => a.ReltipoDeContaOrcamentoConfigFkNavigation)
            .Include(a => a.InverseParentFkNavigation)
            .Where(a => a.ReltipoDeContaOrcamentoConfigFkNavigation.OrcamentoConfigFk == orcamentoId
            && a.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk == tipoContaId)
            .ToDictionary(a => a.Id);

            List<AgrupamentoConfigDataContract> result = new List<AgrupamentoConfigDataContract>();
            Agrupamentoconfig a;
            foreach (int key in objectDictionary.Keys)
            {
                a = objectDictionary[key];
                StringBuilder nome = new StringBuilder();
                nome.Append(GetFullCodigoTransactionless(a.Id, objectDictionary));
                nome.Append(" - ");
                nome.Append(a.Designacao);
                result.Add(
                    new AgrupamentoConfigDataContract
                    {
                        Id = a.Id,
                        Designacao = nome.ToString(),
                        ParentFk = a.ParentFk,
                        TipoDeConta = a.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk,
                        Final = a.InverseParentFkNavigation.Count == 0
                    });
            }

            return result;
        }


        public List<AgrupamentoConfigDataContract> GetAlllActivAgrupamentoConfigByOrcamentoConfigTipoConta(int? orcamentoId, int tipoContaFK)
        {
            Dictionary<int, Agrupamentoconfig> objectDictionary = _moduloContribuicoesContext.Agrupamentoconfig
            .Where(a => a.IndActivo)
            .Include(a => a.ParentFkNavigation)
            .Include(a => a.ReltipoDeContaOrcamentoConfigFkNavigation)
            .Include(a => a.InverseParentFkNavigation)
            //.Where(a => a.ReltipoDeContaOrcamentoConfigFkNavigation.OrcamentoConfigFk == orcamentoId && a.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk == tipoContaFK)
            .ToDictionary(a => a.Id);

            if (orcamentoId != null && orcamentoId > 0)
            {
            objectDictionary = _moduloContribuicoesContext.Agrupamentoconfig
                .Where(a => a.IndActivo)
                .Include(a => a.ParentFkNavigation)
                .Include(a => a.ReltipoDeContaOrcamentoConfigFkNavigation)
                .Include(a => a.InverseParentFkNavigation)
                .Where(a => a.ReltipoDeContaOrcamentoConfigFkNavigation.OrcamentoConfigFk == orcamentoId && a.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk == tipoContaFK)
                .ToDictionary(a => a.Id);
            }

            List<AgrupamentoConfigDataContract> result = new List<AgrupamentoConfigDataContract>();
            Agrupamentoconfig a;
            foreach (int key in objectDictionary.Keys)
            {
                a = objectDictionary[key];
                StringBuilder nome = new StringBuilder();
                nome.Append(GetFullCodigoTransactionless(a.Id, objectDictionary));
                nome.Append(" - ");
                nome.Append(a.Designacao);
                result.Add(
                    new AgrupamentoConfigDataContract
                    {
                        Id = a.Id,
                        Designacao = nome.ToString(),
                        ParentFk = a.ParentFk,
                        TipoDeConta = a.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk,
                        Final = a.InverseParentFkNavigation.Count == 0
                    });
            }

            return result;
        }

        public string GetFullCodigo(int id)
        {
            Agrupamentoconfig agrupamento = _moduloContribuicoesContext.Agrupamentoconfig
                        .Include(a => a.ParentFkNavigation)
                        .Where(a => a.Id == id).FirstOrDefault();
            if (agrupamento != null)
            {
                StringBuilder nome = new StringBuilder();
                nome.Append(agrupamento.Codigo);

                // Aqui cada volta faz uma consulta a base de dados, por isso um ciclo nao ocupa
                // so um core: martela tambem o SQL Server. Ver a nota em GetFullCodigoTransactionless.
                HashSet<int> visitados = new HashSet<int> { agrupamento.Id };
                while (agrupamento.ParentFkNavigation != null && agrupamento.ParentFk.HasValue)
                {
                    if (JaVisitado(visitados, agrupamento.ParentFk.Value))
                        break;

                    nome.Insert(0, agrupamento.ParentFkNavigation.Codigo);
                    agrupamento = _moduloContribuicoesContext.Agrupamentoconfig
                        .Include(a => a.ParentFkNavigation)
                        .Where(a => a.Id == agrupamento.ParentFk).FirstOrDefault();

                    if (agrupamento == null)
                        break;
                }
                return nome.ToString();
            }
            else
                return "";
        }

        public string GetFullDesignacao(int id)
        {
            Agrupamentoconfig agrupamento = _moduloContribuicoesContext.Agrupamentoconfig
                        .Include(a => a.ParentFkNavigation)
                        .Where(a => a.Id == id).FirstOrDefault();
            if (agrupamento != null)
            {
                StringBuilder nome = new StringBuilder();
                nome.Append(agrupamento.Codigo);
                nome.Append("-");
                nome.Append(agrupamento.Designacao);
                nome.Append(" ");

                // Mesma protecção: sem ela um ciclo na arvore consome um core e consulta a base
                // de dados em ciclo. Ver a nota em GetFullCodigoTransactionless.
                HashSet<int> visitados = new HashSet<int> { agrupamento.Id };
                while (agrupamento.ParentFkNavigation != null && agrupamento.ParentFk.HasValue)
                {
                    if (JaVisitado(visitados, agrupamento.ParentFk.Value))
                        break;

                    nome.Insert(0, agrupamento.ParentFkNavigation.Codigo + "-" + agrupamento.ParentFkNavigation.Designacao + " ");
                    agrupamento = _moduloContribuicoesContext.Agrupamentoconfig
                        .Include(a => a.ParentFkNavigation)
                        .Where(a => a.Id == agrupamento.ParentFk).FirstOrDefault();

                    if (agrupamento == null)
                        break;
                }
                return nome.ToString();
            }
            else
                return "";
        }

        // [PT] Um agrupamento que seja pai de si proprio -- ou dois que sejam pai um do outro --
        // punha os tres metodos abaixo a subir a arvore para sempre. O ciclo nao rebenta: cada
        // volta acrescenta ao StringBuilder e o pedido nunca responde, deixando um core do
        // servidor a 100% ate a aplicacao ser reiniciada. Dois pedidos assim ocupavam dois cores.
        // Observado a 2026-08-25 com a linha AGRUPAMENTOCONFIG id=715, parent_fk=715.
        //
        // Guardar os ids ja visitados e o que trava isto. Ao encontrar um repetido para-se e
        // devolve-se o que ja foi construido: a linha com dados maus fica com um codigo curto,
        // visivel, e o resto do ecra continua a funcionar -- ao contrario de lancar excepcao,
        // que mata a listagem inteira por causa de uma linha.
        //
        // [VI] Mot agrupamento tu lam cha chinh no -- hoac hai cai lam cha lan nhau -- khien ba
        // ham duoi day leo cay len mai khong dung. Vong lap khong he bao loi: moi vong lai noi
        // them vao StringBuilder va request khong bao gio tra ve, giu nguyen mot loi CPU o 100%
        // cho den khi restart ung dung. Hai request nhu vay chiem hai loi.
        // Ghi nhan 25/08/2026 voi dong AGRUPAMENTOCONFIG id=715, parent_fk=715.
        //
        // Cach chan la nho lai cac id da di qua. Gap id lap lai thi dung va tra ve phan da dung
        // duoc: dong du lieu hong se co ma ngan, nhin ra ngay, con phan con lai cua man hinh van
        // chay -- khac voi nem exception, mot dong hong se giet ca danh sach.
        private static bool JaVisitado(HashSet<int> visitados, int id)
        {
            return !visitados.Add(id);
        }

        public string GetFullCodigoTransactionless(int id, Dictionary<int, Agrupamentoconfig> agrupamentos)
        {
            // TryGetValue em vez de indexador: um pai que nao esteja no dicionario lançava
            // KeyNotFoundException e derrubava a listagem toda.
            if (!agrupamentos.TryGetValue(id, out Agrupamentoconfig agrupamento) || agrupamento == null)
                return "";

            StringBuilder nome = new StringBuilder();
            nome.Append(agrupamento.Codigo);

            HashSet<int> visitados = new HashSet<int> { agrupamento.Id };
            while (agrupamento.ParentFkNavigation != null && agrupamento.ParentFk.HasValue)
            {
                if (JaVisitado(visitados, agrupamento.ParentFk.Value))
                    break;

                nome.Insert(0, agrupamento.ParentFkNavigation.Codigo);

                if (!agrupamentos.TryGetValue(agrupamento.ParentFk.Value, out agrupamento) || agrupamento == null)
                    break;
            }

            return nome.ToString();
        }

        // [PT] ATENCAO: este metodo devolve sempre true. O ciclo desce a arvore a recolher
        // descendentes e deita fora o resultado, portanto a validacao que o nome promete nao
        // existe -- apagar e sempre permitido, mesmo com filhos. Nao se alterou esse
        // comportamento aqui: mudar quando um agrupamento pode ser apagado e uma decisao de
        // negocio, nao uma correccao. Fica registado no backlog.
        // O que se corrigiu foi o ciclo poder nao terminar: com uma linha que seja pai de si
        // propria, `Contains(a.ParentFk.Value)` volta a encontra-la a cada volta e o metodo
        // consulta a base de dados indefinidamente.
        //
        // [VI] LUU Y: ham nay luon tra ve true. Vong lap di xuong cay gom cac node con roi vut
        // ket qua di, nen phep kiem tra ma ten ham hua hen thuc te khong ton tai -- van xoa duoc
        // du con node con. Khong doi hanh vi do o day: quyet dinh khi nao duoc xoa mot agrupamento
        // la quyet dinh nghiep vu, khong phai sua loi. Da ghi vao backlog.
        // Thu da sua la vong lap co the khong bao gio dung: voi dong tu lam cha chinh no,
        // `Contains(a.ParentFk.Value)` lai tim thay no o moi vong va ham truy van database mai.
        public bool IsAgrupamentoDeleteValid(Agrupamentoconfig agrupamento)
        {
            List<int> agrupamentosIte = new List<int> { agrupamento.Id };
            HashSet<int> visitados = new HashSet<int> { agrupamento.Id };

            while (agrupamentosIte.Count > 0)
            {
                agrupamentosIte = _moduloContribuicoesContext.Agrupamentoconfig
                    .Where(a => a.IndActivo && a.ParentFk.HasValue && agrupamentosIte.Contains(a.ParentFk.Value))
                    .Select(a => a.Id).ToList();

                // So se desce para ids ainda nao vistos; um ciclo para aqui em vez de repetir.
                agrupamentosIte = agrupamentosIte.Where(id => visitados.Add(id)).ToList();
            }

            return true;
        }

        public bool AgrupamentoHasChilds(Agrupamentoconfig agrupamento)
        {
            var countFilhos = _moduloContribuicoesContext.Agrupamentoconfig
                .Where(c => c.IndActivo && c.ParentFk == agrupamento.Id).Count();

            return countFilhos > 0;
        }

        public string ExecucaoOrcamentalExcel(RelatorioExecucaoOrcamentalListagemRequest request, List<ExecucaoOrcamentalDataContract> lista)
        {
            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["execucaoOrcamental"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            var tipoContaText = _moduloContribuicoesContext.Dominio.FirstOrDefault(e => e.IndActivo && e.IdDominio == request.tipoConta).Descricao ?? "";

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["relatorioExecucaoOrcamental"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(21, 1));
            excelDocument.Pages[0].AddText(_localizer["ano"].Value + " " + request.year, new ExcelDocumentTextPosition(1, 2), "Header", new ExcelDocumentTextPosition(21, 2));
            excelDocument.Pages[0].AddText(tipoContaText, new ExcelDocumentTextPosition(1, 3), "Header", new ExcelDocumentTextPosition(21, 3));

            // Adição das colunas na tabela (A tabela tem vários níveis de cabeçalho)
            excelDocument.Pages[0].AddText(_localizer["valorOrcamentadoTotalAno"].Value, new ExcelDocumentTextPosition(5, 5), "HeaderWrap", new ExcelDocumentTextPosition(5, 9));
            excelDocument.Pages[0].AddText(_localizer["execucaoAnoAnterior"].Value, new ExcelDocumentTextPosition(6, 5), "HeaderWrap", new ExcelDocumentTextPosition(6, 7));
            excelDocument.Pages[0].AddText(_localizer["janeiroDezembro"].Value, new ExcelDocumentTextPosition(6, 8), "HeaderWrap", new ExcelDocumentTextPosition(6, 9));
            excelDocument.Pages[0].AddText(_localizer["janeiro"].Value, new ExcelDocumentTextPosition(7, 5), "HeaderWrap", new ExcelDocumentTextPosition(7, 9));
            excelDocument.Pages[0].AddText(_localizer["fevereiro"].Value, new ExcelDocumentTextPosition(8, 5), "HeaderWrap", new ExcelDocumentTextPosition(8, 9));
            excelDocument.Pages[0].AddText(_localizer["marco"].Value, new ExcelDocumentTextPosition(9, 5), "HeaderWrap", new ExcelDocumentTextPosition(9, 9));
            excelDocument.Pages[0].AddText(_localizer["abril"].Value, new ExcelDocumentTextPosition(10, 5), "HeaderWrap", new ExcelDocumentTextPosition(10, 9));
            excelDocument.Pages[0].AddText(_localizer["maio"].Value, new ExcelDocumentTextPosition(11, 5), "HeaderWrap", new ExcelDocumentTextPosition(11, 9));
            excelDocument.Pages[0].AddText(_localizer["junho"].Value, new ExcelDocumentTextPosition(12, 5), "HeaderWrap", new ExcelDocumentTextPosition(12, 9));
            excelDocument.Pages[0].AddText(_localizer["julho"].Value, new ExcelDocumentTextPosition(13, 5), "HeaderWrap", new ExcelDocumentTextPosition(13, 9));
            excelDocument.Pages[0].AddText(_localizer["agosto"].Value, new ExcelDocumentTextPosition(14, 5), "HeaderWrap", new ExcelDocumentTextPosition(14, 9));
            excelDocument.Pages[0].AddText(_localizer["setembro"].Value, new ExcelDocumentTextPosition(15, 5), "HeaderWrap", new ExcelDocumentTextPosition(15, 9));
            excelDocument.Pages[0].AddText(_localizer["outubro"].Value, new ExcelDocumentTextPosition(16, 5), "HeaderWrap", new ExcelDocumentTextPosition(16, 9));
            excelDocument.Pages[0].AddText(_localizer["novembro"].Value, new ExcelDocumentTextPosition(17, 5), "HeaderWrap", new ExcelDocumentTextPosition(17, 9));
            excelDocument.Pages[0].AddText(_localizer["dezembro"].Value, new ExcelDocumentTextPosition(18, 5), "HeaderWrap", new ExcelDocumentTextPosition(18, 9));
            excelDocument.Pages[0].AddText(_localizer["totalExecucao"].Value, new ExcelDocumentTextPosition(19, 5), "HeaderWrap", new ExcelDocumentTextPosition(19, 9));
            excelDocument.Pages[0].AddText(_localizer["taxaExecucao"].Value + " (%)", new ExcelDocumentTextPosition(20, 5), "HeaderWrap", new ExcelDocumentTextPosition(20, 7));
            excelDocument.Pages[0].AddText(_localizer["janeiroJaneiro"].Value, new ExcelDocumentTextPosition(20, 8), "HeaderWrap", new ExcelDocumentTextPosition(20, 9));
            excelDocument.Pages[0].AddText(_localizer["variacaoExecucaoPeriodoHomologo"].Value + " (%)", new ExcelDocumentTextPosition(21, 5), "HeaderWrap", new ExcelDocumentTextPosition(21, 9));

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 5), lista, new List<ColumnOption<ExecucaoOrcamentalDataContract>>()
            {
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = _localizer["instiutiton"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.instiutiton,
                    RowSpan = 5,
                    Width = 14
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = _localizer["contaOGE"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.contaOGE,
                    RowSpan = 5,
                    Width = 14
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = _localizer["centroCustos"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => string.Join(", ", data.centrosCusto),
                    RowSpan = 5,
                    Width = 20
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = _localizer["rubricaClassificacaoEconomica"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => string.Join(", ", data.rubricas),
                    RowSpan = 5,
                    Width = 20
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = _localizer["valorOrcamentoInicial"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.valorOrcamentoInicial,
                    RowSpan = 5,
                    Width = 16
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(1)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.valorOrcamentado,
                    Width = 16
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(2)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.valorAnoAnterior,
                    Width = 16
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.janeiro,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.fevereiro,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.marco,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.abril,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.maio,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.junho,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.julho,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.agosto,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.setembro,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.outubro,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.novembro,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.dezembro,
                    Width = 13
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(4) = ∑(3)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.totalExecucao,
                    Width = 15
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(5) = (4)/(1)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.taxaExecucao + "%",
                    Width = 20
                },
                new ColumnOption<ExecucaoOrcamentalDataContract>()
                {
                    Name = "(6) = (4-2)/(2)",
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.variacaoExecucao + "%",
                    Width = 20
                }
            }, new TableOptions() { AutoFitColumns = false });

            // Conversão do documento excel em base64
            return excelDocument.GetFileString();
        }

        public string ExecucaoOrcamentalPorClassificacaoEconomicaExcel(RelatorioClassificacaoEconomicaRequest request, List<ClassificacaoEconomicaExecucaoDataContract> listaReceitas, List<ClassificacaoEconomicaExecucaoDataContract> listaDespesas)
        {
            // Inicialização do documento excel, um separador (page) para Receitas e outro para Despesas
            var excelDocument = new ExcelDocument(new List<string>() { _localizer["receitas"].Value, _localizer["despesas"].Value }, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            void AddClassificacaoEconomicaPage(int pageIndex, List<ClassificacaoEconomicaExecucaoDataContract> lista, bool isDespesa)
            {
                var page = excelDocument.Pages[pageIndex];

                // Adição dos títulos
                page.AddText(_localizer["relatorioClassificacaoEconomica"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(17, 1));
                page.AddText(_localizer["ano"].Value + " " + request.year, new ExcelDocumentTextPosition(1, 2), "Header", new ExcelDocumentTextPosition(17, 2));

                var colunas = new List<ColumnOption<ClassificacaoEconomicaExecucaoDataContract>>()
                {
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["codigo"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellWrap",
                        Value = (data) => data.codigoCE,
                        Width = 14
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["designacao"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellWrap",
                        Value = (data) => data.designacaoCE,
                        Width = 40
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["ossInicial"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.valorOrcamentoInicial,
                        Width = 15
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["ossCorrigido"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.valorOrcamentado,
                        Width = 15
                    },
                };

                if (isDespesa)
                {
                    colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["cabimentosCE"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.cabimentos, Width = 15 });
                    colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["compromissosCE"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.compromissos, Width = 15 });
                    colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["obrigacoes"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.obrigacoes, Width = 15 });
                }
                else
                {
                    colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["receitaLiquidada"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.receitaLiquidada, Width = 15 });
                }

                colunas.AddRange(new List<ColumnOption<ClassificacaoEconomicaExecucaoDataContract>>()
                {
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["janeiro"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.janeiro,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["fevereiro"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.fevereiro,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["marco"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.marco,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["abril"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.abril,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["maio"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.maio,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["junho"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.junho,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["julho"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.julho,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["agosto"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.agosto,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["setembro"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.setembro,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["outubro"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.outubro,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["novembro"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.novembro,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["dezembro"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.dezembro,
                        Width = 13
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["totalExecucao"].Value,
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCellMoney",
                        Value = (data) => data.totalExecucao,
                        Width = 15
                    },
                    new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>()
                    {
                        Name = _localizer["taxaExecucao"].Value + " (%)",
                        ColumnTextStyleKey = "HeaderWrap",
                        DataTextStyleKey = "TableCell",
                        // [EN] Round to 2 decimals like the on-screen report (element.taxaExecucao.toFixed(2))
                        // instead of concatenating the raw fraction, which Excel displayed as a long ugly decimal.
                        // [VI] Làm tròn 2 số thập phân giống báo cáo trên màn hình, thay vì nối chuỗi số thô
                        // (Excel hiển thị ra số thập phân dài, xấu).
                        Value = (data) => data.taxaExecucao.ToString("F2") + "%",
                        Width = 15
                    },
                });

                colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["saldoExecucaoCE"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.saldoExecucao, Width = 15 });

                if (isDespesa)
                {
                    colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["saldoDisponivel"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.saldoDisponivel, Width = 18 });
                    colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["saldoNaoComprometido"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.saldoNaoComprometido, Width = 18 });
                    colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["valorCabimentadoNaoComprometido"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.valorCabimentadoNaoComprometido, Width = 18 });
                    colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["valorComprometidoNaoLiquidado"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.valorComprometidoNaoLiquidado, Width = 18 });
                    colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["valorLiquidadoNaoPago"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.valorLiquidadoNaoPago, Width = 18 });
                }
                else
                {
                    colunas.Add(new ColumnOption<ClassificacaoEconomicaExecucaoDataContract>() { Name = _localizer["saldoReceitaLiquidadaNaoCobrada"].Value, ColumnTextStyleKey = "HeaderWrap", DataTextStyleKey = "TableCellMoney", Value = (data) => data.saldoReceitaLiquidadaNaoCobrada, Width = 18 });
                }

                // Adição da tabela
                page.AddTable(new ExcelDocumentTextPosition(1, 4), lista, colunas, new TableOptions() { AutoFitColumns = false });
            }

            AddClassificacaoEconomicaPage(0, listaReceitas, false);
            AddClassificacaoEconomicaPage(1, listaDespesas, true);

            // Conversão do documento excel em base64
            return excelDocument.GetFileString();
        }
    }
}