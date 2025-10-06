using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Errors;
using SurveyBasket.Core.Authentication;
using SurveyBasket.Repository.Persistence;
using SurveyBasket.Services.Authentication;
using SurveyBasket.Services.Services;
using System.Reflection;
using System.Text;

namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
            ));

        services.AddAuth(configuration);

        services.AddDatabase(configuration);

        services.AddSwagger();

        services.AddFluentValidation();

        services.AddMapster();

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddMapster(this IServiceCollection services)
    {
        var mappingConfiguration = TypeAdapterConfig.GlobalSettings;
        mappingConfiguration.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfiguration));
        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();

        //services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddOptions<JwtOptions>()
            .BindConfiguration("Jwt")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "SurveyBasket",
                    ValidAudience = "SurveyBasketClient",
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("LBaxSXNdx7SSWCqzE8EJFFHAtSpd5KrU")),
                    ClockSkew = TimeSpan.Zero
                };
            });
        return services;
    }
}