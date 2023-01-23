using Api.Configs;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using DAL;
using Api.Mapper;
using Api.Middlewares;
using AspNetCoreRateLimit;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var authSection = builder.Configuration.GetSection(AuthConfig.Position);
        var authConfig = authSection.Get<AuthConfig>();
        var emailSection = builder.Configuration.GetSection(EmailConfig.Position);
        var pushSection = builder.Configuration.GetSection(PushConfig.Position);

        builder.Services.Configure<AuthConfig>(authSection); // Чтобы Config появился в DI
        builder.Services.Configure<EmailConfig>(emailSection);
        builder.Services.Configure<PushConfig>(pushSection);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = "Enter the user token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme,

            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,

                        },
                        Scheme = "oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });

            c.SwaggerDoc("Auth", new OpenApiInfo { Title = "Auth" });
            c.SwaggerDoc("Api", new OpenApiInfo { Title = "Api" });
        });

        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"), sql => { });
        }, contextLifetime: ServiceLifetime.Scoped);

        builder.Services.AddOptions();
        builder.Services.AddMemoryCache();
        builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
        builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));
        builder.Services.AddInMemoryRateLimiting();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
        
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<GooglePushService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<PostService>();
        builder.Services.AddScoped<LinkGeneratorService>();
        builder.Services.AddScoped<EmailService>();
        builder.Services.AddScoped<FileService>();
        //builder.Services.AddSingleton<DdosGuard>();
        builder.Services.AddScoped<CommentService>();
        builder.Services.AddScoped<CheckService>();
        builder.Services.AddScoped<LikeService>();

        builder.Services.AddAuthentication(o => // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        {
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false; // Выключение проверки на SSL из-за того, что нет нормального SSL-сертификата
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = authConfig.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = authConfig.SymmetricSecurityKey(),
                ClockSkew = TimeSpan.Zero,
            };

        });

        builder.Services.AddAuthorization(o =>
        {
            o.AddPolicy("ValidAccessToken", p =>
            {
                p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();
            });
        });



        var app = builder.Build();

        // using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope()) ?
        using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
        {
            if (serviceScope != null)
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DAL.DataContext>();
                context.Database.Migrate();
            }
        }

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("Api/swagger.json", "Api");
                c.SwaggerEndpoint("Auth/swagger.json", "Auth");
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseClientRateLimiting();

        app.UseAuthorization();
        app.UseGlobalErrorWrapper();
        app.UseTokenValidator();
        app.MapControllers();

        app.Run();
    }
}