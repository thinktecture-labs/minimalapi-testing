using Microsoft.EntityFrameworkCore;

namespace PatientService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services);

        var app = builder.Build();
        ConfigurePipeline(app);

        await EnsureDb(app.Services);
        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDbContext<PatientDbContext>(o => o.UseSqlite("Data Source=patients.db"));

// services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = "https://demo.duendesoftware.com";
//         options.Audience = "api";
//     });

        services.AddAuthorization();
        services.AddControllers();
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        app.UseSwaggerUI();
// app.UseAuthentication();
// app.UseAuthorization();

        app.MapSwagger();
        app.MapPatientHandlers("/patient");
        app.MapControllers();
    }

    private static async Task EnsureDb(IServiceProvider services)
    {
        await using var db = services.CreateScope().ServiceProvider.GetRequiredService<PatientDbContext>();
        await db.Database.MigrateAsync();
    }
}

public record PatientInputModel(string Firstname, string Lastname);