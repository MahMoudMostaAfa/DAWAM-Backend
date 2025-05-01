
using Dawam_backend.Data;
using Dawam_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Dawam_backend.Services.Interfaces;
using Dawam_backend.Services;
using Dawam_backend.Helpers;

namespace Dawam_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure DbContext with connection string
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // REGISTER JWT SERVICE
            builder.Services.AddScoped<JwtTokenHelper>();
            // Configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            // Add JWT Authentication with configuration from appsettings.json
            builder.Services.AddAuthentication(options =>
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
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            // Add authorization services (for role-based access control)
            builder.Services.AddAuthorization();

            // Register services for DI
            builder.Services.AddScoped<IJobService, JobService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();

            // Add Swagger for API documentation
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dawam API", Version = "v1" });
                // Enable authorization using JWT in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid JWT token.\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
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

            builder.Services.AddEndpointsApiExplorer();
            

            var app = builder.Build();
            // Initialize roles
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                SeedRoles.Initialize(services, userManager, roleManager);
            }
            // Initial Admin 
            async Task SeedAdminUserAsync(IServiceProvider services)
            {
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var dbContext = services.GetRequiredService<ApplicationDbContext>();

                var adminEmail = "admin@dawam.com";
                var adminPassword = "Admin@12345"; // Secure password

                // Create Admin Role if not exists
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                // Create Admin User if not exists
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Admin User",
                        IsActive = true
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }


                    if (!dbContext.Categories.Any())
                    {
                        dbContext.Categories.Add(new Category
                        {
                            Name = "Software Development",
                        });
                        await dbContext.SaveChangesAsync();
                    }
                }
            }

            using  (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                SeedAdminUserAsync(services).GetAwaiter().GetResult();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                // Seed jobs if necessary
                SeedJobs(context);
            }



            // Configure the HTTP request pipeline.
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dawam API V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication(); // Very Important: for Identity
            app.UseAuthorization();
            // for cvs
            app.UseStaticFiles();
            app.MapControllers();

            app.Run();
        }

        private static void SeedJobs(ApplicationDbContext context)
        {
            if (!context.Jobs.Any()) // Check if there are any jobs in the database
            {
                var categoryId = context.Categories.FirstOrDefault()?.Id; // Get a valid category or use a default ID
                var userId = "c714889a-ea76-4d9f-bd86-6f5ba16c6801"; // Replace this with an actual user ID, for example an admin user ID

                var jobs = new List<Job>
        {
            new Job
            {
                Title = "Software Engineer",
                Description = "Responsible for developing and maintaining software applications.",
                Requirements = "C#, .NET, SQL",
                CategoryId = categoryId,
                CareerLevel = "Mid-Level",
                JobType = "Full-Time",
                Location = "Remote",
                PostedBy = userId,
                CreatedAt = DateTime.UtcNow
            },
            new Job
            {
                Title = "Data Scientist",
                Description = "Work on analyzing large datasets to provide business insights.",
                Requirements = "Python, Machine Learning, SQL",
                CategoryId = categoryId,
                CareerLevel = "Senior",
                JobType = "Full-Time",
                Location = "On-Site",
                PostedBy = userId,
                CreatedAt = DateTime.UtcNow
            }
        };

                context.Jobs.AddRange(jobs);
                context.SaveChanges();
            }
        }
    }


}
