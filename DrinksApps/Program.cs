using DrinksApps.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// MVC
// ===============================
builder.Services.AddControllersWithViews();



// CORS (permite o app Expo na web chamar a API)
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("app", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});



// JWT (autenticação da API)
// ===============================
// A chave vem do secrets.json (local) ou do appsettings.Production.json (servidor).
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key não configurada. Defina no secrets.json (local) ou no appsettings.Production.json (servidor).");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });



// Sessão
// ===============================
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Banco de Dados
// ===============================
builder.Services.AddDatabase(builder.Configuration);


// Cultura Brasileira
// ===============================
var cultura = new CultureInfo("pt-BR");

CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;



// Build da aplicação
// ===============================
var app = builder.Build();



// Tratamento de erros
// ===============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



// Middlewares
// ===============================
app.UseHttpsRedirection();

app.UseStaticFiles();


// Configuração de idioma
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new[] { cultura },
    SupportedUICultures = new[] { cultura }
});


app.UseRouting();

// CORS: depois do UseRouting e antes da autenticação
app.UseCors("app");

app.UseSession();

// Autenticação (lê o token) vem ANTES da autorização
app.UseAuthentication();

app.UseAuthorization();



// Rotas MVC
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");



// Executa aplicação
// ===============================
app.Run();