using Microsoft.EntityFrameworkCore;
using ExcelDataImport.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }
    public DbSet<ExpaqClaim> Claims { get; set; }
}