using Microsoft.EntityFrameworkCore;
using PatientService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PatientDbContext>(o => o.UseSqlite("Data Source=patients.db"));

var app = builder.Build();

await EnsureDb(app.Services, app.Logger);

app.MapSwagger();
app.UseSwaggerUI();

app.MapGet("/", async (PatientDbContext db) => await db.Patients.ToListAsync())
    .WithName("GetAll");
app.MapGet("/{id}"
        , async (PatientDbContext db, Guid id) =>
        {
            if (await db.Patients.FindAsync(id) is Patient p)
            {
                return Results.Ok(p);
            }

            // Null will not be translated to NotFound. Must be done manualy.
            return Results.NotFound();
        })
    .WithName("Get");

app.MapPut("/{id}"
    , async (PatientDbContext db, Guid id, PatientInputModel input) =>
    {
        var p = await db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (p == null)
        {
            p = new Patient()
            {
                Id = id
            };
            await db.Patients.AddAsync(p);
        }

        p.Firstname = input.Firstname;
        p.Lastname = input.Lastname;

        await db.SaveChangesAsync();
        return Results.NoContent();
    }).WithName("Upsert");
app.MapDelete("/{id}"
    , async (PatientDbContext db, Guid id) =>
    {
        if (await db.Patients.FindAsync(id) is Patient p)
        {
            db.Patients.Remove(p);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    }).WithName("Delete");

app.Run();

async Task EnsureDb(IServiceProvider services, ILogger logger)
{
    using var db = services.CreateScope().ServiceProvider.GetRequiredService<PatientDbContext>();
    await db.Database.MigrateAsync();
}

public record PatientInputModel(string Firstname, string Lastname);