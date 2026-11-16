using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.Data;
using Service.Helpers.Account;
using Service;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Text;
using Repository;
using Microsoft.OpenApi.Models;
using FinalProject.Middlewares;
using FinalProject.Helpers;

// Kompüterin regional ayarı (az-Latn-AZ) onluq ayırıcı kimi vergül işlədir.
// Form-dan gələn "12.50" kimi qiymətlərin düzgün oxunması üçün invariant culture.
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});


// ==================== CORS ====================

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// ==================== IDENTITY ====================

builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddErrorDescriber<AzIdentityErrorDescriber>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequiredUniqueChars = 1;
    opt.Password.RequireNonAlphanumeric = true;
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;
    // appsettings: "Auth:RequireConfirmedEmail" (Development-də false — e-poçt təsdiqi olmadan giriş)
    opt.SignIn.RequireConfirmedEmail =
        builder.Configuration.GetValue("Auth:RequireConfirmedEmail", true);
});


// ==================== SETTINGS ====================

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Smtp"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JWTSettings"));

builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("Stripe"));


// ==================== DATABASE ====================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));


// ==================== REPOSITORY & SERVICE ====================

builder.Services.AddRepositoryLayer();

builder.Services.AddServiceLayer();


// ==================== SWAGGER ====================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// ==================== JWT ====================

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;

        cfg.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidIssuer =
                    builder.Configuration["JWTSettings:Issuer"],

                ValidAudience =
                    builder.Configuration["JWTSettings:Issuer"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["JWTSettings:Key"])),

                ClockSkew = TimeSpan.Zero
            };
    });


// ==================== AUTHORIZATION ====================

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("Admin", p =>
        p.RequireRole("Admin"));
});


var app = builder.Build();


// ==================== SEED (rollar + admin istifadəçi) ====================

await DataSeeder.SeedAsync(app.Services, app.Configuration, app.Logger);


// ==================== SWAGGER ====================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ==================== MIDDLEWARE ====================

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// wwwroot/images altındakı şəkillər frontend-ə /images/... ünvanı ilə verilir
app.UseStaticFiles();


// ==================== CORS ====================

app.UseCors("Frontend");


// ==================== AUTHENTICATION ====================

app.UseAuthentication();

app.UseAuthorization();


// ==================== CONTROLLERS ====================

app.MapControllers();


// ==================== RUN ====================

app.Run();
