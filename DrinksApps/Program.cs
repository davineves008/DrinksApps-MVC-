
using DrinksApps.Data;
using DrinksApps.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// ======================================
// MVC
// ======================================

builder.Services.AddControllersWithViews();


// ======================================
// PASSWORD HASHER
// ======================================

builder.Services.AddScoped<
    IPasswordHasher<Usuario>,
    PasswordHasher<Usuario>>();


// ======================================
// CORS
// ======================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("app", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});


// ======================================
// JWT
// ======================================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key não configurada. Verifique as variáveis de ambiente.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Jwt:Issuer não configurado.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Jwt:Audience não configurado.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.FromMinutes(1)
            };
    });

builder.Services.AddAuthorization();


// ======================================
// SESSÃO
// ======================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;
});


// ======================================
// BANCO DE DADOS
// ======================================

builder.Services.AddDatabase(builder.Configuration);


// ======================================
// CULTURA BRASILEIRA
// ======================================

var cultura = new CultureInfo("pt-BR");

CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;


// ======================================
// BUILD
// ======================================

var app = builder.Build();


// ======================================
// TRATAMENTO DE ERROS
// ======================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// ======================================
// MIDDLEWARES
// ======================================

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRequestLocalization(
    new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture(cultura),
        SupportedCultures = new[] { cultura },
        SupportedUICultures = new[] { cultura }
    });

app.UseRouting();

app.UseCors("app");

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();


// ======================================
// ROTAS
// ======================================

// Controllers com rotas por atributos
app.MapControllers();

// Rotas MVC convencionais
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");


// ======================================
// EXECUÇÃO
// ======================================

app.Run();