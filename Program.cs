using ApiRaizesNordeste.Extensions;
using ApiRaizesNordeste.Middleware;
using ApiRaizesNordeste.Repositories;
using ApiRaizesNordeste.Services;
using ApiRaizesNordeste.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços de banco de dados
builder.Services.AddSwaggerConfiguration();
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddHealthChecks();

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services.Configure<JwtOptions>(jwtSection);
var jwtOptions = jwtSection.Get<JwtOptions>()
    ?? throw new InvalidOperationException("A configuracao JWT nao foi encontrada.");
if (Encoding.UTF8.GetByteCount(jwtOptions.Key) < 32)
    throw new InvalidOperationException("Jwt:Key deve possuir pelo menos 32 bytes.");

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
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

// Registrar validadores FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnidadeRepository, UnidadeRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

// Registrar repositórios
// Registrar serviços de negócio
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IUnidadeService, UnidadeService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// Registrar controllers
builder.Services.AddControllers();

var app = builder.Build();

// Adicionar middleware de validação e exceções
app.UseMiddleware<ValidationExceptionMiddleware>();

// Configurar middleware
app.UseSwaggerConfiguration();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
