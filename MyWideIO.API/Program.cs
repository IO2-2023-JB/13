using MyVideIO.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyWideIO.API.Data.Repositories;
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Services;
using MyWideIO.API.Models.DB_Models;
//using MyVideIO.Models;
using Microsoft.AspNetCore.Identity;
using MyWideIO.API.Data;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ApiDbConnection")));
builder.Services.AddIdentity<ViewerModel, IdentityRole>(config =>
{
    config.SignIn.RequireConfirmedEmail = false;
    config.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    config.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>()
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// CORS
app.UseCors("AllowLocalhost3000");

app.Run();
