using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class PortalDbContext : DbContext
{
    public PortalDbContext(DbContextOptions<PortalDbContext> options) : base(options) { }

    //DbSet properties for all the entities that need to be accessed from database.
}