using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using PatientService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PatientDbContext>(o => o.UseSqlite("Data Source=patients.db"));

// builder.Services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = "https://demo.duendesoftware.com";
//         options.Audience = "api";
//     });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

await EnsureDb(app.Services);

app.UseSwaggerUI();

// app.UseAuthentication();
// app.UseAuthorization();

app.MapSwagger();
app.MapPatientHandlers("/patient");
app.MapControllers();

app.Run();

async Task EnsureDb(IServiceProvider services)
{
    await using var db = services.CreateScope().ServiceProvider.GetRequiredService<PatientDbContext>();
    await db.Database.MigrateAsync();
}

public record PatientInputModel(string Firstname, string Lastname);