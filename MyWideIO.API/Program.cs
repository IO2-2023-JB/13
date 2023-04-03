using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyVideIO.Data;
using MyWideIO.API.Data;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Data.Repositories;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ApiDbConnection")));
        builder.Services.AddIdentity<ViewerModel, UserRole>(config =>
        {
            config.SignIn.RequireConfirmedEmail = false;
            config.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
            config.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

        });
        builder.Services.AddControllers();
       

        //builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ApiDbConnection")));
        builder.Services.AddScoped<IApiRepository, ApiRepository>();
        builder.Services.AddScoped<IUserService, UserService>();
        //builder.Services.AddScoped<IApiRepository, ApiRepository>();
        // Add services to the container.

        // CORS
        builder.Services.AddCors(options =>
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
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TajnyKlucz128bit")), // ten sam klucz
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        });
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(config =>
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

            //config.SwaggerDoc("v1.3", new OpenApiInfo
            //{
            //    Version = "v1.3",
            //    Title = "MyWideIO.API",
            //});
        });

        CreateRoles(builder.Services.BuildServiceProvider()).Wait();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope()) // ?
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // ??
                                                                                              //dbContext.Database.Migrate();
        }
        // Configure the HTTP request pipeline. 
        // if (app.Environment.IsDevelopment()) na razie wlaczony swagger na release
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        var a = JwtSecurityTokenHandler.DefaultInboundClaimTypeMap;

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        // CORS
        app.UseCors("AllowLocalhost3000");

        app.UseDeveloperExceptionPage(); // do wywalenia

        app.Run();
    }
    private static async Task CreateRoles(IServiceProvider serviceProvider)
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
}