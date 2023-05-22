using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Services;
using PicoShelter_ApiServer.DAL;
using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL;
using PicoShelter_ApiServer.FDAL.Interfaces;
using PicoShelter_ApiServer.Responses;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using Hangfire.MySql;
using PicoShelter_ApiServer.Hubs;

namespace PicoShelter_ApiServer
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
            var connectionStrings = Configuration.GetSection("ConnectionStrings");
            var defaultConnectionString = connectionStrings.GetValue<string>("DefaultConnection");
            var isMySql = defaultConnectionString.StartsWith("EnvironmentVariableMySQL=", StringComparison.OrdinalIgnoreCase);
            if (defaultConnectionString.StartsWith("EnvironmentVariable=", StringComparison.OrdinalIgnoreCase))
            {
                defaultConnectionString = Environment.GetEnvironmentVariable(defaultConnectionString.Replace("EnvironmentVariable=", "", StringComparison.OrdinalIgnoreCase));
            }
            else if (isMySql)
            {
                defaultConnectionString = Environment.GetEnvironmentVariable(defaultConnectionString.Replace("EnvironmentVariableMySQL=", "", StringComparison.OrdinalIgnoreCase));
                if (defaultConnectionString != null)
                    defaultConnectionString = MySQLToNetConnectionString(defaultConnectionString);
            }

            var smtpServers = Configuration.GetSection("SmtpServers");
            var defaultSmtpServer = smtpServers.GetSection("DefaultServer");
            var defaultSmtpServerAuth = defaultSmtpServer.GetSection("Authorization");

            var defaultSmtpServerConfig = new EmailAuthDto(
                host: defaultSmtpServer.GetValue<string>("Host"),
                port: defaultSmtpServer.GetValue<int>("Port"),
                useSsl: defaultSmtpServer.GetValue<bool>("UseSsl"),
                username: defaultSmtpServerAuth.GetValue<string>("Username"),
                password: defaultSmtpServerAuth.GetValue<string>("Password"),

                from: new(
                    name: "PicoShelter",
                    address: defaultSmtpServer.GetValue<string>("FromAddress")
                )
            );

            services.AddScoped<ApplicationContext>(s => new ApplicationContext(defaultConnectionString));
            services.AddScoped<IUnitOfWork>(s => new EFUnitOfWork(defaultConnectionString));
            services.AddScoped<IFileUnitOfWork>(s => new FileUnitOfWork(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FileRepository")));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IAlbumService, AlbumService>();
            services.AddScoped<IEmailService>(s => new EmailService(defaultSmtpServerConfig, s.GetService<ILogger<IEmailService>>()));
            services.AddScoped<IConfirmationService, ConfirmationService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddSingleton<ICommentNotifier, CommentHub>();

            services.AddCors();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = AuthOptions.Audience,

                        ValidateLifetime = true,

                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                    options.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = context =>
                        {
                            if (context.SecurityToken is JwtSecurityToken token)
                            {
                                var id = int.Parse(token.Claims.FirstOrDefault().Value);
                                var accountService = context.HttpContext.RequestServices.GetService<IAccountService>();
                                if (!accountService.TokenCheckPasswordChange(id, token.ValidFrom))
                                    context.Fail("Password was changed after login");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseStorage<JobStorage>(isMySql
                    ? new MySqlStorage(defaultConnectionString, new MySqlStorageOptions()
                    {
                        TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 50000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "Hangfire"
                    })
                    : new SqlServerStorage(defaultConnectionString, new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    })));

            services.AddHangfireServer();

            services.AddControllers();
            services.AddMvc()
                .ConfigureApiBehaviorOptions(options =>
                {
                    //options.SuppressModelStateInvalidFilter = true;
                    options.InvalidModelStateResponseFactory = actionContext =>
                    {
                        var modelState = actionContext.ModelState;
                        return new ModelStateErrorResponse(modelState);
                    };
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSignalR();

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.EnableAnnotations();

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                     new OpenApiSecurityScheme
                     {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                     },
                     Array.Empty<string>()
                }});

                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo()
                    {
                        Title = "PicoShelter API",
                        Description = "Powered by ASP.NET Core",
                        Version = "1.0.0.0",
                        Contact = new OpenApiContact()
                        {
                            Email = "heabijay@gmail.com",
                            Name = "heabijay",
                            Url = new Uri("https://t.me/heabijay")
                        }
                    }
                );
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseDeveloperExceptionPage();
            
            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<CommentHub>("/live-comments")
                    .AllowAnonymous()
                    .RequireCors(cors =>
                    {
                        cors.AllowAnyHeader();
                        cors.AllowAnyOrigin();
                        cors.AllowAnyMethod();
                    });
                
                endpoints.Map("/", context =>
                {
                    var url = app.ApplicationServices.GetService<IConfiguration>().GetSection("WebApp").GetSection("Default").GetValue<string>("HomeUrl");
                    context.Response.Redirect(url, true);

                    return Task.CompletedTask;
                });
                endpoints.MapDefaultControllerRoute();
            });

            app.UseHangfireDashboard();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "apidocs";
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "PicoShelter API");
            });
        }


        private static string MySQLToNetConnectionString(string connectionString)
        {
            var matches = Regex.Matches(
                    connectionString,
                    "((^|;)User Id=(?<Uid>[^;]+))|((^|;)Password=(?<Pwd>[^;]+))|((^|;)Database=(?<Database>[^;]+))|((^|;)Data Source=((?<Server>[^:;]+):?(?<Port>[^;]*)))",
                    RegexOptions.IgnoreCase
                    ).Cast<Match>();

            connectionString = "";
            foreach (var m in matches)
            {
                if (m.Groups["Server"].Success)
                    connectionString += "Server=" + m.Groups["Server"].Value + ';';

                if (m.Groups["Port"].Success)
                    connectionString += "Port=" + m.Groups["Port"].Value + ';';

                if (m.Groups["Database"].Success)
                    connectionString += "Database=" + m.Groups["Database"].Value + ';';

                if (m.Groups["Uid"].Success)
                    connectionString += "Uid=" + m.Groups["Uid"].Value + ';';

                if (m.Groups["Pwd"].Success)
                    connectionString += "Pwd=" + m.Groups["Pwd"].Value + ';';
            }

            return connectionString;
        }
    }
}
