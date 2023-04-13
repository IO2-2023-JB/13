using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyVideIO.Data;
using MyWideIO.API.Data;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Data.Repositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services;
using MyWideIO.API.Services.Interfaces;
using Org.OpenAPITools.Filters;
using System.Reflection;
using System.Text;

namespace MyWideIO.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ApiDbConnection")));
            services.AddIdentity<ViewerModel, UserRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = false;
                config.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                config.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

            });
            services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddBlobServiceClient(Configuration.GetConnectionString("AzureBlobConnection"));
            });

            services.AddControllers();
            services.AddSingleton<IImageService, AzureBlobImageService>(); // singleton powinien byc ok
            services.AddSingleton<ITokenService, TokenService>(); // singleton powinien byc ok

            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ApiDbConnection")));
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IVideoRepository, VideoRepository>();
            services.AddScoped<IVideoService, AzureBlobVideoService>();
            // Add services to the container.

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost3000",
                    builder =>
                    {
                        //builder.WithOrigins("http://localhost:3000")
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                        //.AllowCredentials();
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
            CreateRoles(services.BuildServiceProvider()).Wait();
        }
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            // role init
            var roleManager = serviceProvider.GetRequiredService<RoleManager<UserRole>>();
            string[] roleNames = { "Viewer", "Creator", "Admin" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                    await roleManager.CreateAsync(new UserRole(roleName));
            }
        }
        public void Configure(IApplicationBuilder app)
        {
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "openapi/{documentName}/openapi.json";
                });
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/openapi/1.0.6/openapi.json", "VideIO API");
                });
            }
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(config =>
            {
                config.MapControllers();
            });

            // CORS
            app.UseCors("AllowLocalhost3000");

            app.UseDeveloperExceptionPage(); // do wywalenia
        }
    }
}
