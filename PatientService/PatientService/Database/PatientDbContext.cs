using Microsoft.EntityFrameworkCore;

namespace PatientService;

public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<Patient> Patients => Set<Patient>();
}