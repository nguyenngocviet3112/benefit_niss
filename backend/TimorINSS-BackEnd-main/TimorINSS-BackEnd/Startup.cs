using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataManager.DataManagers;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Extensions;
using TimorINSSBackEnd.Middlewares;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-PT");
            services.ConfigureSqlServerContext(Configuration);
            services.ConfigureCors();
            services.ConfigureIISIntegration();
            services.ConfigureScopes();

            services.AddSingleton<LocalizationMiddleware>();
            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("PT");

                var cultures = new CultureInfo[]
                {
                    new CultureInfo("PT"),
                    new CultureInfo("EN"),
                    new CultureInfo("TE")
                };

                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            //services.AddControllers()
            //.AddNewtonsoftJson(options =>
            //{
            //    options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss"; // Định dạng datetime chung
            //    options.SerializerSettings.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-ddTHH:mm:ss" });
            //});


            services.AddCors();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmailSenderDataManager, EmailSenderDataManager>();

            // configure jwt authentication
            var key = Encoding.ASCII.GetBytes(Configuration["AppSettings:Secret"]);
            var internalTokenSalt = Configuration["AppSettings:TokenSalt"];
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var dataBaseService = context.HttpContext.RequestServices.GetRequiredService<IUtilizadoresRepository>();
                        context.HttpContext.Request.Headers.TryGetValue("User-Id", out var requestUserId);
                        var userId = int.Parse(context.Principal.Identity.Name);

                        var user = dataBaseService.Get(userId);
                        var tokenStr = context.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
                        var validToken = ConfirmToken(internalTokenSalt, context, tokenStr, userId);
                        if (user == null || userId.ToString() != requestUserId || !validToken)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(0)
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx => {
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
                      "Origin, X-Requested-With, Content-Type, Accept");
                },
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), @"Imports")),
                RequestPath = new PathString("/imports")
            });

            app.UseMiddleware<LocalizationMiddleware>();

            app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            // ✅ Middleware giải mã Base64 path trực tiếp
            app.Use(async (context, next) =>
            {
                const string apiPrefix = "/api/";
                var path = context.Request.Path.Value ?? "";

                // kiểm tra nếu request bắt đầu bằng /api/
                if (path.StartsWith(apiPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    var encoded = path.Substring(apiPrefix.Length).Trim('/');

                    // Regex: chỉ các chuỗi có thể là base64-url
                    var base64Regex = new Regex(@"^[A-Za-z0-9\-_]+={0,2}$", RegexOptions.Compiled);

                    if (base64Regex.IsMatch(encoded) && encoded.Length > 8)
                    {
                        try
                        {
                            // Chuẩn hóa base64-url -> base64 chuẩn
                            string normalized = encoded.Replace('-', '+').Replace('_', '/');
                            switch (normalized.Length % 4)
                            {
                                case 2: normalized += "=="; break;
                                case 3: normalized += "="; break;
                            }

                            // Giải mã
                            var bytes = Convert.FromBase64String(normalized);
                            var decoded = Encoding.UTF8.GetString(bytes);

                            // Nếu kết quả có dấu '/', coi là path hợp lệ
                            if (decoded.Contains('/'))
                            {
                                context.Request.Path = apiPrefix + decoded;
                            }
                        }
                        catch
                        {
                            // nếu lỗi decode → bỏ qua, giữ nguyên path
                        }
                    }
                }

                await next();
            });


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        # region TokenMethod

        public bool ConfirmToken(string internalTokenSalt, TokenValidatedContext context, string token, int userId)
        {
            var tokenDB = context.HttpContext.RequestServices.GetRequiredService<IUtilizadorTokenRepository>();
            var userToken = tokenDB.GetByUserId(userId).FirstOrDefault();
            if (userToken != null && userToken.TokenString == CreateHashToken(token, userToken.Salt, internalTokenSalt))
                return true;
            else
                return false;
        }

        public string CreateHashToken(string token, string tokenSalt, string internalTokenSalt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                             password: token,
                             salt: Encoding.UTF8.GetBytes(tokenSalt + internalTokenSalt),
                             prf: KeyDerivationPrf.HMACSHA512,
                             iterationCount: 10000,
                             numBytesRequested: 256 / 8);

            var hashedPassword = Convert.ToBase64String(valueBytes);

            return hashedPassword;
        }

        #endregion
    }
}