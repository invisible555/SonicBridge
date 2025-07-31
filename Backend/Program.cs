using Backend.Entity;
using Backend.Repository;
using Backend.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Add services to the container.
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ReactProject API", Version = "v1" });

                // JWT Bearer Auth
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Wprowadü token JWT poprzedzony s≥owem 'Bearer', np: **Bearer eyJhbGciOiJIUzI1...**"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                    new BadRequestObjectResult(context.ModelState);
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendCorsPolicy", policy =>
                {
                    policy
                        .WithOrigins("https://localhost:5173") // Dodaj tu swoje adresy frontu!
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); // Waøne jeúli korzystasz z cookie!
                });
            });
            builder.Services.AddHttpClient("WhisperApi", client =>
            {
                client.BaseAddress = new Uri("http://whisper-api:8000");
            });

            builder.Services.AddScoped<ITranscriptionService, TranscriptionService>();


            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            builder.Services.AddScoped<IFileRepository, FileRepository>();
            builder.Services.AddScoped<PasswordHasher<User>>();
            builder.Services.AddScoped<IFileService, FileService>();


            var jwtKey = builder.Configuration["Jwt:Key"] ?? "powerfullkey";
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,

                };
            });


            builder.Services.AddControllers();
           
       
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseCors("FrontendCorsPolicy");
            app.Use(async (context, next) =>
            {
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Append("Authorization", $"Bearer {token}");
                }
                await next();
            });
            app.UseAuthentication();
            app.UseAuthorization();
         

            app.MapControllers();

            app.Run();
        }
    }
}
