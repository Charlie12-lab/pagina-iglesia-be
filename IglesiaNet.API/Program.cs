using System.Text;
using IglesiaNet.Application.Auth;
using IglesiaNet.Application.Blogs;
using IglesiaNet.Application.Churches;
using IglesiaNet.Application.Common;
using IglesiaNet.Application.Events;
using IglesiaNet.Domain.Blogs;
using IglesiaNet.Domain.Churches;
using IglesiaNet.Domain.Events;
using IglesiaNet.Domain.Users;
using IglesiaNet.Infrastructure.Authentication;
using IglesiaNet.Infrastructure.Persistence.MongoDB;
using IglesiaNet.Infrastructure.Persistence.SQL;
using IglesiaNet.Infrastructure.Persistence.SQL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ─── SQL Server (EF Core) ────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// ─── MongoDB ────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<MongoDbContext>();

// ─── Repositorios (Infrastructure → Domain interfaces) ───────────────────────
builder.Services.AddScoped<IChurchRepository, ChurchRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();

// ─── Servicios de infraestructura ───────────────────────────────────────────
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();

// ─── Application Services ────────────────────────────────────────────────────
builder.Services.AddScoped<AuthAppService>();
builder.Services.AddScoped<ChurchAppService>();
builder.Services.AddScoped<EventAppService>();
builder.Services.AddScoped<BlogAppService>();

// ─── JWT Authentication ─────────────────────────────────────────────────────
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT Secret not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ─── CORS ────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(builder.Configuration["AllowedOrigins"] ?? "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ─── Controllers + Swagger ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IglesiaNet API",
        Version = "v1",
        Description = "API para la plataforma de iglesias — arquitectura DDD"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Ejemplo: 'Bearer {token}'",
        Name = "Authorization", In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey, Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ─── Auto-migrar + Seed ──────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    db.Database.Migrate();
    await DbSeeder.SeedAsync(db, hasher);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
