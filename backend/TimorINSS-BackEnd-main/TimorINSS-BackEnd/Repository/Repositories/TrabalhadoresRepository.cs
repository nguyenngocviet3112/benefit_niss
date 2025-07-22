using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class TrabalhadoresRepository : ITrabalhadoresRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TrabalhadoresRepository(TimorINSSModuloContribuicoesContext storeContext,
                                    IHttpContextAccessor httpContextAccessor)
        {
            _moduloContribuicoesContext = storeContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<Trabalhador> GetAll()
        {
            return _moduloContribuicoesContext.Trabalhador
                .ToList();
        }

        public Trabalhador Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var trabalhador = _moduloContribuicoesContext.Trabalhador
                .SingleOrDefault(u => u.IdTrabalhador == id);

            return trabalhador;
        }

        public TrabalhadorDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var trabalhador = _moduloContribuicoesContext.Trabalhador
                .SingleOrDefault(u => u.IdTrabalhador == id);

            TrabalhadorDto trabalhadorDto = Utils.MappClassToDto<Trabalhador, TrabalhadorDto>(trabalhador);
            return trabalhadorDto;
        }

        public void Add(Trabalhador entity)
        {
            _moduloContribuicoesContext.Trabalhador.Add(entity);
        }


        public void Create(EntidadeEmpregadoraUpsertRequest entity)
        {
            // Khởi tạo đối tượng Trabalhador
            Trabalhador newEntity = new Trabalhador()
            {
                Nome = entity.EntidadeEmpregadora.Nome,
                NomeMae = entity.EntidadeEmpregadora.Nome,
                NomePai = entity.EntidadeEmpregadora.Nome,
                Tin = entity.EntidadeEmpregadora.Tin,
                Niss = entity.EntidadeEmpregadora.Niss,
                UtilizadorCriacao = entity.UserId,
                NumInscProvisoria = entity.EntidadeEmpregadora.Tin,
                FlagImportado = false,
                DataCriacao = DateTime.Now,
                DataAlteracao = DateTime.Now,
                DataNasc = DateTime.Now,
                Ipv6 = "0.0.0.0", // Bạn có thể điền giá trị hợp lệ nếu cần
                SexoTrabalhador = 10, // Mặc định giá trị giới tính, có thể cần điều chỉnh theo dữ liệu
                NacionalidadeTrabalhador = 11, // Mặc định quốc tịch, có thể điều chỉnh
                Interno = true, // Mặc định là false
             
                IndDescNomeMae = false, // Mặc định không ẩn tên mẹ
   
                IndDescNomePai = false, // Mặc định không ẩn tên cha
                EstadoCivil = null, // Nếu không có dữ liệu trạng thái hôn nhân
                Naturalidade = "Dili", // Mặc định giá trị cho nơi sinh

                Contacto = new List<Contacto>()
                {
                    new Contacto()
                    {
                        Email = entity.EntidadeEmpregadora.Email,
                        Telemovel = entity.EntidadeEmpregadora.Telemovel,
                        UtilizadorCriacao = entity.UserId,
                        DataCriacao = DateTime.Now,
                    }
                }
            };

            _moduloContribuicoesContext.Trabalhador.Add(newEntity);
        }


        public void Update(Trabalhador entity)
        {
            Trabalhador entityToUpdate = _moduloContribuicoesContext.Trabalhador
                .Single(d => d.IdTrabalhador == entity.IdTrabalhador);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Trabalhador entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public TrabalhadorListagemResponse GetTrabalhadoresByIdEntidadeEmpregadora(TrabalhadorListagemRequest request)
        {
            DateTime begin = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            DateTime end = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
            if (request.filter.dateFilterBegin.HasValue)
            {
                var beginFullDate = request.filter.dateFilterBegin.Value;
                begin = new DateTime(beginFullDate.Year, beginFullDate.Month, 1);
                end = beginFullDate.AddMonths(1).AddDays(-1);
            }

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var queryTrabalhadores = _moduloContribuicoesContext.Relentidadetrabalhador
                .Include(u => u.RegimeFkNavigation)
                .Where(u => u.DtIniVincTrabalhador <= end &&
                            begin <= (u.DtIniFimTrabalhador ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                            && u.EntidadeFk == request.id)
                .Join(
                    _moduloContribuicoesContext.Trabalhador
                    .Where(u => u.Nome.Contains(request.filter.filterBy) || u.Niss.Contains(request.filter.filterBy)),
                    relEntidadeTrabalhador => relEntidadeTrabalhador.TrabalhadorFk,
                    trabalhador => trabalhador.IdTrabalhador,
                    (relEntidadeTrabalhador, trabalhador) => new TrabalhadorListagem
                    {
                        id = trabalhador.IdTrabalhador,
                        nome = trabalhador.Nome,
                        regime = relEntidadeTrabalhador.RegimeFkNavigation.Descricao,
                        incricaoINSS = trabalhador.Niss,
                        dtInicioDeVinculo = relEntidadeTrabalhador.DtIniVincTrabalhador,
                        dtFimDeVinculo = relEntidadeTrabalhador.DtIniFimTrabalhador,
                        idRel = relEntidadeTrabalhador.IdRel,
                    }
                );

            var trabalhadores = queryTrabalhadores
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryTrabalhadores.Count();

            TrabalhadorListagemResponse result = new TrabalhadorListagemResponse
            {
                trabalhadores = trabalhadores,
                rows = totalNumber
            };
            return result;
        }

        public VincularTrabalhadorListagemResponse getTrabalhadoresByFilter(SearchFilterRequest request)
        {
            VincularTrabalhadorListagemResponse response = new VincularTrabalhadorListagemResponse();

            var filter = request.filter;
            if (filter == null || string.IsNullOrWhiteSpace(filter.filterBy))
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                });

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            IQueryable<Trabalhador> query = null;

            switch (filter.filterField)
            {
                case "Nome":
                    query = _moduloContribuicoesContext.Trabalhador.Where(u => u.Nome.Contains(filter.filterBy));
                    break;

                case "NISS":
                    query = _moduloContribuicoesContext.Trabalhador.Where(u => string.Compare(u.Niss.Trim(), filter.filterBy.Trim()) == 0);
                    break;

                case "NISS Provisório":
                    query = _moduloContribuicoesContext.Trabalhador.Where(u => string.Compare(u.NumInscProvisoria, filter.filterBy.Trim()) == 0);
                    break;

                case "Documento Identificação":
                    if (filter.filter != null)
                    {
                        int tipoDoc = 0;
                        if (int.TryParse(filter.filter.filterField, out tipoDoc))
                        {
                            query = _moduloContribuicoesContext.Documentoidentificacao.Where(u => u.IndActivo)
                                .Where(u => string.Compare(filter.filterBy, u.Numero) == 0
                                            && tipoDoc == u.TpDocIdentificacao)
                                .Join(
                                    _moduloContribuicoesContext.Trabalhador,
                                    documento => documento.TrabalhadorDocumetoFk,
                                    trabalhador => trabalhador.IdTrabalhador,
                                    (relEntidadeTrabalhador, trabalhador) => trabalhador);
                        }
                        else
                            response.Errors.Add(new Error
                            {
                                ErrorCode = ((int)ErrorsDataContract.InvalidFilter).ToString(),
                                ErrorMessage = ErrorsDataContract.InvalidFilter.ToString()
                            });
                    }
                    else
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.InvalidFilter).ToString(),
                            ErrorMessage = ErrorsDataContract.InvalidFilter.ToString()
                        });
                    break;

                default:
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.InvalidFilter).ToString(),
                        ErrorMessage = ErrorsDataContract.InvalidFilter.ToString()
                    });
                    break;
            }

            var trabalhadores = new List<VincularTrabalhadorListagem>();
            var totalNumber = 0;
            if (query != null)
            {
                totalNumber = query.Count();
                trabalhadores = query
                    .Select(e => new VincularTrabalhadorListagem
                    {
                        id = e.IdTrabalhador,
                        nome = e.Nome,
                        niss = e.Niss,
                        nissProvisorio = e.NumInscProvisoria,
                    })
                    .OrderBy(filter.orderBy, filter.orderDirection)
                    .Skip(index * rows)
                    .Take(rows)
                    .ToList();
            }

            response = new VincularTrabalhadorListagemResponse
            {
                trabalhadores = trabalhadores,
                rows = totalNumber
            };

            return response;
        }

        private TrabalhadorDto setDetailsToTrabalhadorDto(TrabalhadorDto trabalhador)
        {
            trabalhador.Ipv6 = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            return trabalhador;
        }

        public Trabalhador GetByNiss(string niss)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var trabalhador = _moduloContribuicoesContext.Trabalhador
                .FirstOrDefault(u => u.Niss == niss);

            return trabalhador;
        }

        public bool NissExists(string niss, int idTrabalhador = 0)
        {
            if (niss == null)
            {
                return false;
            }
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            
            return _moduloContribuicoesContext.Trabalhador
                .Any(u => u.Niss == niss && u.IdTrabalhador != idTrabalhador);
        }

        public bool TinExists(string tin, int idTrabalhador = 0)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.Trabalhador
                .Any(u => u.Tin == tin && u.IdTrabalhador != idTrabalhador);
        }

        public TrabalhadorListagemResponse GetTrabalhadoresByNiss(TrabalhadorListagemRequest request)
        {
            TrabalhadorListagemResponse response = new TrabalhadorListagemResponse();
            try
            {
                var filter = request.filter;
                if (filter == null)
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                        ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                    });

                int index = 0;
                if (filter.index.HasValue)
                    index = filter.index.Value;

                int rows = 5;
                if (filter.rows.HasValue)
                    rows = filter.rows.Value;

                string orderBy = "";
                OrderDirectionEnum orderDirection = OrderDirectionEnum.descending;
                if (filter.orderDirection.HasValue)
                {
                    orderBy = filter.orderBy;
                    orderDirection = (OrderDirectionEnum)filter.orderDirection;
                }

                var queryTrabalhadores = _moduloContribuicoesContext.Relentidadetrabalhador
                     .Include(u => u.RegimeFkNavigation)
                     .Where(u => u.EntidadeFk == request.id)
                     .Join(
                         _moduloContribuicoesContext.Trabalhador
                         .Where(u => string.Compare(u.Niss.Trim(), filter.filterBy.Trim()) == 0),
                         relEntidadeTrabalhador => relEntidadeTrabalhador.TrabalhadorFk,
                         trabalhador => trabalhador.IdTrabalhador,
                         (relEntidadeTrabalhador, trabalhador) => new TrabalhadorListagem
                         {
                             id = trabalhador.IdTrabalhador,
                             nome = trabalhador.Nome,
                             regime = relEntidadeTrabalhador.RegimeFkNavigation.Descricao,
                             incricaoINSS = trabalhador.Niss,
                             dtInicioDeVinculo = relEntidadeTrabalhador.DtIniVincTrabalhador,
                             dtFimDeVinculo = relEntidadeTrabalhador.DtIniFimTrabalhador,
                             idRel = relEntidadeTrabalhador.IdRel,
                         }
                     );

                var trabalhadores = queryTrabalhadores
                    .OrderBy(orderBy, orderDirection)
                    .Skip(index * rows)
                    .Take(rows)
                    .ToList();

                var totalNumber = queryTrabalhadores.Count();

                response = new TrabalhadorListagemResponse
                {
                    trabalhadores = trabalhadores,
                    rows = totalNumber
                };
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public Trabalhador GetTrabalhadorViewById(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var trabalhador = _moduloContribuicoesContext.Trabalhador
                .Include(t => t.Contacto.Where(c => c.IndActivo))
                .Include(t => t.Morada)
                .Include(t => t.Documentoidentificacao.Where(d => d.IndActivo))
                .Include(t => t.Inssestrangeiro.Where(i => i.IndActivo))
                .SingleOrDefault(t => t.IdTrabalhador == id);
            return trabalhador;
        }

        public string GetNextNumInscProvisoria()
        {
            long numInsc = 0;
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            Trabalhador trabalhador = _moduloContribuicoesContext.Trabalhador
                .Where(t => !string.IsNullOrWhiteSpace(t.NumInscProvisoria))
                .OrderByDescending(o => o.IdTrabalhador)
                .FirstOrDefault();

            if (trabalhador == null)
            {
                numInsc = 111111111;
            }
            else
            {
                numInsc = long.Parse(trabalhador.NumInscProvisoria) + 1;
            }
            return numInsc.ToString();
        }

        public string GetNextNISS()
        {
            long numINSS = 0;
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            Trabalhador trabalhador = _moduloContribuicoesContext.Trabalhador
                .Where(t => !string.IsNullOrWhiteSpace(t.Niss))
                .OrderByDescending(o => o.IdTrabalhador)
                .FirstOrDefault();

            if (trabalhador == null)
            {
                numINSS = 111111111;
            }
            else
            {
                numINSS = long.Parse(trabalhador.Niss) + 1;
            }
            return numINSS.ToString();
        }

        public string GetNextNISSBuySequence()
        {
            var connection = _moduloContribuicoesContext.Database.GetDbConnection();
            try
            {
                // Mở kết nối nếu chưa mở
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                // Tạo lệnh SQL
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT NEXT VALUE FOR SEQ_NISS_TRABALHADOR";

                    // Sử dụng ExecuteScalar để lấy giá trị tiếp theo từ sequence
                    var result = command.ExecuteScalar();

                    // Chuyển đổi giá trị trả về (object) thành long và trả về dưới dạng chuỗi
                    var nextValue = Convert.ToInt64(result);
                    return nextValue.ToString();
                }
            }
            finally
            {
                // Đảm bảo kết nối được đóng lại nếu không sử dụng nữa
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
            //var nextValue = _moduloContribuicoesContext.Database.ExecuteSqlRaw("SELECT NEXT VALUE FOR SEQ_NISS_TRABALHADOR");

            //return nextValue.ToString();
        }

        public Trabalhador GetInternalByNiss(string niss)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var trabalhador = _moduloContribuicoesContext.Trabalhador
                .FirstOrDefault(u => u.Niss == niss && u.Interno == true);

            return trabalhador;
        }

        public DestinatarioDataContract GetDestinatarioByNiss(string niss)
        {
            return _moduloContribuicoesContext.Trabalhador
                .Where(u => u.Niss == niss)
                .Include(u => u.Morada)
                .Select(u => new DestinatarioDataContract
                {
                    TrabalhadorFk = u.IdTrabalhador,
                    Nome = u.Nome,
                    Niss = u.Niss,
                    Tin = u.Tin
                })
                .FirstOrDefault();
        }

        public DestinatarioDataContract GetDestinatarioByTin(string tin)
        {
            return _moduloContribuicoesContext.Trabalhador
                .Where(u => u.Tin == tin)
               .Select(u => new DestinatarioDataContract
               {
                   TrabalhadorFk = u.IdTrabalhador,
                   Nome = u.Nome,
                   Niss = u.Niss,
                   Tin = u.Tin,
               })
                .FirstOrDefault();
        }

        public List<Trabalhador> GetTrabalhadoresByNissOrTin(List<string> niss, List<string> tin)
        {
            return _moduloContribuicoesContext.Trabalhador
                .Where(u => niss.Contains(u.Niss) || tin.Contains(u.Tin))
                .ToList();
        }

        public Destinatario GetDestinatarioByIdTrabalhador(int id)
        {
            return _moduloContribuicoesContext.Trabalhador
                .Where(u => u.IdTrabalhador == id)
               .Select(u => new Destinatario
               {
                   TrabalhadorFk = u.IdTrabalhador,
                   Nome = u.Nome,
                   Niss = u.Niss,
                   Tin = u.Tin,
               })
                .FirstOrDefault();
        }
    }
}