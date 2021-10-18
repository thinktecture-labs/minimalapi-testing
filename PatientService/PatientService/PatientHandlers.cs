using Microsoft.EntityFrameworkCore;
using PatientService;

namespace PatientService;

public static class PatientHandlers
{
    public static async Task<IResult> GetAll(PatientDbContext db) => Results.Ok(await db.Patients.ToListAsync());

    public static async Task<IResult> GetById(PatientDbContext db, Guid id)
    {
        if (await db.Patients.FindAsync(id) is Patient p)
        {
            return Results.Ok(p);
        }

        return Results.NotFound();
    }

    public static async Task<IResult> Delete(PatientDbContext db, Guid id)
    {
        if (await db.Patients.FindAsync(id) is Patient p)
        {
            db.Patients.Remove(p);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    }

    public static async Task<IResult> Upsert(PatientDbContext db, Guid id, PatientInputModel input)
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
    }
}