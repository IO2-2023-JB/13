﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyWideIO.API.Data;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Data.Repositories;
using MyWideIO.API.Middleware;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Enums;
using MyWideIO.API.Services;
using MyWideIO.API.Services.Interfaces;
using Org.OpenAPITools.Filters;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Primitives;
using MyWideIO.API.Filters;

namespace MyWideIO.API
{
    public class Startup
    {
        // todo - allow request with size > 5mb
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ApiDbConnection")));
            services.AddIdentity<AppUserModel, UserRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = false;
                config.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                config.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 0;

            });
            services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddBlobServiceClient(Configuration.GetConnectionString("AzureBlobConnection"));
            });

            services.AddControllers();

            
            services.AddSingleton<IImageStorageService, AzureBlobImageStorageService>(); // singleton powinien byc ok
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IVideoStorageService, AzureBlobVideoStorageService>();

            services.AddScoped<IVideoRepository, VideoRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<ITransactionService, TransactionService>();

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost3000",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                        //builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

            // JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:JwtKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
                // allow token in query string
                options.Events = new JwtBearerEvents {
                    OnMessageReceived = (context) => {

                        if (!context.Request.Query.TryGetValue("access_token", out var values)) {
                            return Task.CompletedTask;
                        }

                        if (values.Count > 1)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Fail(
                                "Only one 'access_token' query string parameter can be defined. " +
                                $"However, {values.Count:N0} were included in the request."
                            );

                            return Task.CompletedTask;
                        }

                        var token = values.Single();

                        if (string.IsNullOrWhiteSpace(token)) {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Fail(
                                "The 'access_token' query string parameter was defined, " +
                                "but a value to represent the token was not included."
                            );

                            return Task.CompletedTask;
                        }
                        
                        context.Token = token.StartsWith("Bearer ") ? token[7..] : token;

                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();
            //builder.Services.AddControllers(options =>
            //{
            //    //options.InputFormatters.Insert(0,new InputFormatterStream());
            //}).AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(config =>
            {
                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.\r\n\r\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                config.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
                config.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);
                config.SwaggerDoc("1.0.6", new OpenApiInfo
                {
                    Title = "VideIO API",
                    Description = "VideIO project API specification.",
                    Version = "1.0.6",
                });
                config.DocumentFilter<BasePathFilter>("/VideIO/1.0.6");
                config.OperationFilter<GeneratePathParamsValidationFilter>();
                config.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly().GetName().Name}.xml");
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = null; //:)
                options.Limits.MaxRequestBufferSize = null;
            });

            CreateRoles(services.BuildServiceProvider()).Wait();
        }
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            // role init
            var roleManager = serviceProvider.GetRequiredService<RoleManager<UserRole>>();
            
            string[] roleNames = Enum.GetValues(typeof(UserTypeEnum)).Cast<UserTypeEnum>().Select(t => t.ToString()).ToArray();

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                    await roleManager.CreateAsync(new UserRole(roleName));
            }
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("AllowLocalhost3000");

            {
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/openapi/1.0.6/openapi.json", "VideIO API");
                });
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "openapi/{documentName}/openapi.json";
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}/api" } };
                    });
                });
            }
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UsePathBase("/api");

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(config =>
            {
                config.MapControllers();
            });

            // CORS

            app.UseDeveloperExceptionPage(); // do wywalenia
        }
    }
}
