using Microsoft.EntityFrameworkCore;
using PatientService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PatientDbContext>(o => o.UseSqlite("Data Source=patients.db"));

var app = builder.Build();

await EnsureDb(app.Services);

app.MapSwagger();
app.UseSwaggerUI();

app.MapGet("", PatientHandlers.GetAll).WithName("GetAll");
app.MapGet("{id}", PatientHandlers.GetById).WithName("Get");
app.MapPut("{id}", PatientHandlers.Upsert).WithName("Upsert");
app.MapDelete("{id}", PatientHandlers.Delete).WithName("Delete");

app.Run();

async Task EnsureDb(IServiceProvider services)
{
    using var db = services.CreateScope().ServiceProvider.GetRequiredService<PatientDbContext>();
    await db.Database.MigrateAsync();
}

public record PatientInputModel(string Firstname, string Lastname);