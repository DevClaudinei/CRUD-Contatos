using DomainModels.Models;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Persons> Persons { get; set; }
    }
}