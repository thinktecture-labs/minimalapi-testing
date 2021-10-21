using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace PatientService;

public static class PatientHandlers
{
    public static IEndpointRouteBuilder MapPatientHandlers(this IEndpointRouteBuilder builder, string path)
    {
        builder.Branch(path
            , patients =>
            {
                patients.MapGet(PatientHandlers.GetAll)
                    .Produces<Patient[]>();
                patients.Branch("{id}"
                    , patient =>
                    {
                        patient.MapGet(PatientHandlers.GetById)
                            .Produces<Patient>()
                            .Produces(StatusCodes.Status404NotFound);
                        patient.MapPut(PatientHandlers.Upsert)
                            .Produces(StatusCodes.Status204NoContent);
                        patient.MapDelete(PatientHandlers.Delete)
                            .Produces(StatusCodes.Status204NoContent)
                            .Produces(StatusCodes.Status404NotFound);
                    });
            });

        return builder;
    }

    private static async Task<IResult> GetAll(PatientDbContext db) => Results.Ok(await db.Patients.ToListAsync());

    private static async Task<IResult> GetById(PatientDbContext db, Guid id)
    {
        if (await db.Patients.FindAsync(id) is Patient p)
        {
            return Results.Ok(p);
        }

        return Results.NotFound();
    }

    private static async Task<IResult> Delete(PatientDbContext db, Guid id)
    {
        if (await db.Patients.FindAsync(id) is Patient p)
        {
            db.Patients.Remove(p);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    }

    private static async Task<IResult> Upsert(PatientDbContext db, Guid id, PatientInputModel input)
    {
        var p = await db.Patients.FindAsync(id);
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