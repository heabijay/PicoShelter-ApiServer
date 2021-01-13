using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Services;
using PicoShelter_ApiServer.DAL;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL;
using PicoShelter_ApiServer.FDAL.Interfaces;
using PicoShelter_ApiServer.Responses;
using PicoShelter_ApiServer.Responses.Models;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

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
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=picoshelterdb;Trusted_Connection=True;";
            services.AddScoped<IUnitOfWork>(s => new EFUnitOfWork(connectionString));
            services.AddScoped<IFileUnitOfWork>(s => new FileUnitOfWork("FileRepository"));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IAlbumService, AlbumService>();

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
                            var token = context.SecurityToken as JwtSecurityToken;
                            if (token != null)
                            {
                                var id = int.Parse(token.Claims.FirstOrDefault().Value);
                                var db = context.HttpContext.RequestServices.GetService<IUnitOfWork>();
                                var acc = db.Accounts.Get(id);
                                if (acc != null)
                                {
                                    if (acc.LastPasswordChange > token.ValidFrom)
                                        context.Fail("Password was changed after login");
                                }
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

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
                     new string[] { }
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "apidocs";
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "PicoShelter API");
            });
        }
    }
}
