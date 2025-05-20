using Dawam_backend.Data;
using Dawam_backend.Helpers;
using Dawam_backend.Models;
using Dawam_backend.Services;
using Dawam_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using System.Text;

namespace Dawam_backend.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddDbContextAndIdentity(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["Jwt:Key"]))
                };
            });
        }

        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dawam API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme"
                });

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
        }

        public static void AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<JwtTokenHelper>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IAnalysisService, AnalysisService>();
            services.AddScoped<ISavedJobService, SavedJobService>();
            services.AddScoped<IAuthService, AuthService>();

            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            services.AddTransient<IEmailService, EmailService>();
        }

        public static void AddStripeConfiguration(this IServiceCollection services, IConfiguration config)
        {
            var stripeSettings = config.GetSection("Stripe");
            StripeConfiguration.ApiKey = stripeSettings["SecretKey"];
        }
    }
}