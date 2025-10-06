using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UserRegistration.Application.Users.Commands.Register;
using UserRegistration.Infrastructure;
using UserRegistration.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserRegistration.API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite: Bearer {seu_token}"
    });

    // Referência por ID "Bearer" (importante pro cadeado funcionar)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<UserRegistrationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("UserRegistration")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());

builder.Services.AddScoped<
    UserRegistration.Application.Abstractions.Repositories.IUsersRepository,
    UserRegistration.Infrastructure.Repositories.UsersRepository>();

builder.Services.AddSingleton<
    UserRegistration.Application.Abstractions.Security.IPasswordHasher,
    UserRegistration.Infrastructure.Security.PasswordHasher>();

builder.Services.AddScoped<UserRegistration.Application.Abstractions.IUnitOfWork>(sp =>
    sp.GetRequiredService<UserRegistration.Infrastructure.UserRegistrationDbContext>());

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<
    UserRegistration.Application.Abstractions.Security.IJwtTokenGenerator,
    UserRegistration.Infrastructure.Security.JwtTokenGenerator>();

builder.Services.AddSingleton<
    UserRegistration.Application.Abstractions.Security.IJwtTokenGenerator,
    UserRegistration.Infrastructure.Security.JwtTokenGenerator>();

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowSpa", p => p
        .WithOrigins(
            "http://localhost:4200",
            "https://localhost:4200"  
        )
        .AllowAnyHeader()
        .AllowAnyMethod()    
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpa");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
